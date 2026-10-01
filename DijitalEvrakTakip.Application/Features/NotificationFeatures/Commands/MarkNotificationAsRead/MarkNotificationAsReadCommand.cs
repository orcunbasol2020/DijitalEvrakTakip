using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    Guid Id,
    string UserId
) : IRequest<MessageResponse>;
