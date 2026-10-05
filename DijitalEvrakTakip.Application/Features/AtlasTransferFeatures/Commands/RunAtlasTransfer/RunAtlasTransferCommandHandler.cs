using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RunAtlasTransfer;

public sealed class RunAtlasTransferCommandHandler
    : IRequestHandler<RunAtlasTransferCommand, AtlasTransferRunResultDto>
{
    private readonly IAtlasTransferService _transferService;

    public RunAtlasTransferCommandHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<AtlasTransferRunResultDto> Handle(
        RunAtlasTransferCommand request,
        CancellationToken cancellationToken)
    {
        return _transferService.RunAsync(ignoreEnabledFlag: true, cancellationToken);
    }
}
