using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentByQrCode;

public sealed record GetOutgoingDocumentByQrCodeQuery(string QrCode) : IRequest<OutgoingDocument>;
