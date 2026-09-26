using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetDocumentsByStatus;

public sealed record GetDocumentsByStatusQuery() : IRequest<IList<IncomingDocument>>
{
    /// <summary>
    /// DocumentStatusEnum değeri: 1 Ön Kayıt, 2 Güncelleme, 3 Teslim, 4 Eşleştirme, 5 Ocr, 6 Yayınla.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Verilirse sadece o departmana ait kayıtlar döner.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Verilirse sadece o kullanıcının oluşturduğu kayıtlar döner.
    /// </summary>
    public string? CreatedUserId { get; set; }
}
