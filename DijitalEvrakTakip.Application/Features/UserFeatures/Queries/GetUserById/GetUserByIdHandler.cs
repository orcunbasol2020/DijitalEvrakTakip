using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserService _userService;

    public GetUserByIdHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return null;

        return new UserDto
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            UserName = user.UserName,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name,
            DepartmentShortName = user.Department?.ShortName,
            TitleName = user.TitleName,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };
    }
}
