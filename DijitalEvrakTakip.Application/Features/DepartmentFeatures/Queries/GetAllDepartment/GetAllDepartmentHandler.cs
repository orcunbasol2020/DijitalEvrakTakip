using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DepartmentFeatures.Queries.GetAllDepartment;

public sealed class GetAllDepartmentHandler
    : IRequestHandler<GetAllDepartmentQuery, IList<DepartmentDto>>
{
    private readonly IDepartmentService _departmentService;

    public GetAllDepartmentHandler(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IList<DepartmentDto>> Handle(
        GetAllDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);

        return departments
            .Where(x => !x.IsDeleted)
            .Select(x => new DepartmentDto(
                x.Id,
                x.Name
            ))
            .ToList();
    }
}
