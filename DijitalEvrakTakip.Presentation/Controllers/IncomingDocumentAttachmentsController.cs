using DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.RemoveIncomingDocumentAttachment;
using DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UpdateIncomingDocumentAttachment;
using DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UploadIncomingDocumentAttachment;
using DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachmentFile;
using DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachments;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Gelen evrağın elektronik ekleri (PDF, en fazla 10 MB). Fiziki ekler evrakın
/// AttachmentDescription alanında tutulur, burada yönetilmez.
/// </summary>
public sealed class IncomingDocumentAttachmentsController : ApiController
{
    public IncomingDocumentAttachmentsController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    [RequestSizeLimit(11_000_000)]
    public async Task<IActionResult> Upload(
        [FromForm] Guid incomingDocumentId,
        [FromForm] IFormFile file,
        [FromForm] string? name,
        [FromForm] string? description,
        [FromForm] string userId,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { Message = "Yüklenecek dosya boş olamaz." });

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);

        var command = new UploadIncomingDocumentAttachmentCommand(
            incomingDocumentId,
            file.FileName,
            memoryStream.ToArray(),
            name,
            description,
            ResolveUserId(userId));

        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateIncomingDocumentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveIncomingDocumentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(
            request with { UserId = ResolveUserId(request.UserId) },
            cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByIncomingDocumentId(
        [FromQuery] Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetIncomingDocumentAttachmentsQuery(incomingDocumentId),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>Ek dosyasını ekin Id'si ile döndürür; dosya adıyla erişim yoktur.</summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> Download(
        [FromQuery] Guid id,
        CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetIncomingDocumentAttachmentFileQuery(id), cancellationToken);

        if (file is null)
            return NotFound(new { Message = "Ek dosyası bulunamadı." });

        // Tarayıcıda açılabilmesi için inline döner; kaydedilirken yüklenen dosyanın adı kullanılır
        Response.Headers["Content-Disposition"] =$"inline; filename*=UTF-8''{Uri.EscapeDataString(file.FileName)}";
        return File(file.Content, file.ContentType);
    }
}
