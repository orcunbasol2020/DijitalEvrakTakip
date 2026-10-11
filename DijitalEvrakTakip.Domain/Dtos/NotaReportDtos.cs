namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record NotaSenderDto(
    int ExternalInstitutionId,
    string Name,
    // ExternalInstitution.Type
    int Type,
    int NotaCount,
    // Son notanın sisteme kayıt zamanı (CreatedDate), dateField'tan bağımsız
    DateTime LastNotaDate
);

public sealed record NotaSummaryDto(
    int ExternalInstitutionId,
    string Name,
    int NotaCount,
    int ENotaCount,
    // Silinmemiş alıcısı olmayan notalar; departments'a girmez
    int NoRecipientCount,
    DateTime? LastNotaDate,
    IList<NotaDepartmentCountDto> Departments,
    IList<NotaMonthCountDto> Months,
    // Dönem süzgecinden bağımsız, misyonun nota gönderdiği tüm yıllar
    IList<NotaYearCountDto> Years
);

public sealed record NotaDepartmentCountDto(
    int DepartmentId,
    string? DepartmentName,
    string? DepartmentShortName,
    int NotaCount,
    int GeregiCount,
    int BilgiCount
);

public sealed record NotaMonthCountDto(int Year, int Month, int NotaCount);

public sealed record NotaYearCountDto(int Year, int NotaCount);

public sealed record NotaListItemDto(
    Guid Id,
    string QrCode,
    string? OrginalNo,
    string? Subject,
    int? DocumentTypeId,
    DateTime CreatedDate,
    DateTime? DocumentDate,
    IList<NotaRecipientDto> Recipients
);

public sealed record NotaRecipientDto(
    int DepartmentId,
    string? DepartmentName,
    string? DepartmentShortName,
    bool? ActionRequired
);
