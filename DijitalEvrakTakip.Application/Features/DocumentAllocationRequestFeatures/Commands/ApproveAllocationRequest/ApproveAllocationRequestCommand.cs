using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;

// Alıcı zimmeti kabul eder: Teslim talebi Teslim Alındı, Devir talebi Devir Alındı olarak üzerine geçer
public sealed record ApproveAllocationRequestCommand(
    Guid RequestId,
    string UserId
) : IRequest<MessageResponse>;
