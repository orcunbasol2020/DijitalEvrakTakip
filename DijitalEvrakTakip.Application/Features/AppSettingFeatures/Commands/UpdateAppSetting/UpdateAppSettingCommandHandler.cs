using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingCommandHandler
    : IRequestHandler<UpdateAppSettingCommand, MessageResponse>
{
    private readonly IAppSettingService _appSettingService;

    public UpdateAppSettingCommandHandler(IAppSettingService appSettingService)
    {
        _appSettingService = appSettingService;
    }

    public async Task<MessageResponse> Handle(
        UpdateAppSettingCommand request,
        CancellationToken cancellationToken)
    {
        Guid? updatedByUserId = Guid.TryParse(request.UserId, out var parsed) ? parsed : null;

        await _appSettingService.UpdateValueAsync(
            request.Key,
            request.Value?.Trim(),
            updatedByUserId,
            cancellationToken);

        return new MessageResponse("Ayar güncellendi.");
    }
}
