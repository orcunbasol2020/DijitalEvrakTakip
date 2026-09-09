using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;

public sealed record CreateOutgoingDocumentCommand
(
    string? QrCode,
    string? OriginalDocumentNumber,
    string? SecurityDegree,
    int? Type,
    int? LanguageId,
    string? Subject,
    string? Content_Ocr,
    int? Status,
    Guid? DepartmentId,
    Guid? ExternalInstitutonId,
    bool? ElectronicCopy,
    bool? EbysTransfer,
    int? PageCount,
    string? Notes,
    DateTime? DocumentDate,
    string CreatedUserId
) : IRequest<MessageResponse>;
