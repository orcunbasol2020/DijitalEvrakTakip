using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, MessageResponse>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MessageResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return new MessageResponse("Kullanıcı bulunamadı");

        await _userService.DeleteAsync(user, cancellationToken);

        return new MessageResponse("Kullanıcı silindi");
    }
}
