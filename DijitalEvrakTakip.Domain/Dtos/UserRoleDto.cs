namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class UserRolesDto
{
    public string UserId { get; set; }
    public IList<string> Roles { get; set; }
}
