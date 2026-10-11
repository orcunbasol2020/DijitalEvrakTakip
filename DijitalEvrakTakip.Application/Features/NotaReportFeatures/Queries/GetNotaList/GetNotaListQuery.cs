using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaList;

public sealed record GetNotaListQuery(
    int? ExternalInstitutionId,
    int? DepartmentId,
    int? Year,
    int? Month,
    string? DateField,
    int? Page,
    int? PageSize
) : IRequest<PagedResultDto<NotaListItemDto>>;
