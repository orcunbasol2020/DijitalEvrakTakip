using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IList<Department>> GetAllAsync(
        bool includeMissions,
        CancellationToken cancellationToken)
    {
        // Merkez teşkilatı, DISNET'teki gibi Bakan'ın alt ağacıdır. Kaynakta Bakan'a bağlı olmayan eski / dış
        // kayıtlar (eski bakanlıklar, kapanmış daireler ...) tip olarak merkez olsa da listeye girmez; yurtiçi
        // temsilcilikler ise tip olarak 30001 olsa da bu ağaçta oldukları için gelir.
        var centralIds = (await GetDescendantIdsAsync(DepartmentTypes.MinisterDepartmentId, includeSelf: true, cancellationToken))
            .ToHashSet();

        var departments = await _departmentRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        return departments
            .Where(x => centralIds.Contains(x.Id) || (includeMissions && IsMission(x.Type)))
            // Kardeşler arasındaki sıra (DomisticOrganizationSortOrder; DISNET'teki sıra), sonra makam seviyesi,
            // sonra ad; boşlar sona. Ön yüz ağacı bu sırayı koruyarak kurar.
            .OrderBy(x => x.SortOrder == null)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Level == null)
            .ThenBy(x => x.Level)
            .ThenBy(x => x.Name, TurkishNameComparer)
            .ToList();
    }

    private static readonly StringComparer TurkishNameComparer = StringComparer.Create(new CultureInfo("tr-TR"), ignoreCase: true);

    private static bool IsMission(int? type) =>
        type is >= DepartmentTypes.Embassy and <= DepartmentTypes.HonoraryConsulate;

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _departmentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<int>> GetDescendantIdsAsync(
        int departmentId,
        bool includeSelf,
        CancellationToken cancellationToken)
    {
        // Birim sayısı küçük (~1000) olduğu için tüm liste bir kez çekilip hiyerarşi bellekte gezilir.
        var departments = await _departmentRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Id, x.ParentId })
            .ToListAsync(cancellationToken);

        if (!departments.Any(x => x.Id == departmentId))
            return new List<int>();

        var childrenByParent = departments
            .Where(x => x.ParentId.HasValue)
            .ToLookup(x => x.ParentId!.Value, x => x.Id);

        var result = new List<int>();
        var visited = new HashSet<int> { departmentId };
        if (includeSelf)
            result.Add(departmentId);

        var stack = new Stack<int>();
        stack.Push(departmentId);

        while (stack.Count > 0)
        {
            int parentId = stack.Pop();
            foreach (int childId in childrenByParent[parentId])
            {
                // Döngüsel veri (A->B->A) durumunda sonsuz döngüye girmemek için ziyaret kontrolü.
                if (!visited.Add(childId))
                    continue;

                result.Add(childId);
                stack.Push(childId);
            }
        }

        return result;
    }
}
