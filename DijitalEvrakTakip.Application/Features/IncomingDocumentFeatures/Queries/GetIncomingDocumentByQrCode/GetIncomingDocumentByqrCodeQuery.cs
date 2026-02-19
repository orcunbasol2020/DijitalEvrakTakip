using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentByQrCode
{
    public sealed record GetIncomingDocumentByqrCodeQuery(string qrCode) : IRequest<IncomingDocument>;
}
