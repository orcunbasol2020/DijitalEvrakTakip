using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveByDocumentId;

public sealed class GetActiveDocumentAllocationByDocumentIdHandler
    : IRequestHandler<GetActiveDocumentAllocationByDocumentIdQuery, DocumentAllocationDto>
{
    private readonly IDocumentAllocationService _documentAllocationService;

    public GetActiveDocumentAllocationByDocumentIdHandler(
        IDocumentAllocationService documentAllocationService)
    {
        _documentAllocationService = documentAllocationService;
    }

    public async Task<DocumentAllocationDto> Handle(
        GetActiveDocumentAllocationByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var allocation = await _documentAllocationService
            .GetActiveByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        // Eğer aktif kayıt yoksa null dönecek
        if (allocation == null) return null;

        return new DocumentAllocationDto
        {
            Id = allocation.Id,
            IncomingDocumentId = allocation.IncomingDocumentId,
            UserId = allocation.UserId.ToString(),
            FullName = allocation.User.Name + " " + allocation.User.Surname, // User bilgilerini ekliyoruz
            Status = allocation.Status,
            Source = allocation.Source,
            IsActive = allocation.IsActive,
            IsDeleted = allocation.IsDeleted,
            CreatedDate = allocation.CreatedDate,
            UpdateDate = allocation.UpdateDate
        };
    }
}