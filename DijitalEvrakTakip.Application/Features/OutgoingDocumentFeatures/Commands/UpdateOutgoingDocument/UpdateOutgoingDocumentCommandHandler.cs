using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.UpdateOutgoingDocument;

public sealed class UpdateOutgoingDocumentCommandHandler
    : IRequestHandler<UpdateOutgoingDocumentCommand, MessageResponse>
{
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public UpdateOutgoingDocumentCommandHandler(IOutgoingDocumentService outgoingDocumentService)
    {
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        UpdateOutgoingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        await _outgoingDocumentService.UpdateAsync(request, cancellationToken);

        return new MessageResponse("Giden evrak başarıyla güncellendi");
    }
}
