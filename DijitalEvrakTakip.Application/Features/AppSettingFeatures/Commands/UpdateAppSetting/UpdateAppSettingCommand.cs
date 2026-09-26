using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;

/// <param name="Key">AppSettingKeys sabitlerinden biri.</param>
/// <param name="Value">Yeni değer; boş bırakılabilir.</param>
/// <param name="UserId">Güncelleyen kullanıcı (Guid string).</param>
public sealed record UpdateAppSettingCommand(
    string Key,
    string? Value,
    string? UserId
) : IRequest<MessageResponse>;
