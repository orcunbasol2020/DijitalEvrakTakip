namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record UserLoginDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string DepartmentShortName { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public IList<string> Roles { get; set; }
}
