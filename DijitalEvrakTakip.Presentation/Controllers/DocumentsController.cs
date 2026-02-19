using DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocument;
using DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentById;
using DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetDocumentByNumber;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Presentation.Controllers
{
    public sealed class DocumentsController : ApiController
    {
        public DocumentsController(IMediator mediator) : base(mediator)
        {
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        //{
        //    var response = await _mediator.Send(new GetAllDocumentQuery(), cancellationToken);
        //    return Ok(response);
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> GetById([FromQuery] GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response is null)
                return NotFound();

            return Ok(response);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetByNumber([FromQuery] GetDocumentByNumberQuery request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response is null)
                return NotFound();

            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDocumentQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }


    }
}
