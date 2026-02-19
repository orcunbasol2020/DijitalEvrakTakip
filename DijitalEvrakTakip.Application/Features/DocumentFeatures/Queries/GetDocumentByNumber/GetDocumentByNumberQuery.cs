using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentByNumber
{
    public sealed record GetDocumentByNumberQuery(string documentNumber) : IRequest<Document>;
}
