using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.DeleteRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.UpdateRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services
{
    public sealed class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            await EnsureNameIsFreeAsync(request.Name, exceptId: null, cancellationToken);

            Role role = request.Adapt<Role>();
            role.Name = request.Name.Trim();
            await _roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

        public async Task UpdateAsync(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            Role role = await _roleRepository
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Rol bulunamadı.");

            await EnsureNameIsFreeAsync(request.Name, exceptId: role.Id, cancellationToken);

            role.Name = request.Name.Trim();
            role.IsActive = request.IsActive;
            role.UpdateDate = DateTime.UtcNow;
            // Where() takipsiz okur; Update olmadan değişiklik kaydedilmez.
            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            Role role = await _roleRepository
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Rol bulunamadı.");

            // Atanmış rol silinirse kullanıcılar haber verilmeden yetki kaybeder; önce atamalar kaldırılmalı.
            int assignedCount = await _userRoleRepository
                .Where(x => x.RoleId == role.Id && !x.IsDeleted)
                .CountAsync(cancellationToken);

            if (assignedCount > 0)
                throw new Exception($"\"{role.Name}\" rolü {assignedCount} kullanıcıda atanmış; önce rolü kullanıcılardan kaldırın.");

            role.IsDeleted = true;
            role.UpdateDate = DateTime.UtcNow;
            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Ön yüz menü ve sayfa yetkilerini rol adıyla eşleştirdiği için iki rol aynı adı taşıyamaz.
        // Karşılaştırma Türkçe, büyük / küçük harfe duyarsız: "yönetici" = "YÖNETİCİ".
        private async Task EnsureNameIsFreeAsync(string name, Guid? exceptId, CancellationToken cancellationToken)
        {
            string trimmed = name.Trim();
            bool taken = await _roleRepository
                .Where(x => !x.IsDeleted
                    && x.Id != exceptId
                    && EF.Functions.Collate(x.Name, SqlCollations.Turkish) == trimmed)
                .AnyAsync(cancellationToken);

            if (taken)
                throw new Exception($"\"{trimmed}\" adında başka bir rol var.");
        }

        //public IQueryable<Role> GetAll()
        //{
        //    return _roleRepository.GetAll();
        //}

        public async Task<IList<Role>> GetAllAsync(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            IList<Role> roles = await _roleRepository.GetAll().ToListAsync(cancellationToken);
            return roles;
        }
    }
}
