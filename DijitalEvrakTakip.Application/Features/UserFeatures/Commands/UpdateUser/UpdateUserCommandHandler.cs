using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, MessageResponse>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MessageResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return new MessageResponse("Kullanıcı bulunamadı");

        if (!string.IsNullOrWhiteSpace(request.Name))
            user.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Surname))
            user.Surname = request.Surname;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.UserName))
            user.UserName = request.UserName;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (request.DepartmentId.HasValue)
            user.DepartmentId = request.DepartmentId.Value;

        await _userService.UpdateAsync(user, cancellationToken);

        return new MessageResponse("Kullanıcı bilgileri güncellendi");
    }
}
