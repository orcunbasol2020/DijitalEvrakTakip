using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;

public sealed record GetAllIncomingDocumentQuery() : IRequest<IList<IncomingDocument>>
{
    /// <summary>
    /// "completed" | "pending" | "error" | "all" | null
    /// </summary>
    public string? Status { get; set; }
    public string? AssignedUserFullName { get; set; }

    /// <summary>
    /// Evrağı oluşturan kullanıcı Id'si. Verilmezse tüm kullanıcıların kayıtları döner.
    /// </summary>
    public string? CreatedUserId { get; set; }

    /// <summary>
    /// Departman Id'si. Verilirse sadece o departmana ait kayıtlar döner (aynı birimdeki kullanıcılar birbirinin evraklarını görebilir).
    /// </summary>
    public Guid? DepartmentId { get; set; }
}
