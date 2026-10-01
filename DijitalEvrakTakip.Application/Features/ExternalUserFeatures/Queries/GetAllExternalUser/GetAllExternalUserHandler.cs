using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser
{
    public sealed class GetAllExternalUserHandler : IRequestHandler<GetAllExternalUserQuery, IList<ExternalUserDto>>
    {
        private readonly IExternalUserService _externalUserService;

        public GetAllExternalUserHandler(IExternalUserService externalUserService)
        {
            _externalUserService = externalUserService;
        }

        public async Task<IList<ExternalUserDto>> Handle(GetAllExternalUserQuery request, CancellationToken cancellationToken)
        {
            var query = _externalUserService
                .GetAll();

            var result = await query
                .ProjectToType<ExternalUserDto>()
                .OrderBy(u => u.Name)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}