using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.CreateLanguage;

public sealed record CreateLanguageCommand(
    string Name,
    bool OcrSupport = false,
    bool IsActive = true
) : IRequest<MessageResponse>;
