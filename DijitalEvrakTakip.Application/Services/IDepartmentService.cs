using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDepartmentService
{
    Task CreateAsync(Department department, CancellationToken cancellationToken);
    Task<IList<Department>> GetAllAsync(CancellationToken cancellationToken);
}
