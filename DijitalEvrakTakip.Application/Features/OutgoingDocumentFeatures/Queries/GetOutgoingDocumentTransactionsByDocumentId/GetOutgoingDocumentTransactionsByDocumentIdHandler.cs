using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentTransactionsByDocumentId;

public sealed class GetOutgoingDocumentTransactionsByDocumentIdHandler
    : IRequestHandler<GetOutgoingDocumentTransactionsByDocumentIdQuery, IList<OutgoingDocumentTransactionDto>>
{
    private readonly IOutgoingDocumentTransactionRepository _repository;

    public GetOutgoingDocumentTransactionsByDocumentIdHandler(IOutgoingDocumentTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<OutgoingDocumentTransactionDto>> Handle(
        GetOutgoingDocumentTransactionsByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _repository
            .Where(x => x.OutgoingDocumentId == request.OutgoingDocumentId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return transactions
            .Select(x => new OutgoingDocumentTransactionDto
            {
                Id = x.Id,
                OutgoingDocumentId = x.OutgoingDocumentId,
                Type = x.Type,
                TypeName = x.Type.HasValue && Enum.IsDefined(typeof(OutgoingTransactionTypeEnum), x.Type.Value)
                    ? ((OutgoingTransactionTypeEnum)x.Type.Value).GetDescription()
                    : "-",
                CargoPostNumber = x.CargoPostNumber,
                UserId = x.UserId,
                CreatedDate = x.CreatedDate,
                UpdateDate = x.UpdateDate
            })
            .ToList();
    }
}
