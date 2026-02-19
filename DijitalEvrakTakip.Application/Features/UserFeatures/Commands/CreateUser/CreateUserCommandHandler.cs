using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, MessageResponse>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MessageResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // islemler ...

        await _userService.CreateAsync(request, cancellationToken);

        return new("Kullanıcı Başarıyla Kaydedildi");
    }
}
