using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDepartmentService
{
    Task CreateAsync(Department department, CancellationToken cancellationToken);
    Task<IList<Department>> GetAllAsync(CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Verilen departmanın altındaki tüm departmanların Id'lerini (çok seviyeli) döndürür.
    /// Hiyerarşi Department.ParentId -> üst departmanın Department.Id eşleşmesiyle kurulur.
    /// includeSelf=true ise departmanın kendi Id'si de listeye eklenir.
    /// </summary>
    Task<IList<Guid>> GetDescendantIdsAsync(Guid departmentId, bool includeSelf, CancellationToken cancellationToken);
}
