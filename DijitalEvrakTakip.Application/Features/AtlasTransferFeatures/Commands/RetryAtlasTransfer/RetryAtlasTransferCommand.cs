using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RetryAtlasTransfer;

/// <summary>
/// Atlas aktarımı hatalı olan evrakı güncel bilgileriyle yeniden aktarım kuyruğuna alır.
/// </summary>
public sealed record RetryAtlasTransferCommand(
    Guid DocumentId,
    string UserId
) : IRequest<MessageResponse>;
