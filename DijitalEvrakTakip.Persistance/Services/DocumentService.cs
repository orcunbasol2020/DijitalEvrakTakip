using DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services
{
    public sealed class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork _unitOfWork;


        public DocumentService(IDocumentRepository documentRepository, IUnitOfWork unitOfWork)
        {
            _documentRepository = documentRepository;
            _unitOfWork = unitOfWork;
        }


        //public async Task<IList<Document>> GetAllAsync(GetAllDocumentQuery request, CancellationToken cancellationToken)
        //{
        //    IList<Document> documents = await _documentRepository.GetAll().ToListAsync(cancellationToken);
        //    return documents;
        //}

        public async Task<Document> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _documentRepository.GetByExpressionAsync(
                x => x.Id == id,
                cancellationToken
            );
        }


        public async Task<Document> GetByNumberAsync(string number, CancellationToken cancellationToken)
        {
            return await _documentRepository.GetByExpressionAsync(
                x => x.BelgeId == number,
                cancellationToken
            );
        }

        public async Task<IList<Document>> GetAllAsync(GetAllDocumentQuery request, CancellationToken cancellationToken)
        {
            var query = _documentRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.OcrStatus) && request.OcrStatus != "all")
            {
                query = request.OcrStatus switch
                {
                    "completed" => query.Where(x => x.Durum == 1),
                    "pending" => query.Where(x => x.Durum == 0),
                    "error" => query.Where(x => x.Durum == 2),
                    _ => query
                };
            }

            return await query.ToListAsync(cancellationToken);
        }



    }
}
