using DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Commands.ReceiveZimmetChange;
using DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Queries.GetAllAtlasZimmetChanges;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DijitalEvrakTakip.Presentation.Controllers;

/// <summary>
/// Atlas EBYS entegrasyonu tamamlanana kadar kullanılan dummy (stub) uç noktalar.
/// Gerçek Atlas EBYS sistemi, zimmet değişikliği olduğunda bu servise bildirim gönderecek.
/// </summary>
public sealed class AtlasEbysController : ApiController
{
    public AtlasEbysController(IMediator mediator)
        : base(mediator) { }

    [HttpPost("[action]")]
    public async Task<IActionResult> ZimmetDegisikligi(
        ReceiveAtlasZimmetChangeCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllAtlasZimmetChangesQuery(), cancellationToken);
        return Ok(response);
    }
}
