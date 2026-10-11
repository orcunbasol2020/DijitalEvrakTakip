using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.WindowsLogin;

/// <param name="IdentityName">
/// Kerberos ile doğrulanmış Windows kimliği; Windows'ta "DOMAIN\kullanici", Linux'ta "kullanici@REALM".
/// İstemciden değil, controller tarafından doğrulanmış kimlikten doldurulur.
/// </param>
public sealed record WindowsLoginQuery(
    string IdentityName,
    string IpAddress,
    string UserAgent
) : IRequest<UserLoginDto>;
