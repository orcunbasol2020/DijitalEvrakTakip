using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.DeleteExternalUser;

public sealed class DeleteExternalUserCommandHandler
    : IRequestHandler<DeleteExternalUserCommand, MessageResponse>
{
    private readonly IExternalUserService _externalUserService;

    public DeleteExternalUserCommandHandler(IExternalUserService externalUserService)
    {
        _externalUserService = externalUserService;
    }

    public async Task<MessageResponse> Handle(
        DeleteExternalUserCommand request,
        CancellationToken cancellationToken)
    {
        var externalUser = await _externalUserService.GetByIdAsync(request.Id, cancellationToken);

        if (externalUser is null)
            return new MessageResponse("Harici kullanıcı bulunamadı");

        await _externalUserService.DeleteAsync(externalUser, cancellationToken);

        return new MessageResponse("Harici kullanıcı silindi");
    }
}
