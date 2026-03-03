namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record DocumentAssignmentDto(
    Guid Id,
    Guid DocumentId,
    string? UserId,
    bool? Lock,
    bool? IsActive,
    DateTime CreatedDate
);