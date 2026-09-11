using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.UpdateExternalUser;

public sealed class UpdateExternalUserCommandHandler
    : IRequestHandler<UpdateExternalUserCommand, MessageResponse>
{
    private readonly IExternalUserService _externalUserService;

    public UpdateExternalUserCommandHandler(IExternalUserService externalUserService)
    {
        _externalUserService = externalUserService;
    }

    public async Task<MessageResponse> Handle(
        UpdateExternalUserCommand request,
        CancellationToken cancellationToken)
    {
        var externalUser = await _externalUserService.GetByIdAsync(request.Id, cancellationToken);

        if (externalUser is null)
            return new MessageResponse("Harici kullanıcı bulunamadı");

        if (!string.IsNullOrWhiteSpace(request.Name))
            externalUser.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Surname))
            externalUser.Surname = request.Surname;

        if (!string.IsNullOrWhiteSpace(request.Email))
            externalUser.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.IdentityNo))
            externalUser.IdentityNo = request.IdentityNo;

        if (request.UserType.HasValue)
            externalUser.UserType = request.UserType.Value;

        if (request.ExternalInstitutionId.HasValue)
            externalUser.ExternalInstitutionId = request.ExternalInstitutionId.Value;

        if (request.IsActive.HasValue)
            externalUser.IsActive = request.IsActive.Value;

        await _externalUserService.UpdateAsync(externalUser, cancellationToken);

        return new MessageResponse("Harici kullanıcı güncellendi");
    }
}
