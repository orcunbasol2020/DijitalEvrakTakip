using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;

public sealed record GetAllOutgoingDocumentQuery() : IRequest<IList<OutgoingDocument>>
{
    /// <summary>
    /// OutgoingDocumentStatusEnum değeri. Verilmezse tüm kayıtlar döner.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Evrağı oluşturan kullanıcı Id'si. Verilmezse tüm kullanıcıların kayıtları döner.
    /// </summary>
    public string? CreatedUserId { get; set; }

    /// <summary>
    /// Departman Id'si. Verilirse sadece o departmana ait kayıtlar döner (aynı birimdeki kullanıcılar birbirinin evraklarını görebilir).
    /// </summary>
    public Guid? DepartmentId { get; set; }
}
