using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetReceivedDocumentsByUserId;

/// <summary>
/// Kullanıcının teslim aldığı tüm gelen ve giden evrak zimmetlerini, aktif olmasa da, tek listede getirir.
/// </summary>
/// <param name="UserId">Zimmeti teslim alan kullanıcı.</param>
/// <param name="DocumentDirection">DocumentDirectionEnum (1 = Gelen, 2 = Giden); null ise ikisi birlikte.</param>
/// <param name="Page">1 tabanlı sayfa numarası.</param>
/// <param name="PageSize">Sayfa boyutu; null ise tüm kayıtlar döner.</param>
public sealed record GetReceivedDocumentsByUserIdQuery(
    Guid UserId,
    int? DocumentDirection,
    int? Page,
    int? PageSize)
    : IRequest<PagedResultDto<UserActiveDocumentDto>>;
