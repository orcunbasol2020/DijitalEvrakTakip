using Azure.Core;
using DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;
using DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;


    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        //User user = new()
        //{
        //    Name = request.Name,
        //    Surname = request.Surname,
        //    Email = request.Email,
        //    IsActive = request.IsActive,
        //    DepartmentId = request.DepartmentId
        //};

        //User user = request.Adapt<User>();
        //await _context.Set<User>().AddAsync(user, cancellationToken);
        //await _context.SaveChangesAsync(cancellationToken);
        User user = request.Adapt<User>();
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }

    public async Task<IList<User>> GetAllAsync(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        IList<User> users = await _userRepository.GetAll().ToListAsync(cancellationToken);
        return users;
    }

    public IQueryable<User> GetAll()
    {
        return _userRepository.GetAll();
    }

    public Task<UserLoginDto> GetUser(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    //public async Task<UserLoginDto> GetUserByUserName(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
    //{
    //    var login = await _userRepository.GetByExpressionAsync(predicate);
    //    UserLoginDto user = login.Adapt<UserLoginDto>();
    //    return user;
    //}


    public async Task<UserLoginDto> GetUserByUserName(
    Expression<Func<User, bool>> predicate,
    CancellationToken cancellationToken)
    {
        var login = await _userRepository
            .GetAll()
            .Include(u => u.Department)
            .FirstOrDefaultAsync(predicate, cancellationToken);

        if (login == null)
            return null;

        UserLoginDto user = login.Adapt<UserLoginDto>();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository
            .GetAll()
            .Include(u => u.Department)
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        user.IsDeleted = true;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}
