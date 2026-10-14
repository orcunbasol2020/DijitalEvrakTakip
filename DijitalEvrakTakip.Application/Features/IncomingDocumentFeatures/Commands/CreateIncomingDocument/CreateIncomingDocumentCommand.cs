using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;

public sealed record CreateIncomingDocumentCommand
(
    Guid DocumentId,
    string? OrginalNo,
    string QrCode,
    int? SecurityDegree,
    int? UrgencyDegree,
    int? DocumentTypeId,
    int? LanguageId,
    string? Subject,
    string? Content_Ocr,
    int? ExternalInstitutionId,
    int? DepartmentId,
    int? Status,
    bool? ElectronicCopy,
    bool? Release,
    bool? ActionRequired,
    int? PageCount,
    DateTime? DocumentDate,
    DateTime? ReleaseDate,
    int? OcrStatus,
    int? SubmissionStatus,
    string UserId,
    string? DocumentName,
    string? Notes,
    string? CreatedUserId = null,
    bool? HasAttachment = null,
    string? AttachmentDescription = null,
    // "Nereye" listesi. Gönderilmezse DepartmentId tek alıcı olarak kaydedilir
    IList<IncomingDocumentRecipient>? Recipients = null
) : IRequest<MessageResponse>;
