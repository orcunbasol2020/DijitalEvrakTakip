using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveDocumentsByUserId;

public sealed class GetActiveDocumentsByUserIdHandler
    : IRequestHandler<GetActiveDocumentsByUserIdQuery, PagedResultDto<UserActiveDocumentDto>>
{
    private readonly IUserActiveDocumentService _userActiveDocumentService;

    public GetActiveDocumentsByUserIdHandler(IUserActiveDocumentService userActiveDocumentService)
    {
        _userActiveDocumentService = userActiveDocumentService;
    }

    public Task<PagedResultDto<UserActiveDocumentDto>> Handle(
        GetActiveDocumentsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _userActiveDocumentService.GetActiveByUserIdAsync(
            request.UserId,
            request.DocumentDirection,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
