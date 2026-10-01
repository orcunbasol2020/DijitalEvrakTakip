using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserLoginLogFeatures.Queries.GetUserLoginLogs;

/// <summary>
/// Login denemelerini tarihe göre yeniden eskiye, sayfalı ve filtreli listeler.
/// </summary>
/// <param name="UserName">Kullanıcı adında geçen metin (contains, büyük/küçük harf duyarsız); boş ise filtre yok.</param>
/// <param name="UserId">Belirli bir kullanıcının denemeleri; null ise filtre yok.</param>
/// <param name="IsSuccess">true = sadece başarılı, false = sadece başarısız, null = hepsi.</param>
/// <param name="StartDate">Bu tarihten itibaren (dahil); null ise filtre yok.</param>
/// <param name="EndDate">Bu tarihe kadar (dahil); null ise filtre yok.</param>
/// <param name="Page">1 tabanlı sayfa numarası.</param>
/// <param name="PageSize">Sayfa boyutu; null ise tüm kayıtlar döner.</param>
public sealed record GetUserLoginLogsQuery(
    string? UserName,
    Guid? UserId,
    bool? IsSuccess,
    DateTime? StartDate,
    DateTime? EndDate,
    int? Page,
    int? PageSize)
    : IRequest<PagedResultDto<UserLoginLogDto>>;
