using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories
{
    public sealed class ExternalInstitutionRepository
        : Repository<ExternalInstitution, AppDbContext>, IExternalInstitutionRepository
    {
        public ExternalInstitutionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
