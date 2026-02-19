using MediatR;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;

public sealed class UpdateIncomingDocumentCommandHandler
    : IRequestHandler<UpdateIncomingDocumentCommand, MessageResponse>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public UpdateIncomingDocumentCommandHandler(
        IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        UpdateIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            throw new ArgumentException("Geçersiz evrak Id.");

        await _incomingDocumentService.UpdateAsync(request, cancellationToken);

        return new MessageResponse("Gelen evrak başarıyla güncellendi.");
    }
}
