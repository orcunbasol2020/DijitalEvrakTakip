using DijitalEvrakTakip.Application.Eyp;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// EYP için ComAgency'den kurum kodu ve iletişim bilgisi okur. ComAgency kaynak sistemin tablosudur;
/// yalnızca okunur. AgencyId DETSİS kodudur.
/// </summary>
public sealed class ComAgencyLookup
{
    // Birim ağacında yukarı çıkarken sonsuz döngüye karşı sınır
    private const int MaxDepth = 20;

    private readonly AppDbContext _context;

    public ComAgencyLookup(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Kurumun iletişim bilgisi; kayıt yoksa null.</summary>
    public async Task<EypContact?> GetContactAsync(int agencyId, CancellationToken cancellationToken)
    {
        var row = await _context.Database.SqlQuery<AgencyContactRow>($"""
                SELECT Address, Phone, Fax, Email, WebAddress
                FROM ComAgency
                WHERE AgencyId = {agencyId}
                """)
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : new EypContact(row.Address, row.Phone, row.Fax, row.Email, row.WebAddress);
    }

    /// <summary>
    /// İç birimin (ComDepartment) kurum kodları. Kkk birimin aktif ComAgency kaydının kodudur; birimin aktif
    /// kaydı yoksa aktif kaydı olan ilk üst birimin kodu (kapatılmış kod Atlas'ta tanınmayabilir). BirimKkk
    /// birimin kendi kodudur (kaydı kapatılmış olsa da); hiç kaydı yoksa Kkk. Ağaçta kod bulunamazsa ikisi de null.
    /// </summary>
    public async Task<(string? Kkk, string? BirimKkk)> ResolveDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        // Kapatılmış kayıt da olsa birimin kendi kodu; aktif kayıt varsa o
        var ownCode = await FirstCodeAsync($"""
                SELECT TOP 1 AgencyId AS Value
                FROM ComAgency
                WHERE DepartmentId = {departmentId}
                ORDER BY ISNULL(IsDeleted, 0), AgencyId
                """, cancellationToken);

        int? current = departmentId;
        for (var depth = 0; current is not null && depth < MaxDepth; depth++)
        {
            var id = current.Value;
            var activeCode = await FirstCodeAsync($"""
                    SELECT TOP 1 AgencyId AS Value
                    FROM ComAgency
                    WHERE DepartmentId = {id} AND ISNULL(IsDeleted, 0) = 0 AND ISNULL(IsActive, 1) = 1
                    ORDER BY AgencyId
                    """, cancellationToken);

            if (activeCode is not null)
                return (activeCode.Value.ToString(), (ownCode ?? activeCode.Value).ToString());

            current = await _context.Set<Department>()
                .Where(x => x.Id == id)
                .Select(x => x.ParentId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return (null, ownCode?.ToString());
    }

    // Sorgu TOP 1 ile tek satır döner; EF'in alt sorgu oluşturmaması için sonuç listeyle alınır
    private async Task<int?> FirstCodeAsync(FormattableString sql, CancellationToken cancellationToken)
    {
        var codes = await _context.Database.SqlQuery<int>(sql).ToListAsync(cancellationToken);
        return codes.Count > 0 ? codes[0] : null;
    }

    private sealed class AgencyContactRow
    {
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? WebAddress { get; set; }
    }
}
