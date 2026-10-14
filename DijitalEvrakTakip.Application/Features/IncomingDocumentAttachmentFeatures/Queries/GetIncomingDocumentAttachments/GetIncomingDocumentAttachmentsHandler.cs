using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachments;

public sealed class GetIncomingDocumentAttachmentsHandler
    : IRequestHandler<GetIncomingDocumentAttachmentsQuery, IList<IncomingDocumentAttachmentDto>>
{
    private readonly IIncomingDocumentAttachmentService _attachmentService;

    public GetIncomingDocumentAttachmentsHandler(IIncomingDocumentAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    public async Task<IList<IncomingDocumentAttachmentDto>> Handle(
        GetIncomingDocumentAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        var attachments = await _attachmentService
            .GetByIncomingDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        return attachments.Select(IncomingDocumentAttachmentMapper.ToDto).ToList();
    }
}
