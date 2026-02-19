using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DepartmentFeatures.Queries.GetAllDepartment;

public sealed record GetAllDepartmentQuery()
    : IRequest<IList<DepartmentDto>>;
