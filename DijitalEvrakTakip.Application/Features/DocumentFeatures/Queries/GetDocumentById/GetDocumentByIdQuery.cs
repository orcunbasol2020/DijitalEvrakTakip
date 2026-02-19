using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentById
{
    public sealed record GetDocumentByIdQuery(Guid Id) : IRequest<Document>;

}
