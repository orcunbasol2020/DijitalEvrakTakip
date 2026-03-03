using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode;

public sealed record GetIncomingDocumentByqrCodeQuery(string qrCode) : IRequest<IncomingDocument>;
