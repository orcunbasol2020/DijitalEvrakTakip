using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;

// Alıcı zimmeti kabul eder: Teslim talebi Teslim Alındı, Devir talebi Devir Alındı olarak üzerine geçer.
// HasDiscrepancy ile şerh koyarak kabul eder (ör. sayfa / ek eksik); bu durumda Note zorunludur
public sealed record ApproveAllocationRequestCommand(
    Guid RequestId,
    string UserId,
    bool HasDiscrepancy = false,
    string? Note = null
): IRequest<MessageResponse>;
