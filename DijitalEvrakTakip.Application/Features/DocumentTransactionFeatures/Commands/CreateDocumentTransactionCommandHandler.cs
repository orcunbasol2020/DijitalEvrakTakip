using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using Mapster;

namespace DijitalEvrakTakip.Application.Features.DocumentTransactionFeatures.Commands.CreateDocumentTransaction;

public sealed class CreateDocumentTransactionCommandHandler
    : IRequestHandler<CreateDocumentTransactionCommand, MessageResponse>
{
    private readonly IDocumentTransactionService _service;

    public CreateDocumentTransactionCommandHandler(IDocumentTransactionService service)
    {
        _service = service;
    }

    public async Task<MessageResponse> Handle(
        CreateDocumentTransactionCommand request,
        CancellationToken cancellationToken)
    {
        DocumentTransaction transaction = request.Adapt<DocumentTransaction>();

        await _service.CreateAsync(transaction, cancellationToken);

        return new("Transaction Başarıyla Kaydedildi");
    }
}
