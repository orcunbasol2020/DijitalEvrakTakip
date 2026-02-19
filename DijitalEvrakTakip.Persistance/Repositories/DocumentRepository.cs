using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Persistance.Repositories
{

    public sealed class DocumentRepository : Repository<Document, AppDbContext>, IDocumentRepository
    {
        public DocumentRepository(AppDbContext context) : base(context)
        {
        }
    }
}


