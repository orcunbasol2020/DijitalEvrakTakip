using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.ReceiveDocumentAllocation;

// Evrağın aktif zimmeti kendisinde olan kullanıcı evrağı teslim alır
public sealed record ReceiveDocumentAllocationCommand(
    Guid IncomingDocumentId,
    string UserId
) : IRequest<MessageResponse>;
