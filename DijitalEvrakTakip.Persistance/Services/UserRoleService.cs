using DijitalEvrakTakip.Application.Features.UserRoleFeatures.Commands.CreateUserRole;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Repositories;
using GenericRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleService(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task CreateAsync(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        UserRole userRole = request.Adapt<UserRole>();
        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<string>> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _userRoleRepository
            .Where(x => x.UserId == userId)
            .Include(x => x.Role)
            .Select(x => x.Role.Name)
            .ToListAsync(cancellationToken);
    }


    public async Task<IList<UserRole>> GetUserRoleByUser(
        Expression<Func<UserRole, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await _userRoleRepository
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

}
