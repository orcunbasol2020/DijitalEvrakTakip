using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetAllocationRequestsByDocumentId;

// Evrağın tüm zimmet talepleri (bekleyen ve sonuçlanan), eskiden yeniye
public sealed record GetAllocationRequestsByDocumentIdQuery(Guid IncomingDocumentId)
    : IRequest<IList<DocumentAllocationRequestDto>>;
