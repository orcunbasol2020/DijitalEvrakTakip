using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Commands.CreateDocumentAssignment;

public sealed class CreateDocumentAssignmentValidator
    : AbstractValidator<CreateDocumentAssignmentCommand>
{
    public CreateDocumentAssignmentValidator()
    {
        RuleFor(p => p.DocumentId)
            .NotEmpty().WithMessage("Doküman Id boş olamaz!"); // Guid.Empty kontrolü yapar


        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı Id boş olamaz!")
            .MaximumLength(50).WithMessage("Kullanıcı Id 50 karakterden fazla olamaz!");
    }
}