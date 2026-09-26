using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DeparmentFeatures.Commands.CreateDepartment
{
    public sealed record CreateDepartmentCommand(
         string Name,
         string ShortName,
         Guid? ParentId = null,
         int? DisnetId = null
        ) : IRequest<MessageResponse>;

}
