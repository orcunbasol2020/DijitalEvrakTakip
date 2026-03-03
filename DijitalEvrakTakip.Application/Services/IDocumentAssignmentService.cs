using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDocumentAssignmentService
{
    // Yeni assignment oluştur (SaveChanges yapmaz)
    void Create(DocumentAssignment documentAssignment);

    // Mevcut assignment güncelle (SaveChanges yapmaz)
    void Update(DocumentAssignment assignment);

    // Tüm assignment’ları getir
    Task<IList<DocumentAssignment>> GetAllAsync(CancellationToken cancellationToken);

    // Belirli evrakın assignment DTO’larını getir
    Task<IList<DocumentAssignmentDto>> GetByDocumentIdAsync(
        Guid documentId,
        CancellationToken cancellationToken);

    // Belirli assignment’ı ID ile getir
    Task<DocumentAssignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    // Belirli evrak için aktif assignment getir
    Task<DocumentAssignment?> GetActiveByDocumentIdAsync(
        Guid documentId,
        CancellationToken cancellationToken);

    // Tek commit / SaveChanges
    Task SaveChangesAsync(CancellationToken cancellationToken);
}