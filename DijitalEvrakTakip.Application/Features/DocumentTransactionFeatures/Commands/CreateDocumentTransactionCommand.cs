using DijitalEvrakTakip.Domain.Dtos;
using MediatR;
using System;

namespace DijitalEvrakTakip.Application.Features.DocumentTransactionFeatures.Commands.CreateDocumentTransaction
{
    public sealed record CreateDocumentTransactionCommand(
        Guid DocumentId,
        string TransactionType,
        string UserId,
        bool IsActive,
        Guid? IncomingDocumentId = null
    ) : IRequest<MessageResponse>;
}
