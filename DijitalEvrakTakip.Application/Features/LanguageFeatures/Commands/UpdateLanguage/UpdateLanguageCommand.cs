using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.UpdateLanguage;

public sealed record UpdateLanguageCommand(
    Guid Id,
    string? Name,
    bool? OcrSupport,
    bool? IsActive
) : IRequest<MessageResponse>;
