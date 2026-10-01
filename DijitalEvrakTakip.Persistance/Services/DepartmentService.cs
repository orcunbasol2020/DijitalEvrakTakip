using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        Department department,
        CancellationToken cancellationToken)
    {
        await _departmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Department>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _departmentRepository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _departmentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<Guid>> GetDescendantIdsAsync(
        Guid departmentId,
        bool includeSelf,
        CancellationToken cancellationToken)
    {
        // Departman sayısı az olduğu için tüm liste bir kez çekilip hiyerarşi bellekte gezilir.
        var departments = await _departmentRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Id, x.ParentId })
            .ToListAsync(cancellationToken);

        if (!departments.Any(x => x.Id == departmentId))
            return new List<Guid>();

        var childrenByParent = departments
            .Where(x => x.ParentId.HasValue)
            .ToLookup(x => x.ParentId!.Value, x => x.Id);

        var result = new List<Guid>();
        var visited = new HashSet<Guid> { departmentId };
        if (includeSelf)
            result.Add(departmentId);

        var stack = new Stack<Guid>();
        stack.Push(departmentId);

        while (stack.Count > 0)
        {
            Guid parentId = stack.Pop();
            foreach (Guid childId in childrenByParent[parentId])
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
