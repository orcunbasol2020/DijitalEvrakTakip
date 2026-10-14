using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;

/// <param name="Type">Bkz. ExternalInstitutionTypes. Boşsa tüm türler.</param>
/// <param name="Search">Adda geçen metin ya da tam DETSİS numarası.</param>
/// <param name="Take">
/// Kaynakta 160 binden fazla kamu kurumu var: tür boşsa ya da Kurum (2) ise en fazla Take kayıt döner
/// (varsayılan DefaultTake, en fazla MaxTake). Misyon, dilekçe ve uluslararası kuruluş listeleri küçük olduğu
/// için tamamı döner.
/// </param>
public sealed record GetAllExternalInstitutionQuery(
    int? Type = null,
    string? Search = null,
    int? Take = null)
    : IRequest<IList<ExternalInstitutionDto>>
{
    public const int DefaultTake = 50;
    public const int MaxTake = 200;
}
