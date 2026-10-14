using DijitalEvrakTakip.Domain.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

/// <param name="UserName">AD kullanıcı adı; "HITIT\kullanici" biçimi de kabul edilir.</param>
/// <param name="Password">Ortak geliştirme şifresi (LoginSettings.FormLoginPassword).</param>
public sealed record GetUserByUsernameQuery(
    string UserName,
    string Password
    ) : IRequest<UserLoginDto>
{
    /// <summary>İstemciden değil, controller tarafından ayarlardan (LoginSettings.FormLoginPassword) doldurulur.</summary>
    [JsonIgnore]
    public string ExpectedPassword { get; init; }

    /// <summary>İstemciden değil, controller tarafından HttpContext'ten doldurulur.</summary>
    [JsonIgnore]
    public string IpAddress { get; init; }

    /// <summary>İstemciden değil, controller tarafından HttpContext'ten doldurulur.</summary>
    [JsonIgnore]
    public string UserAgent { get; init; }
}
