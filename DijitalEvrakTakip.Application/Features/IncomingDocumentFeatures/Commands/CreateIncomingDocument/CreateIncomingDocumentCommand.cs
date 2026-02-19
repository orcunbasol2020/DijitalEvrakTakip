using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;

public sealed record CreateIncomingDocumentCommand
(
    Guid DocumentId,
    string? OrginalNo,
    string QrCode,
    int? SecurityDegree,
    int? DocumentTypeId,
    int? LanguageId,
    string? Subject,
    string? Content_Ocr,
    Guid? ExternalInstitutionId,
    Guid? DepartmentId,
    int? Status,
    bool? ElectronicCopy,
    bool? Release,
    int? PageCount,
    DateTime? DocumentDate,
    DateTime? ReleaseDate,
    int? OcrStatus,
    int? SubmissionStatus,
    string UserId,
    string? DocumentName,
    string? Notes
) : IRequest<MessageResponse>;
