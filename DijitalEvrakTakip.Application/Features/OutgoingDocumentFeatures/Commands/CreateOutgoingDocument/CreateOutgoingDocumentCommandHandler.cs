using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;

public sealed class CreateOutgoingDocumentCommandHandler
    : IRequestHandler<CreateOutgoingDocumentCommand, MessageResponse>
{
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public CreateOutgoingDocumentCommandHandler(IOutgoingDocumentService outgoingDocumentService)
    {
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateOutgoingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var id = await _outgoingDocumentService.CreateAsync(request, cancellationToken);

        return new MessageResponse(
            Message: "Giden evrak başarıyla oluşturuldu",
            Data: new { Id = id });
    }
}
