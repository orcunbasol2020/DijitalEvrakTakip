using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentById;

public sealed record GetOutgoingDocumentByIdQuery(Guid Id) : IRequest<OutgoingDocument>;
