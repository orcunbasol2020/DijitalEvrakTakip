using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDepartmentService
{
    /// <summary>
    /// Silinmemiş birimler, makam sırasıyla (seviye, seviyedeki sıra, ad). includeMissions=false ise yalnızca merkez birimler
    /// (DepartmentTypes.Central); true ise yurt dışı temsilciliklerimiz de.
    /// </summary>
    Task<IList<Department>> GetAllAsync(bool includeMissions, CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Verilen departmanın altındaki tüm departmanların Id'lerini (çok seviyeli) döndürür.
    /// Hiyerarşi Department.ParentId -> üst departmanın Department.Id eşleşmesiyle kurulur.
    /// includeSelf=true ise departmanın kendi Id'si de listeye eklenir.
    /// </summary>
    Task<IList<int>> GetDescendantIdsAsync(int departmentId, bool includeSelf, CancellationToken cancellationToken);
}
