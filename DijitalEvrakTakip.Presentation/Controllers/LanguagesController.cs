using DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.CreateLanguage;
using DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.DeleteLanguage;
using DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.UpdateLanguage;
using DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetAllLanguage;
using DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetLanguageById;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

public sealed class LanguagesController : ApiController
{
    public LanguagesController(IMediator mediator) : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(
        CreateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(
        UpdateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Delete(
        DeleteLanguageCommand request,
        CancellationToken cancellationToken)
    {
        MessageResponse response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllLanguageQuery(),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetLanguageByIdQuery(id),
            cancellationToken);

        if (response is null)
            return NotFound($"Language with Id '{id}' not found.");

        return Ok(response);
    }
}
