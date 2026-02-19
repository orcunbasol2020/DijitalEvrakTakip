using DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;
using DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;
using DijitalEvrakTakip.Application.Services;
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
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            Role role = request.Adapt<Role>();
            await _roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
