using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;

namespace DijitalEvrakTakip.Application.Services
{
    public interface IIncomingDocumentApplicationService
    {
        Task<bool> PreRegisterAsync(
            PreRegisterIncomingDocumentCommand request,
            CancellationToken cancellationToken);
    }

}
