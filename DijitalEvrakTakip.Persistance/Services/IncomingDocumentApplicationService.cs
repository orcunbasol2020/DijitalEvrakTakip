using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentApplicationService : IIncomingDocumentApplicationService
{
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IDocumentAllocationRepository _documentAllocationRepository;
    private readonly IDocumentTransactionRepository _documentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncomingDocumentApplicationService(
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentAllocationRepository documentAllocationRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentAllocationRepository = documentAllocationRepository;
        _documentTransactionRepository = documentTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> PreRegisterAsync(
        PreRegisterIncomingDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var isExist = await _incomingDocumentRepository
            .AnyAsync(x => x.QrCode == request.QrCode && !x.IsDeleted, cancellationToken);

        if (isExist)
            return false;

        var document = new IncomingDocument
        {
            QrCode = request.QrCode,
            UserId = request.UserId,
            SubmissionStatus = 1,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        await _incomingDocumentRepository.AddAsync(document, cancellationToken);

        var allocation = new DocumentAllocation
        {
            DocumentId = document.Id,
            UserId = request.UserId,
            Status = 1,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _documentAllocationRepository.AddAsync(allocation, cancellationToken);

        var transaction = new DocumentTransaction
        {
            DocumentId = document.Id,
            TransactionType = 1,
            UserId = request.UserId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }



}
