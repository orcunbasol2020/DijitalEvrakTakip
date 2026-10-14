using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Eyp;

/// <summary>Kurumun iletişim bilgisi (ComAgency); boş alanlar pakete "-" olarak yazılır.</summary>
public sealed record EypContact(string? Address, string? Phone, string? Fax, string? Email, string? WebAddress);

/// <summary>
/// "Nereye" listesi boşken dağıtım listesine yazılan, evrakı alan kurum (Bakanlık); ayarlardan gelir.
/// </summary>
public sealed record EypRecipient(string Kkk, string Name, EypContact? Contact = null);

/// <summary>
/// Dağıtım listesine yazılan "Nereye" birimi. Kkk birimin aktif kurum kodudur; birimin kaydı kapatılmışsa
/// aktif üst birimin kodu olur, BirimKkk ise her durumda birimin kendi kodudur. Name birimin adıdır.
/// </summary>
public sealed record EypDistribution(string Kkk, string BirimKkk, string Name, bool? ActionRequired, EypContact? Contact = null);

/// <summary>Pakete içeriğiyle (DED) eklenecek elektronik ek; EYP sırasıyla verilir. FileName ekin özgün dosya adıdır.</summary>
public sealed record EypAttachment(string Name, string? Description, string FileName, byte[] Content);

/// <summary>
/// Gelen evrakı EYP üstverisine çevirir. Gelen evrakta paketi oluşturan gönderen dış kurumdur,
/// dağıtımda evrakın "Nereye" birimleri (boşsa evrakı alan kurum, Bakanlık) bulunur.
/// Atlas ekibiyle netleşmesi beklenen eşlemeler: BelgeNo, Kişiye Özel gizlilik derecesi ve
/// İvedi Süreli ivedilik karşılığı.
/// </summary>
public static class IncomingDocumentEypMapper
{
    public const string UstYaziFileName = "ustyazi.pdf";
    public const string PdfMimeType = "application/pdf";

    /// <summary>DETSİS kodu olmayan gönderenlerin (yabancı misyonlar, dilekçeler) KKK'sı.</summary>
    public const string NoDetsisCode = "99999999";

    // Örnek paketlerdeki gibi sıra 1 üst yazıdır, ekler 2'den başlar
    private const int FirstAttachmentOrder = 2;

    /// <summary>
    /// EYP üretmek için eksik olan bilgileri döner; liste boşsa evrak aktarılabilir.
    /// Gönderen kurumun DETSİS kodu aranmaz: kodu olmayan gönderene <see cref="NoDetsisCode"/> yazılır.
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
    /// Dağıtım listesi <paramref name="distributions"/> ("Nereye") doluysa ondan, boşsa
    /// <paramref name="recipient"/>'tan oluşur. Eksik bilgi varsa <see cref="InvalidOperationException"/> fırlatır.
    /// </summary>
    public static EypMetadataDto Map(
        IncomingDocument document,
        EypRecipient recipient,
        IReadOnlyList<EypDistribution>? distributions = null,
        IReadOnlyList<EypAttachment>? attachments = null,
        EypContact? senderContact = null)
    {
        distributions ??= Array.Empty<EypDistribution>();
        attachments ??= Array.Empty<EypAttachment>();

        var errors = Validate(document).ToList();
        if (document.ExternalInstitutionId is not null && document.ExternalInstitution is null)
            errors.Add("Gönderen kurum bulunamadı.");
        if (distributions.Count == 0 && (string.IsNullOrWhiteSpace(recipient.Kkk) || string.IsNullOrWhiteSpace(recipient.Name)))
            errors.Add("Evrakın \"Nereye\" birimi yok ve evrakı alan kurumun KKK kodu veya adı ayarlarda tanımlı değil.");

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
            OlusturanKkk = string.IsNullOrWhiteSpace(institution.DetsisCode) ? NoDetsisCode : institution.DetsisCode.Trim(),
            Sender = institution.Name.Trim(),
            // İletişim bilgisi ComAgency'den; gelmezse (ör. dilekçe) en azından view'daki adres
            OlusturanAdres = Clean(senderContact?.Address) ?? Clean(institution.Address),
            OlusturanTelefon = Clean(senderContact?.Phone),
            OlusturanFaks = Clean(senderContact?.Fax),
            OlusturanEPosta = Clean(senderContact?.Email),
            OlusturanWebAdresi = Clean(senderContact?.WebAddress)
        };

        var ivedilik = ToIvedilikKodu(document.UrgencyDegree);
        if (distributions.Count > 0)
        {
            foreach (var distribution in distributions)
            {
                metadata.DagitimListesi.Add(CreateDagitim(
                    distribution.Kkk, distribution.BirimKkk, distribution.Name, distribution.Contact,
                    // Bilgi/Gereği alıcı bazında; girilmemişse Gereği
                    distribution.ActionRequired == false ? "BLG" : "GRG",
                    ivedilik));
            }
        }
        else
        {
            metadata.DagitimListesi.Add(CreateDagitim(
                recipient.Kkk, null, recipient.Name, recipient.Contact,
                document.ActionRequired == false ? "BLG" : "GRG",
                ivedilik));
        }

        // Elektronik ekler (yüklenen PDF'ler) içerikleriyle DED türünde eklenir
        var siraNo = FirstAttachmentOrder - 1;
        foreach (var attachment in attachments)
        {
            siraNo++;
            metadata.EkListesi.Add(new EkDto
            {
                Tur = "DED",
                SiraNo = siraNo,
                Ad = attachment.Name,
                Aciklama = attachment.Description,
                DosyaAdi = attachment.FileName,
                MimeTuru = PdfMimeType,
                // Örnek paketlerdeki gibi elektronik ekler imzalı bileşen olarak (Ekler/ klasörü) eklenir
                ImzaliMi = true,
                DosyaIcerigi = attachment.Content
            });
        }

        // Fiziki ek dosyası tutulmaz; açıklamasıyla FZK türünde, içeriksiz ve elektronik eklerden sonra tanımlanır.
        // "Ek var" denip açıklama girilmemişse, ek elektronik eklerden ibaret sayılır.
        var physicalDescription = Clean(document.AttachmentDescription);

        if (document.HasAttachment == true && (physicalDescription is not null || attachments.Count == 0))
        {
            metadata.EkListesi.Add(new EkDto
            {
                Tur = "FZK",
                SiraNo = siraNo + 1,
                Ad = physicalDescription ?? "Fiziki ek",
                Aciklama = physicalDescription
            });
        }

        return metadata;
    }

    private static DagitimDto CreateDagitim(
        string kkk, string? birimKkk, string name, EypContact? contact, string bilgiGeregi, string ivedilik) => new()
    {
        KKK = kkk.Trim(),
        BirimKKK = Clean(birimKkk),
        KurumAdi = name.Trim(),
        BilgiGeregiRumuz = bilgiGeregi,
        IvedilikRumuz = ivedilik,
        Adres = Clean(contact?.Address),
        Telefon = Clean(contact?.Phone),
        Faks = Clean(contact?.Fax),
        EPosta = Clean(contact?.Email),
        WebAdresi = Clean(contact?.WebAddress)
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
