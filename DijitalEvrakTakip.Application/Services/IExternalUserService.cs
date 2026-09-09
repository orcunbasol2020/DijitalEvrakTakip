using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Application.Services;

public interface IExternalUserService
{
    Task CreateAsync(CreateExternalUserCommand request, CancellationToken cancellationToken);

    Task<IList<ExternalUser>> GetAllAsync(GetAllExternalUserQuery request, CancellationToken cancellationToken);

    IQueryable<ExternalUser> GetAll();

    Task<ExternalUserDto> GetByExpressionAsync(
        Expression<Func<ExternalUser, bool>> predicate,
        CancellationToken cancellationToken);
}