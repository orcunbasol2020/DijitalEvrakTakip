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

    public async Task<PagedResultDto<UserActiveDocumentDto>> GetActiveByUserIdAsync(
        Guid userId,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        // Gelen ve giden sorguları aynı DbContext üzerinden geldiği için Concat
        // tek bir SQL (UNION ALL) olarak çalışır; sıralama ve sayfalama da veritabanında yapılır.
        IQueryable<UserActiveDocumentDto> query = documentDirection switch
        {
            (int)DocumentDirectionEnum.Incoming => BuildIncomingQuery(userId),
            (int)DocumentDirectionEnum.Outgoing => BuildOutgoingQuery(userId),
            _ => BuildIncomingQuery(userId).Concat(BuildOutgoingQuery(userId))
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
    private IQueryable<UserActiveDocumentDto> BuildIncomingQuery(Guid userId)
    {
        return from allocation in _incomingAllocationRepository.GetAll()
               where allocation.UserId == userId
                     && allocation.IsActive
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
                   AllocatedDate = allocation.CreatedDate
               };
    }

    // Giden evrak: birimden (FromName) dış kuruma (ToName) gider.
    // Giden evrakta ayrı bir belge adı alanı olmadığından DocumentName olarak Subject kullanılır.
    private IQueryable<UserActiveDocumentDto> BuildOutgoingQuery(Guid userId)
    {
        return from allocation in _outgoingAllocationRepository.GetAll()
               where allocation.UserId == userId
                     && allocation.IsActive
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
                   AllocatedDate = allocation.CreatedDate
               };
    }
}
