using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Eyp;

/// <summary>EYP dağıtım listesine yazılan, evrakı alan kurum (Bakanlık).</summary>
public sealed record EypRecipient(string Kkk, string Name);

/// <summary>
/// Gelen evrakı EYP üstverisine çevirir. Gelen evrakta paketi oluşturan gönderen dış kurumdur,
/// dağıtımda evrakı alan kurum (Bakanlık) bulunur.
/// Atlas ekibiyle netleşmesi beklenen eşlemeler: BelgeNo, Kişiye Özel gizlilik derecesi,
/// İvedi Süreli ivedilik karşılığı ve DETSİS kodu olmayan gönderenler (yabancı misyonlar, dilekçeler).
/// </summary>
public static class IncomingDocumentEypMapper
{
    public const string UstYaziFileName = "ustyazi.pdf";
    public const string PdfMimeType = "application/pdf";

    /// <summary>
    /// EYP üretmek için eksik olan bilgileri döner; liste boşsa evrak aktarılabilir.
    /// Gönderen kurumun DETSİS kodu aranmaz: yabancı misyonların ve dilekçelerin DETSİS kodu yoktur,
    /// bu gönderenlerin EYP'de nasıl yazılacağı Atlas ekibiyle netleşene kadar kod boş bırakılır.
    /// </summary>
    public static IReadOnlyList<string> Validate(IncomingDocument document)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(document.QrCode))
            errors.Add("Evrak numarası (QR kod) yok.");

        if (string.IsNullOrWhiteSpace(document.Subject))
            errors.Add("Konu girilmemiş.");

        if (document.DocumentDate is null)
            errors.Add("Evrak tarihi girilmemiş.");

        if (document.SecurityDegree is null)
            errors.Add("Gizlilik derecesi seçilmemiş.");
        else if (ToGuvenlikKodu(document.SecurityDegree) is null)
            errors.Add($"'{DescribeSecurityDegree(document.SecurityDegree.Value)}' gizlilik derecesinin EYP karşılığı belirlenmedi.");

        if (document.ExternalInstitutionId is null)
            errors.Add("Gönderen kurum seçilmemiş.");

        return errors;
    }

    /// <summary>
    /// Evrakın EYP üstverisini oluşturur. <see cref="IncomingDocument.ExternalInstitution"/> yüklü olmalıdır.
    /// Eksik bilgi varsa <see cref="InvalidOperationException"/> fırlatır.
    /// </summary>
    public static EypMetadataDto Map(IncomingDocument document, EypRecipient recipient)
    {
        var errors = Validate(document).ToList();
        if (document.ExternalInstitutionId is not null && document.ExternalInstitution is null)
            errors.Add("Gönderen kurum bulunamadı.");
        if (string.IsNullOrWhiteSpace(recipient.Kkk) || string.IsNullOrWhiteSpace(recipient.Name))
            errors.Add("Evrakı alan kurumun KKK kodu veya adı ayarlarda tanımlı değil.");

        if (errors.Count > 0)
            throw new InvalidOperationException(string.Join(" ", errors));

        var institution = document.ExternalInstitution!;

        var metadata = new EypMetadataDto
        {
            DocumentId = document.Id.ToString(),
            Title = document.Subject!.Trim(),
            GizlilikRumuz = ToGuvenlikKodu(document.SecurityDegree),
            DocumentDate = document.DocumentDate,
            // Gelen evrakta belge numarası gönderen kurumun sayısıdır; girilmemişse Atlas numarası yazılır
            BelgeNo = string.IsNullOrWhiteSpace(document.OrginalNo) ? document.QrCode.Trim() : document.OrginalNo.Trim(),
            MimeType = PdfMimeType,
            FileName = UstYaziFileName,
            // DETSİS kodu olmayan gönderende (yabancı misyon, dilekçe) boş kalır; bkz. Validate
            OlusturanKkk = string.IsNullOrWhiteSpace(institution.DetsisCode) ? null : institution.DetsisCode.Trim(),
            Sender = institution.Name,
            OlusturanAdres = string.IsNullOrWhiteSpace(institution.Address) ? null : institution.Address.Trim()
        };

        metadata.DagitimListesi.Add(new DagitimDto
        {
            KKK = recipient.Kkk.Trim(),
            KurumAdi = recipient.Name.Trim(),
            // Bilgi/Gereği girilmemişse Gereği
            BilgiGeregiRumuz = document.ActionRequired == false ? "BLG" : "GRG",
            IvedilikRumuz = ToIvedilikKodu(document.UrgencyDegree)
        });

        // Ek dosyası sistemde tutulmuyor; ek fiziki olarak geldiği için FZK türünde, içeriksiz tanımlanır
        if (document.HasAttachment == true)
        {
            var description = string.IsNullOrWhiteSpace(document.AttachmentDescription)
                ? null
                : document.AttachmentDescription.Trim();

            metadata.EkListesi.Add(new EkDto
            {
                Tur = "FZK",
                SiraNo = 1,
                Ad = description ?? "Fiziki ek",
                Aciklama = description
            });
        }

        return metadata;
    }

    /// <summary>
    /// SecurityDegreeEnum → EYP güvenlik kodu. Kişiye Özel'in EYP karşılığı belirlenmediği için null döner.
    /// </summary>
    public static string? ToGuvenlikKodu(int? securityDegree) => (SecurityDegreeEnum?)securityDegree switch
    {
        SecurityDegreeEnum.Unclassified => "YOK",
        SecurityDegreeEnum.Special => "OZL",
        SecurityDegreeEnum.ServiceUseOnly => "HZO",
        SecurityDegreeEnum.Confidential => "GZL",
        SecurityDegreeEnum.TopSecret => "CGZ",
        _ => null
    };

    /// <summary>UrgencyDegreeEnum → EYP ivedilik kodu. Girilmemişse Normal.</summary>
    public static string ToIvedilikKodu(int? urgencyDegree) => (UrgencyDegreeEnum?)urgencyDegree switch
    {
        UrgencyDegreeEnum.Urgent => "IVD",
        UrgencyDegreeEnum.VeryUrgent => "CIV",
        UrgencyDegreeEnum.Lightning => "YLDRM",
        UrgencyDegreeEnum.Dated => "GNL",
        // İvedi Süreli'nin ayrı bir EYP kodu yok; İvedi olarak gönderilir
        UrgencyDegreeEnum.UrgentTimeLimited => "IVD",
        _ => "NRM"
    };

    private static string DescribeSecurityDegree(int value) =>
        Enum.IsDefined(typeof(SecurityDegreeEnum), value)
            ? ((SecurityDegreeEnum)value).GetDescription()
            : value.ToString();
}
