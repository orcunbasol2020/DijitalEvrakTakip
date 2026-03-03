using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using Mapster;
using MediatR;
using System.Reflection.Metadata;

namespace DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Commands.CreateDocumentAssignment;

public sealed class CreateDocumentAssignmentCommandHandler
    : IRequestHandler<CreateDocumentAssignmentCommand, MessageResponse>
{
    private readonly IDocumentAssignmentService _documentAssignmentService;
    private readonly IIncomingDocumentService _incomingDocumentService;

    public CreateDocumentAssignmentCommandHandler(
        IDocumentAssignmentService documentAssignmentService,
        IIncomingDocumentService incomingDocumentService)
    {
        _documentAssignmentService = documentAssignmentService;
        _incomingDocumentService = incomingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateDocumentAssignmentCommand request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Aktif assignment kontrolü
        var activeAssignment = await _documentAssignmentService
            .GetActiveByDocumentIdAsync(request.DocumentId, cancellationToken);

        if (activeAssignment != null)
        {
            // Aynı kullanıcıya atanmışsa engelle
            if (activeAssignment.UserId == request.UserId)
                return new MessageResponse("Bu evrak zaten bu kullanıcıya atanmış");

            // Başka kullanıcıya atanmışsa pasife çek
            activeAssignment.IsActive = false;
            activeAssignment.Lock = false;

            // Senkron Update metodunu kullan
            _documentAssignmentService.Update(activeAssignment);
        }

        // Yeni assignment oluştur
        DocumentAssignment assignment = request.Adapt<DocumentAssignment>();
        assignment.IsActive = true;
        assignment.Lock = true;

        // Senkron Create metodunu kullan
        _documentAssignmentService.Create(assignment);

        // IncomingDocument güncelle (CurrentAssignmentUserId)
        await _incomingDocumentService.SetCurrentAssignmentAsync(
            request.DocumentId,
            Guid.TryParse(request.UserId, out var userGuid) ? userGuid : (Guid?)null
        );

        // 4️⃣ Tek commit / SaveChanges
        await _documentAssignmentService.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Evrak başarıyla atandı");
    }
}