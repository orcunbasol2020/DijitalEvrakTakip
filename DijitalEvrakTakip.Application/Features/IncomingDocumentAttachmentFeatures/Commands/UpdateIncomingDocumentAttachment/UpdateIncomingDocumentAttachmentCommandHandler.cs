using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UpdateIncomingDocumentAttachment;

public sealed class UpdateIncomingDocumentAttachmentCommandHandler
    : IRequestHandler<UpdateIncomingDocumentAttachmentCommand, IncomingDocumentAttachmentDto>
{
    private readonly IIncomingDocumentAttachmentService _attachmentService;

    public UpdateIncomingDocumentAttachmentCommandHandler(IIncomingDocumentAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    public async Task<IncomingDocumentAttachmentDto> Handle(
        UpdateIncomingDocumentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentService.GetByIdAsync(request.Id, cancellationToken);
        if (attachment is null)
            throw new Exception("Ek bulunamadı.");

        if (request.Name != null) attachment.Name = request.Name.Trim();
        if (request.Description != null)
            attachment.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        if (request.OrderNo.HasValue) attachment.OrderNo = request.OrderNo.Value;

        await _attachmentService.UpdateAsync(attachment, cancellationToken);

        return IncomingDocumentAttachmentMapper.ToDto(attachment);
    }
}
