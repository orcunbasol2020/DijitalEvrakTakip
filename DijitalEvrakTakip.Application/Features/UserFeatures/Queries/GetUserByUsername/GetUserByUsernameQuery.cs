using DijitalEvrakTakip.Domain.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

public sealed record GetUserByUsernameQuery(
    string UserName,
    string Password
    ) : IRequest<UserLoginDto>
{
    /// <summary>İstemciden değil, controller tarafından HttpContext'ten doldurulur.</summary>
    [JsonIgnore]
    public string IpAddress { get; init; }

    /// <summary>İstemciden değil, controller tarafından HttpContext'ten doldurulur.</summary>
    [JsonIgnore]
    public string UserAgent { get; init; }
}
