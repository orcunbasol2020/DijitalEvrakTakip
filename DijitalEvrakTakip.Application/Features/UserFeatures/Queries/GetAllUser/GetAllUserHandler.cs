using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser
{
    public sealed class GetAllUserHandler : IRequestHandler<GetAllUserQuery, IList<UserDto>>
    {
        private readonly IUserService _userService;

        public GetAllUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IList<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            //IList<UserDto> users = await _userService.GetAllAsync(request, cancellationToken);
            //return users.OrderBy(u => u.Name).ToList();

            var query = _userService
                .GetAll()
                .Include(u => u.Department);

            var result = await query
                .ProjectToType<UserDto>()
                .OrderBy(u => u.Name)
                .ToListAsync(cancellationToken);

            return result;
        }
    }


}
