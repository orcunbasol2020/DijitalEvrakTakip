using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.CreateScannedDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class ScannedDocumentsController : ApiController
{
    public ScannedDocumentsController(IMediator mediator) : base(mediator) { }

    // Create methodu olduğu gibi.
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateScannedDocumentCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    // GetAll methodu olduğu gibi.
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllScannedDocumentQuery(),
            cancellationToken);

        return Ok(response);
    }

    // Yeni Update endpoint'i ekliyoruz
    [HttpPut("[action]")]
    public async Task<IActionResult> Update(
        UpdateScannedDocumentCommand request,
        CancellationToken cancellationToken)
    {
        // API'ye gelen request'i kullanarak güncelleme işlemini yapıyoruz
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public IActionResult GetPdf(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        fileName = Path.GetFileName(fileName);

        var basePath = @"C:\EvrakTakip\belgeler\Processed";
        var fullPath = Path.Combine(basePath, fileName);

        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

        return File(stream, "application/pdf");
    }
}