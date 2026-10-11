using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IQueryable<User> GetAll()
    {
        return _userRepository.GetAll();
    }

    public IQueryable<User> Search(IQueryable<User> query, string search)
    {
        string term = search.Trim();
        return query.Where(u =>
            EF.Functions.Collate(u.Name, SqlCollations.Turkish).Contains(term)
            || EF.Functions.Collate(u.Surname, SqlCollations.Turkish).Contains(term)
            || EF.Functions.Collate(u.UserName, SqlCollations.Turkish).Contains(term));
    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        // Kaynakta aynı kullanıcı adı eski (silinmiş / pasif) kayıtlarda da geçebilir; geçerli kayıt öne alınır.
        return await _userRepository
            .GetAll()
            .Include(u => u.Department)
            .Where(x => x.UserName == userName)
            .OrderBy(x => x.IsDeleted)
            .ThenByDescending(x => x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository
            .GetAll()
            .Include(u => u.Department)
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
