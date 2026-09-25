using DijitalEvrakTakip.Application.Features.UserLoginLogFeatures.Queries.GetUserLoginLogs;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class UserLoginLogService : IUserLoginLogService
{
    private const int MaxPageSize = 200;

    private readonly IUserLoginLogRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserLoginLogService(IUserLoginLogRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(
        Guid? userId,
        string userName,
        bool isSuccess,
        LoginFailureReasonEnum? failureReason,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken)
    {
        UserLoginLog log = new()
        {
            UserId = userId,
            UserName = userName ?? string.Empty,
            IsSuccess = isSuccess,
            FailureReason = failureReason,
            IpAddress = Truncate(ipAddress, 64),
            UserAgent = Truncate(userAgent, 512),
            LoginDate = DateTime.UtcNow
        };

        await _repository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResultDto<UserLoginLogDto>> GetPagedAsync(
        GetUserLoginLogsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<UserLoginLog> query = _repository.GetAll();

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            string term = request.UserName.Trim();
            query = query.Where(x => x.UserName.Contains(term));
        }

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);

        if (request.IsSuccess.HasValue)
            query = query.Where(x => x.IsSuccess == request.IsSuccess.Value);

        if (request.StartDate.HasValue)
            query = query.Where(x => x.LoginDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            // EndDate sadece gün olarak geldiyse o günün tamamını kapsasın.
            DateTime endExclusive = request.EndDate.Value.TimeOfDay == TimeSpan.Zero
                ? request.EndDate.Value.AddDays(1)
                : request.EndDate.Value;
            query = query.Where(x => x.LoginDate < endExclusive);
        }

        IQueryable<UserLoginLogRow> projected = query
            .OrderByDescending(x => x.LoginDate)
            .Select(x => new UserLoginLogRow
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                FullName = x.User != null ? x.User.Name + " " + x.User.Surname : null,
                DepartmentName = x.User != null && x.User.Department != null ? x.User.Department.Name : null,
                IsSuccess = x.IsSuccess,
                FailureReason = x.FailureReason,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                LoginDate = x.LoginDate
            });

        if (request.PageSize is null or <= 0)
        {
            List<UserLoginLogRow> allRows = await projected.ToListAsync(cancellationToken);

            return new PagedResultDto<UserLoginLogDto>
            {
                Items = allRows.Select(ToDto).ToList(),
                TotalCount = allRows.Count,
                Page = 1,
                PageSize = allRows.Count
            };
        }

        int effectivePageSize = Math.Min(request.PageSize.Value, MaxPageSize);
        int effectivePage = request.Page is null or <= 0 ? 1 : request.Page.Value;

        int totalCount = await projected.CountAsync(cancellationToken);

        List<UserLoginLogRow> rows = await projected
            .Skip((effectivePage - 1) * effectivePageSize)
            .Take(effectivePageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<UserLoginLogDto>
        {
            Items = rows.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = effectivePage,
            PageSize = effectivePageSize
        };
    }

    // Enum açıklaması (Description attribute) SQL'e çevrilemediği için projeksiyon
    // önce ham enum ile yapılır, DTO'ya dönüşüm bellekte tamamlanır.
    private static UserLoginLogDto ToDto(UserLoginLogRow row) => new()
    {
        Id = row.Id,
        UserId = row.UserId,
        UserName = row.UserName,
        FullName = row.FullName,
        DepartmentName = row.DepartmentName,
        IsSuccess = row.IsSuccess,
        FailureReason = row.FailureReason?.ToString(),
        FailureReasonText = row.FailureReason?.GetDescription(),
        IpAddress = row.IpAddress,
        UserAgent = row.UserAgent,
        LoginDate = row.LoginDate
    };

    private sealed class UserLoginLogRow
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsSuccess { get; set; }
        public LoginFailureReasonEnum? FailureReason { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime LoginDate { get; set; }
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
