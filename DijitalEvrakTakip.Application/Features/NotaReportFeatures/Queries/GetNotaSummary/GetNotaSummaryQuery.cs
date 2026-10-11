using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSummary;

public sealed record GetNotaSummaryQuery(
    int? ExternalInstitutionId,
    int? Year,
    int? Month,
    string? DateField
) : IRequest<NotaSummaryDto>;
