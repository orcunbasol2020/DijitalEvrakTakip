using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using Mapster;

namespace DijitalEvrakTakip.Application.Features.DeparmentFeatures.Commands.CreateDepartment;

public sealed class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, MessageResponse>
{
    private readonly IDepartmentService _departmentService;

    public CreateDepartmentCommandHandler(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<MessageResponse> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        Department department = request.Adapt<Department>();

        await _departmentService.CreateAsync(department, cancellationToken);

        return new("Birim Başarıyla Kaydedildi");
    }
}
