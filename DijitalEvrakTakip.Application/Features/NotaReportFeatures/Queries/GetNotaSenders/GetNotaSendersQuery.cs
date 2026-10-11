using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSenders;

public sealed record GetNotaSendersQuery(
    int? Year,
    string? DateField
) : IRequest<IList<NotaSenderDto>>;
