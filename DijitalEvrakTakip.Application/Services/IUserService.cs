using DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Application.Services;

public interface IUserService
{
    Task CreateAsync(CreateUserCommand request, CancellationToken cancellationToken);
    Task<IList<User>> GetAllAsync(GetAllUserQuery request, CancellationToken cancellationToken);
    public IQueryable<User> GetAll();
    Task<UserLoginDto> GetUserByUserName(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken);

}
