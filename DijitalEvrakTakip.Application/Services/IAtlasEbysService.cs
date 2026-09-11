using DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Commands.ReceiveZimmetChange;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IAtlasEbysService
{
    Task<MessageResponse> ReceiveZimmetChangeAsync(
        ReceiveAtlasZimmetChangeCommand request,
        CancellationToken cancellationToken);

    Task<IList<AtlasZimmetChange>> GetAllAsync(CancellationToken cancellationToken);
}
