using DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocument;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services
{
    public interface IDocumentService
    {
        Task<IList<Document>> GetAllAsync(GetAllDocumentQuery request, CancellationToken cancellationToken);
        Task<Document> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Document> GetByNumberAsync(string number, CancellationToken cancellationToken);
    }
}
