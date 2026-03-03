using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories
{
    public sealed class DocumentAssignmentRepository
        : Repository<DocumentAssignment, AppDbContext>, IDocumentAssignmentRepository
    {
        public DocumentAssignmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}