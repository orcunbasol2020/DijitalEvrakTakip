using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;

public sealed class PreRegisterIncomingDocumentCommandHandler
    : IRequestHandler<PreRegisterIncomingDocumentCommand, string>
{
    private readonly IIncomingDocumentApplicationService _incomingDocumentApplicationService;

    public PreRegisterIncomingDocumentCommandHandler(
        IIncomingDocumentApplicationService incomingDocumentApplicationService)
    {
        _incomingDocumentApplicationService = incomingDocumentApplicationService;
    }

    public async Task<string> Handle(
        PreRegisterIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        return await _incomingDocumentApplicationService
            .PreRegisterAsync(request, cancellationToken);
    }
}
