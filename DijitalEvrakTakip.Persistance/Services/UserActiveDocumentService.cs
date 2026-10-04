using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class UserActiveDocumentService : IUserActiveDocumentService
{
    private const int MaxPageSize = 200;

    private readonly IDocumentAllocationRepository _incomingAllocationRepository;
    private readonly IOutgoingDocumentAllocationRepository _outgoingAllocationRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IOutgoingDocumentRepository _outgoingDocumentRepository;

    public UserActiveDocumentService(
        IDocumentAllocationRepository incomingAllocationRepository,
        IOutgoingDocumentAllocationRepository outgoingAllocationRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IOutgoingDocumentRepository outgoingDocumentRepository)
    {
        _incomingAllocationRepository = incomingAllocationRepository;
        _outgoingAllocationRepository = outgoingAllocationRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _outgoingDocumentRepository = outgoingDocumentRepository;
    }

    public Task<PagedResultDto<UserActiveDocumentDto>> GetActiveByUserIdAsync(
        Guid userId,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        return GetByUserIdAsync(
            userId, onlyActive: true, excludeSelfAllocations: false,
            documentDirection, page, pageSize, cancellationToken);
    }

    public Task<PagedResultDto<UserActiveDocumentDto>> GetReceivedByUserIdAsync(
        Guid userId,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        // Kullanıcının kendine yaptığı zimmetler (örn. evrak kaydı sırasında) teslim alma sayılmaz.
        return GetByUserIdAsync(
            userId, onlyActive: false, excludeSelfAllocations: true,
            documentDirection, page, pageSize, cancellationToken);
    }

    private async Task<PagedResultDto<UserActiveDocumentDto>> GetByUserIdAsync(
        Guid userId,
        bool onlyActive,
        bool excludeSelfAllocations,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        // Gelen ve giden sorguları aynı DbContext üzerinden geldiği için Concat
        // tek bir SQL (UNION ALL) olarak çalışır; sıralama ve sayfalama da veritabanında yapılır.
        IQueryable<UserActiveDocumentDto> query = documentDirection switch
        {
            (int)DocumentDirectionEnum.Incoming => BuildIncomingQuery(userId, onlyActive, excludeSelfAllocations),
            (int)DocumentDirectionEnum.Outgoing => BuildOutgoingQuery(userId, onlyActive, excludeSelfAllocations),
            _ => BuildIncomingQuery(userId, onlyActive, excludeSelfAllocations)
                .Concat(BuildOutgoingQuery(userId, onlyActive, excludeSelfAllocations))
        };

        query = query.OrderByDescending(x => x.AllocatedDate);

        if (pageSize is null or <= 0)
        {
            var allItems = await query.ToListAsync(cancellationToken);

            return new PagedResultDto<UserActiveDocumentDto>
            {
                Items = allItems,
                TotalCount = allItems.Count,
                Page = 1,
                PageSize = allItems.Count
            };
        }

        int effectivePageSize = Math.Min(pageSize.Value, MaxPageSize);
        int effectivePage = page is null or <= 0 ? 1 : page.Value;

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((effectivePage - 1) * effectivePageSize)
            .Take(effectivePageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<UserActiveDocumentDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = effectivePage,
            PageSize = effectivePageSize
        };
    }

    // Gelen evrak: dış kurumdan (FromName) birime (ToName) gelir.
    private IQueryable<UserActiveDocumentDto> BuildIncomingQuery(Guid userId, bool onlyActive, bool excludeSelfAllocations)
    {
        return from allocation in _incomingAllocationRepository.GetAll()
               where allocation.UserId == userId
                     && (!onlyActive || allocation.IsActive)
                     && (!excludeSelfAllocations || allocation.CreatedUserId != userId)
                     && !allocation.IsDeleted
               join document in _incomingDocumentRepository.GetAll()
                   on allocation.IncomingDocumentId equals document.Id into documents
               from document in documents.DefaultIfEmpty()
               select new UserActiveDocumentDto
               {
                   AllocationId = allocation.Id,
                   DocumentId = allocation.IncomingDocumentId,
                   DocumentDirection = (int)DocumentDirectionEnum.Incoming,
                   QrCode = document!.QrCode,
                   DocumentNo = document.OrginalNo,
                   DocumentName = document.DocumentName,
                   DocumentDate = document.DocumentDate,
                   FromName = document.ExternalInstitution!.Name,
                   ToName = document.Department!.Name,
                   Status = allocation.Status,
                   Source = allocation.Source,
                   IsActive = allocation.IsActive,
                   AllocatedDate = allocation.CreatedDate
               };
    }

    // Giden evrak: birimden (FromName) dış kuruma (ToName) gider.
    // Giden evrakta ayrı bir belge adı alanı olmadığından DocumentName olarak Subject kullanılır.
    private IQueryable<UserActiveDocumentDto> BuildOutgoingQuery(Guid userId, bool onlyActive, bool excludeSelfAllocations)
    {
        return from allocation in _outgoingAllocationRepository.GetAll()
               where allocation.UserId == userId
                     && (!onlyActive || allocation.IsActive)
                     && (!excludeSelfAllocations || allocation.CreatedUserId != userId)
                     && !allocation.IsDeleted
               join document in _outgoingDocumentRepository.GetAll()
                   on allocation.OutgoingDocumentId equals document.Id into documents
               from document in documents.DefaultIfEmpty()
               select new UserActiveDocumentDto
               {
                   AllocationId = allocation.Id,
                   DocumentId = allocation.OutgoingDocumentId,
                   DocumentDirection = (int)DocumentDirectionEnum.Outgoing,
                   QrCode = document!.QrCode,
                   DocumentNo = document.OriginalDocumentNumber,
                   DocumentName = document.Subject,
                   DocumentDate = document.DocumentDate,
                   FromName = document.Department!.Name,
                   ToName = document.ExternalInstitution!.Name,
                   Status = allocation.Status,
                   Source = allocation.Source,
                   IsActive = allocation.IsActive,
                   AllocatedDate = allocation.CreatedDate
               };
    }
}
