using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetDocumentTransactionsByDocumentIdHandler
    : IRequestHandler<GetDocumentTransactionsByDocumentIdQuery, IList<DocumentTransactionDto>>
{
    private readonly IDocumentTransactionRepository _repository;
    private readonly IUserRepository _userRepository;

    public GetDocumentTransactionsByDocumentIdHandler(
        IDocumentTransactionRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<IList<DocumentTransactionDto>> Handle(
        GetDocumentTransactionsByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await (
            from t in _repository.Where(x =>
                x.DocumentId == request.DocumentId &&
                x.IsDeleted == false &&
                x.IsActive == true)

                // İşlemi yapan kullanıcı
            join u in _userRepository.GetAll()
                on t.UserId equals u.Id.ToString() into userJoin
            from u in userJoin.DefaultIfEmpty()

                // Kaydı oluşturan kullanıcı
            join cu in _userRepository.GetAll()
                on t.CreatedUserId equals cu.Id.ToString() into createdUserJoin
            from cu in createdUserJoin.DefaultIfEmpty()

            orderby t.CreatedDate descending

            select new DocumentTransactionDto
            {
                Id = t.Id,
                DocumentId = t.DocumentId,

                TransactionTypeName = t.TransactionType.HasValue
                    ? ((TransactionTypeEnum)t.TransactionType.Value).GetDescription()
                    : "-",

                UserFullName = u != null
                    ? u.Name + " " + u.Surname
                    : "-",

                CreatedUserFullName = cu != null
                    ? cu.Name + " " + cu.Surname
                    : "-",

                TransactionType = t.TransactionType,
                IsActive = t.IsActive,
                IsDeleted = t.IsDeleted,
                CreatedDate = t.CreatedDate,
                UpdateDate = t.UpdateDate
            })
            .ToListAsync(cancellationToken);

        return transactions;
    }
}
