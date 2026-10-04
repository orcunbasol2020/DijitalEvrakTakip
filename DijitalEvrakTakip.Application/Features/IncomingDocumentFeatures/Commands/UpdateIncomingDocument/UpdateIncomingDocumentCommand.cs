using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;

public sealed record UpdateIncomingDocumentCommand
(
    Guid Id,
    string? OrginalNo,
    string QrCode,
    int? SecurityDegree,
    int? UrgencyDegree,
    int? DocumentTypeId,
    int? LanguageId,
    string? Subject,
    string? Content_Ocr,
    Guid? ExternalInstitutionId,
    Guid? DepartmentId,
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
    bool? HasAttachment = null,
    string? AttachmentDescription = null
): IRequest<MessageResponse>;
