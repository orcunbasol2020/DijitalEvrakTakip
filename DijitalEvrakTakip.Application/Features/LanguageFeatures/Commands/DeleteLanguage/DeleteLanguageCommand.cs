using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.DeleteLanguage;

public sealed record DeleteLanguageCommand(
    Guid Id
) : IRequest<MessageResponse>;
