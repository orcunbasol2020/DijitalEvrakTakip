using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;

public sealed class CreateExternalUserCommandHandler
    : IRequestHandler<CreateExternalUserCommand, MessageResponse>
{
    private readonly IExternalUserService _externalUserService;

    public CreateExternalUserCommandHandler(IExternalUserService externalUserService)
    {
        _externalUserService = externalUserService;
    }

    public async Task<MessageResponse> Handle(
        CreateExternalUserCommand request,
        CancellationToken cancellationToken)
    {
        await _externalUserService.CreateAsync(request, cancellationToken);

        return new MessageResponse("Harici kullanıcı oluşturuldu");
    }
}
