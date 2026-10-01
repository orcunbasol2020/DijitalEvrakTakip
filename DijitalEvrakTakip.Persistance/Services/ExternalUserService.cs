using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;
using DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ExternalUserService : IExternalUserService
{
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExternalUserService(IExternalUserRepository externalUserRepository, IUnitOfWork unitOfWork)
    {
        _externalUserRepository = externalUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateExternalUserCommand request, CancellationToken cancellationToken)
    {
        ExternalUser user = request.Adapt<ExternalUser>();

        await _externalUserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<ExternalUser>> GetAllAsync(GetAllExternalUserQuery request, CancellationToken cancellationToken)
    {
        IList<ExternalUser> users = await _externalUserRepository
            .GetAll()
            .ToListAsync(cancellationToken);

        return users;
    }

    public IQueryable<ExternalUser> GetAll()
    {
        return _externalUserRepository.GetAll();
    }

    public async Task<ExternalUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _externalUserRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ExternalUserDto> GetByExpressionAsync(
        Expression<Func<ExternalUser, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var entity = await _externalUserRepository
            .GetAll()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        if (entity == null)
            return null;

        return entity.Adapt<ExternalUserDto>();
    }

    public async Task UpdateAsync(ExternalUser externalUser, CancellationToken cancellationToken)
    {
        _externalUserRepository.Update(externalUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ExternalUser externalUser, CancellationToken cancellationToken)
    {
        externalUser.IsDeleted = true;
        _externalUserRepository.Update(externalUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}