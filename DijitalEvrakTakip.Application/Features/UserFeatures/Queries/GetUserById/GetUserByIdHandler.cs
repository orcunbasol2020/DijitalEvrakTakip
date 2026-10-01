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
            DepartmentId = user.DepartmentId.ToString(),
            DepartmentName = user.Department?.Name,
            DepartmentShortName = user.Department?.ShortName,
            UserType = user.UserType,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted,
            CreatedDate = user.CreatedDate,
            UpdateDate = user.UpdateDate
        };
    }
}
