using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetReceivedDocumentsByUserId;

public sealed class GetReceivedDocumentsByUserIdHandler
    : IRequestHandler<GetReceivedDocumentsByUserIdQuery, PagedResultDto<UserActiveDocumentDto>>
{
    private readonly IUserActiveDocumentService _userActiveDocumentService;

    public GetReceivedDocumentsByUserIdHandler(IUserActiveDocumentService userActiveDocumentService)
    {
        _userActiveDocumentService = userActiveDocumentService;
    }

    public Task<PagedResultDto<UserActiveDocumentDto>> Handle(
        GetReceivedDocumentsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _userActiveDocumentService.GetReceivedByUserIdAsync(
            request.UserId,
            request.DocumentDirection,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
