using System.Globalization;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaList;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSenders;
using DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSummary;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class NotaReportService : INotaReportService
{
    // Ay / yıl sınırları Türkiye saatine göre hesaplanır (UTC+3, 2016'dan beri yaz saati yok).
    // CreatedDate UTC yazılır, bu yüzden kaydırılır; DocumentDate kullanıcının girdiği takvim
    // günüdür, olduğu gibi kullanılır. DocumentDate boşsa CreatedDate'e düşülür.
    private const int TurkeyUtcOffsetHours = 3;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private static readonly StringComparer TurkishComparer =
        StringComparer.Create(CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true);

    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IExternalInstitutionRepository _externalInstitutionRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public NotaReportService(
        IIncomingDocumentRepository incomingDocumentRepository,
        IExternalInstitutionRepository externalInstitutionRepository,
        IDepartmentRepository departmentRepository)
    {
        _incomingDocumentRepository = incomingDocumentRepository;
        _externalInstitutionRepository = externalInstitutionRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<IList<NotaSenderDto>> GetSendersAsync(
        GetNotaSendersQuery request,
        CancellationToken cancellationToken)
    {
        bool useDocumentDate = NotaReportPeriodRules.UseDocumentDate(request.DateField);

        var counts = await ApplyPeriod(Notas(), useDocumentDate, request.Year, month: null)
            .GroupBy(x => x.ExternalInstitutionId!.Value)
            .Select(g => new { ExternalInstitutionId = g.Key, NotaCount = g.Count(), LastNotaDate = g.Max(x => x.CreatedDate) })
            .ToListAsync(cancellationToken);

        var ids = counts.Select(x => x.ExternalInstitutionId).ToList();

        // Silinmiş gönderenlerin notaları listeye girmez
        var institutions = await _externalInstitutionRepository
            .GetAll()
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .Select(x => new { x.Id, x.Name, x.Type })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return counts
            .Where(x => institutions.ContainsKey(x.ExternalInstitutionId))
            .Select(x =>
            {
                var institution = institutions[x.ExternalInstitutionId];
                return new NotaSenderDto(institution.Id, institution.Name, institution.Type, x.NotaCount, x.LastNotaDate);
            })
            .OrderByDescending(x => x.NotaCount)
            .ThenBy(x => x.Name, TurkishComparer)
            .ToList();
    }

    public async Task<NotaSummaryDto> GetSummaryAsync(
        GetNotaSummaryQuery request,
        CancellationToken cancellationToken)
    {
        Guid externalInstitutionId = request.ExternalInstitutionId!.Value;
        bool useDocumentDate = NotaReportPeriodRules.UseDocumentDate(request.DateField);

        var institution = await _externalInstitutionRepository
            .GetAll()
            .Where(x => x.Id == externalInstitutionId)
            .Select(x => new { x.Id, x.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (institution is null)
            throw new Exception("Dış kurum bulunamadı.");

        // Misyon olmayan bir kurum için Notas() boş döner, özet sıfır sayılarla gelir.
        // Bir misyonun notaları az sayıda olduğundan hafif bir izdüşüm çekilip sayım bellekte yapılır;
        // years dönem süzgecinden bağımsız olduğu için tüm notalar gerekiyor.
        var notas = await Notas()
            .Where(x => x.ExternalInstitutionId == externalInstitutionId)
            .Select(x => new NotaRow(
                x.DocumentTypeId,
                x.CreatedDate,
                x.DocumentDate,
                x.Distributions
                    .Where(d => !d.IsDeleted)
                    .Select(d => new NotaRecipientRow(d.DepartmentId, d.ActionRequired))
                    .ToList()))
            .ToListAsync(cancellationToken);

        DateTime PeriodDate(NotaRow row) => GetPeriodDate(row.CreatedDate, row.DocumentDate, useDocumentDate);

        var years = notas
            .GroupBy(x => PeriodDate(x).Year)
            .Select(g => new NotaYearCountDto(g.Key, g.Count()))
            .OrderByDescending(x => x.Year)
            .ToList();

        var filtered = notas
            .Where(x => (!request.Year.HasValue || PeriodDate(x).Year == request.Year.Value) &&
                        (!request.Month.HasValue || PeriodDate(x).Month == request.Month.Value))
            .ToList();

        // Bir nota birden fazla birime gidebilir; birim başına bir kez sayılır
        var departmentCounts = filtered
            .SelectMany(x => x.Recipients.DistinctBy(r => r.DepartmentId))
            .GroupBy(x => x.DepartmentId)
            .Select(g => new
            {
                DepartmentId = g.Key,
                NotaCount = g.Count(),
                // null Atlas'a Gereği olarak gider
                GeregiCount = g.Count(r => r.ActionRequired != false),
                BilgiCount = g.Count(r => r.ActionRequired == false)
            })
            .ToList();

        var departmentIds = departmentCounts.Select(x => x.DepartmentId).ToList();
        var departments = await _departmentRepository
            .GetAll()
            .Where(x => departmentIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name, x.ShortName })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var departmentDtos = departmentCounts
            .Select(x =>
            {
                departments.TryGetValue(x.DepartmentId, out var department);
                return new NotaDepartmentCountDto(
                    x.DepartmentId, department?.Name, department?.ShortName,
                    x.NotaCount, x.GeregiCount, x.BilgiCount);
            })
            .OrderByDescending(x => x.NotaCount)
            .ThenBy(x => x.DepartmentName ?? string.Empty, TurkishComparer)
            .ToList();

        var monthCounts = filtered
            .GroupBy(x => (PeriodDate(x).Year, PeriodDate(x).Month))
            .ToDictionary(g => g.Key, g => g.Count());

        var months = BuildMonths(notas.Select(PeriodDate).ToList(), request.Year, request.Month)
            .Select(m => new NotaMonthCountDto(m.Year, m.Month, monthCounts.GetValueOrDefault(m)))
            .ToList();

        return new NotaSummaryDto(
            institution.Id,
            institution.Name,
            filtered.Count,
            filtered.Count(x => x.DocumentTypeId == NotaDocumentTypes.ENota),
            filtered.Count(x => x.Recipients.Count == 0),
            filtered.Count == 0 ? null : filtered.Max(x => x.CreatedDate),
            departmentDtos,
            months,
            years);
    }

    public async Task<PagedResultDto<NotaListItemDto>> GetListAsync(
        GetNotaListQuery request,
        CancellationToken cancellationToken)
    {
        Guid externalInstitutionId = request.ExternalInstitutionId!.Value;
        bool useDocumentDate = NotaReportPeriodRules.UseDocumentDate(request.DateField);

        var query = ApplyPeriod(
            Notas().Where(x => x.ExternalInstitutionId == externalInstitutionId),
            useDocumentDate, request.Year, request.Month);

        if (request.DepartmentId.HasValue)
        {
            Guid departmentId = request.DepartmentId.Value;
            query = query.Where(x => x.Distributions.Any(d => !d.IsDeleted && d.DepartmentId == departmentId));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        int pageSize = request.PageSize is null or <= 0 ? DefaultPageSize : Math.Min(request.PageSize.Value, MaxPageSize);
        int page = request.Page is null or <= 0 ? 1 : request.Page.Value;

        var ordered = useDocumentDate
            ? query.OrderByDescending(x => x.DocumentDate ?? x.CreatedDate.AddHours(TurkeyUtcOffsetHours))
            : query.OrderByDescending(x => x.CreatedDate);

        var items = await ordered
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new NotaListItemDto(
                x.Id,
                x.QrCode,
                x.OrginalNo,
                x.Subject,
                x.DocumentTypeId,
                x.CreatedDate,
                x.DocumentDate,
                // Süzülen birim değil, notanın tüm alıcıları; eklenme sırasıyla
                x.Distributions
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.CreatedDate)
                    .Select(d => new NotaRecipientDto(
                        d.DepartmentId,
                        d.Department != null ? d.Department.Name : null,
                        d.Department != null ? d.Department.ShortName : null,
                        d.ActionRequired))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<NotaListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Yabancı bir misyondan gelen her evrak, türü ne olursa olsun notadır
    private IQueryable<IncomingDocument> Notas() =>
        _incomingDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted &&
                        x.ExternalInstitutionId != null &&
                        x.ExternalInstitution!.Type == ExternalInstitutionTypes.Mission);

    // GetPeriodDate ile aynı kuralı SQL'e çevrilebilir biçimde uygular
    private static IQueryable<IncomingDocument> ApplyPeriod(
        IQueryable<IncomingDocument> query,
        bool useDocumentDate,
        int? year,
        int? month)
    {
        if (year.HasValue)
        {
            var start = new DateTime(year.Value, month ?? 1, 1);
            var end = month.HasValue ? start.AddMonths(1) : start.AddYears(1);

            if (useDocumentDate)
                return query.Where(x =>
                    (x.DocumentDate ?? x.CreatedDate.AddHours(TurkeyUtcOffsetHours)) >= start &&
                    (x.DocumentDate ?? x.CreatedDate.AddHours(TurkeyUtcOffsetHours)) < end);

            var utcStart = start.AddHours(-TurkeyUtcOffsetHours);
            var utcEnd = end.AddHours(-TurkeyUtcOffsetHours);
            return query.Where(x => x.CreatedDate >= utcStart && x.CreatedDate < utcEnd);
        }

        if (month.HasValue)
        {
            int m = month.Value;
            return useDocumentDate
                ? query.Where(x => (x.DocumentDate ?? x.CreatedDate.AddHours(TurkeyUtcOffsetHours)).Month == m)
                : query.Where(x => x.CreatedDate.AddHours(TurkeyUtcOffsetHours).Month == m);
        }

        return query;
    }

    private static DateTime GetPeriodDate(DateTime createdDate, DateTime? documentDate, bool useDocumentDate) =>
        useDocumentDate && documentDate.HasValue
            ? documentDate.Value
            : createdDate.AddHours(TurkeyUtcOffsetHours);

    /// <summary>
    /// Aylık seyir eksenindeki aylar: yıl verilmişse o yılın 12 ayı (ay da verilmişse yalnızca o ay);
    /// yıl verilmemişse ilk ve son nota arasındaki aylar boşluksuz (ay verilmişse her yılın o ayı).
    /// </summary>
    private static IEnumerable<(int Year, int Month)> BuildMonths(IList<DateTime> periodDates, int? year, int? month)
    {
        if (year.HasValue)
        {
            if (month.HasValue)
                return new[] { (year.Value, month.Value) };

            return Enumerable.Range(1, 12).Select(m => (year.Value, m));
        }

        if (periodDates.Count == 0)
            return Array.Empty<(int, int)>();

        var first = periodDates.Min();
        var last = periodDates.Max();

        if (month.HasValue)
            return Enumerable.Range(first.Year, last.Year - first.Year + 1).Select(y => (y, month.Value));

        var result = new List<(int Year, int Month)>();
        for (var cursor = new DateTime(first.Year, first.Month, 1); cursor <= last; cursor = cursor.AddMonths(1))
            result.Add((cursor.Year, cursor.Month));

        return result;
    }

    private sealed record NotaRow(
        int? DocumentTypeId,
        DateTime CreatedDate,
        DateTime? DocumentDate,
        List<NotaRecipientRow> Recipients);

    private sealed record NotaRecipientRow(Guid DepartmentId, bool? ActionRequired);
}
