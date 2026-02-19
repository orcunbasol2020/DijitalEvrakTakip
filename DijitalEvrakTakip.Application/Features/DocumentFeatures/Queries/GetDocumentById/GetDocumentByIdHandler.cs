using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentById
{
    public sealed class GetDocumentByIdHandler
     : IRequestHandler<GetDocumentByIdQuery, Document>
    {
        private readonly IDocumentRepository _documentRepository;

        public GetDocumentByIdHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<Document> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _documentRepository.GetByExpressionAsync(
                x => x.Id == request.Id,
                cancellationToken
            );
        }

    }

}
