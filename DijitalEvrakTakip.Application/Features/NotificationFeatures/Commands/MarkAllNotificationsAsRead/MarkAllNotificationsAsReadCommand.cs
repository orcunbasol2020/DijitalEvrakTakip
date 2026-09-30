using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotificationFeatures.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(
    string UserId
) : IRequest<MessageResponse>;
