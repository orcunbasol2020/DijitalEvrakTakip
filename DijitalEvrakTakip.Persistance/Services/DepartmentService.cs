using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
        return await _departmentRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && (includeMissions || x.Type == DepartmentTypes.Central))
            // Makam sırası: önce seviye (Bakan, Bakan Yardımcılığı, GM ...), sonra seviyedeki sıra, sonra ad;
            // seviyesi / sırası boş olanlar sona
            .OrderBy(x => x.Level == null)
            .ThenBy(x => x.Level)
            .ThenBy(x => x.SortOrder == null)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

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
