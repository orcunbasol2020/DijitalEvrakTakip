using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DepartmentFeatures.Queries.GetAllDepartment;

/// <param name="IncludeMissions">
/// false (varsayılan): yalnızca merkez birimler. true: yurt dışı temsilciliklerimiz de (büyükelçilik,
/// başkonsolosluk, daimi temsilcilik ...).
/// </param>
public sealed record GetAllDepartmentQuery(bool IncludeMissions = false)
    : IRequest<IList<DepartmentDto>>;
