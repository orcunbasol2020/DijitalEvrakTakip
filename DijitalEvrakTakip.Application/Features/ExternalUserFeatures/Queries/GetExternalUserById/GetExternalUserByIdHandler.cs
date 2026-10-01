using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using Mapster;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetExternalUserById;

public sealed class GetExternalUserByIdHandler
    : IRequestHandler<GetExternalUserByIdQuery, ExternalUserDto?>
{
    private readonly IExternalUserService _externalUserService;

    public GetExternalUserByIdHandler(IExternalUserService externalUserService)
    {
        _externalUserService = externalUserService;
    }

    public async Task<ExternalUserDto?> Handle(
        GetExternalUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _externalUserService.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return null;

        return entity.Adapt<ExternalUserDto>();
    }
}
