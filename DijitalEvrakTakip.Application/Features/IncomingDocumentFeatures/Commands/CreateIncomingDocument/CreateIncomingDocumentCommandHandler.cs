using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;

public sealed class CreateIncomingDocumentCommandHandler
    : IRequestHandler<CreateIncomingDocumentCommand, MessageResponse>
{
    private readonly IIncomingDocumentService _incomingDocumentService;

    public CreateIncomingDocumentCommandHandler(
        IIncomingDocumentService incomingDocumentService)
    {
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        await _incomingDocumentService.CreateAsync(request, cancellationToken);

        return new MessageResponse("Gelen Evrak Başarıyla Oluşturuldu.");
    }
}
