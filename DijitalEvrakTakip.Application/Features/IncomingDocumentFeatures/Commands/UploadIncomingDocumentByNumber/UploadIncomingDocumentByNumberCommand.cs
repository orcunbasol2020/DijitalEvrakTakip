using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UploadIncomingDocumentByNumber;

/// <summary>
/// Kullanıcının yüklediği PDF'i evrak numarasına göre gelen evraka bağlar.
/// Bu numarada evrak yoksa yeni gelen evrak kaydı oluşturulur.
/// </summary>
public sealed record UploadIncomingDocumentByNumberCommand(
    string DocumentNumber,
    string FileName,
    byte[] FileContent,
    string UserId
) : IRequest<MessageResponse>;
