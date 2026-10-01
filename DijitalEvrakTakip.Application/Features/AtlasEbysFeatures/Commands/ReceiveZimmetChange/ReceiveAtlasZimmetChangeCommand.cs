using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Commands.ReceiveZimmetChange;

/// <summary>
/// Atlas EBYS sisteminin zimmet değişikliği bildirimi (webhook) için gönderdiği kabul edilen istek.
/// </summary>
public sealed record ReceiveAtlasZimmetChangeCommand(
    string AtlasZimmetId,
    string AtlasDocumentNo,
    string? QrCode,
    string? FromUserSicilNo,
    string? FromUserName,
    string? FromUnitName,
    string? ToUserSicilNo,
    string? ToUserName,
    string? ToUnitName,
    string? ZimmetTuru,
    DateTime ZimmetTarihi,
    string? Description
) : IRequest<MessageResponse>;
