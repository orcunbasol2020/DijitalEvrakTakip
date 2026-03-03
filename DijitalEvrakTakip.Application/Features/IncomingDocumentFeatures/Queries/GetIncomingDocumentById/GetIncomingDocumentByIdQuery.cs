using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentById;

public sealed record GetIncomingDocumentByIdQuery(Guid Id) : IRequest<IncomingDocument>;