using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveByDocumentId;

public sealed record GetActiveDocumentAllocationByDocumentIdQuery(Guid IncomingDocumentId)
    : IRequest<DocumentAllocationDto>;  // Sadece tek bir aktif kayıt dönecek