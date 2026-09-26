using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DeparmentFeatures.Commands.CreateDepartment;

public sealed class CreateDepartmentValidator
    : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Birim adı boş olamaz!")
            .MinimumLength(3).WithMessage("Birim adı en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Birim adı 200 karakterden fazla olamaz!");

        RuleFor(p => p.ParentId)
            .NotEqual(Guid.Empty).When(p => p.ParentId.HasValue)
            .WithMessage("Üst birim Id geçersiz!");

        RuleFor(p => p.DisnetId)
            .GreaterThan(0).When(p => p.DisnetId.HasValue)
            .WithMessage("Disnet Id 0'dan büyük olmalıdır!");
    }
}
