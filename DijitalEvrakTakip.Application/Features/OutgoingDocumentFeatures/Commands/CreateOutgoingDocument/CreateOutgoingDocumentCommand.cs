using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;

public sealed record CreateOutgoingDocumentCommand
(
    string? QrCode,
    string? OriginalDocumentNumber,
    int? SecurityDegree,
    int? UrgencyDegree,
    int? Type,
    Guid? LanguageId,
    string? Subject,
    string? Content_Ocr,
    int? Status,
    Guid? DepartmentId,
    Guid? ExternalInstitutonId,
    bool? ElectronicCopy,
    bool? EbysTransfer,
    bool? ActionRequired,
    int? PageCount,
    string? Notes,
    DateTime? DocumentDate,
    string CreatedUserId
) : IRequest<MessageResponse>;
