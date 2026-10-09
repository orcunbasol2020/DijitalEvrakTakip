using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.RemoveIncomingDocumentAttachment;

public sealed class RemoveIncomingDocumentAttachmentCommandHandler
    : IRequestHandler<RemoveIncomingDocumentAttachmentCommand, MessageResponse>
{
    private readonly IIncomingDocumentAttachmentService _attachmentService;

    public RemoveIncomingDocumentAttachmentCommandHandler(IIncomingDocumentAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    public async Task<MessageResponse> Handle(
        RemoveIncomingDocumentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await _attachmentService.RemoveAsync(request.Id, request.UserId, cancellationToken);

        return new("Ek kaldırıldı");
    }
}
