namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// EYP (e-Yazışma Paketi) üstveri bilgisi. Paket oluştururken doldurulur, paket ayrıştırılırken
/// okunan değerlerle döner. Alan adları örnek EypServiceAdapter ile uyumlu tutulmuştur.
/// </summary>
public sealed class EypMetadataDto
{
    // Paketin BelgeId'si; GUID değilse adaptör yeni GUID üretir. Bizde IncomingDocument.Id
    public string DocumentId { get; set; } = string.Empty;

    // Ustveri Konu
    public string? Title { get; set; }

    // Ayrıştırmada Title ile aynı değer döner (uyumluluk için)
    public string? Konu { get; set; }

    // Üst yazı dosyası verilmezse adaptör bu metinden PDF üretir
    public string? Icerik { get; set; }

    // Üst yazı MIME türü (ör. application/pdf)
    public string? MimeType { get; set; }

    // Oluştururken kullanılan güvenlik kodu: YOK / HZO / OZL / GZL / CGZ
    public string? GizlilikRumuz { get; set; }

    // Ayrıştırmada okunan güvenlik kodu
    public string? SecurityLevel { get; set; }

    // NihaiUstveri Tarih; paket tamamlanırken zorunlu
    public DateTime? DocumentDate { get; set; }

    // NihaiUstveri BelgeNo; paket tamamlanırken zorunlu
    public string? BelgeNo { get; set; }

    // Üst yazının paket içindeki dosya adı
    public string? FileName { get; set; }

    public long? FileSize { get; set; }

    // Oluşturan kurum: KKK (DETSİS kodu) ve adı
    public string? OlusturanKkk { get; set; }
    public string? Sender { get; set; }

    // Oluşturan kurumun iletişim bilgileri; en az biri doluysa pakete eklenir
    public string? OlusturanAdres { get; set; }
    public string? OlusturanEPosta { get; set; }
    public string? OlusturanFaks { get; set; }
    public string? OlusturanTelefon { get; set; }
    public string? OlusturanWebAdresi { get; set; }

    // Ayrıştırmada ilk dağıtım kurumunun adı
    public string? Receiver { get; set; }

    // Ayrıştırmada ilk imzacının adı ve görevi
    public string? ImzalayanAdi { get; set; }
    public string? ImzalayanUnvan { get; set; }

    // Oluştururken en az bir dağıtım zorunludur
    public List<DagitimDto> DagitimListesi { get; set; } = new();

    public List<IlgiDto> IlgiListesi { get; set; } = new();

    public List<EkDto> EkListesi { get; set; } = new();

    // Ek alanlar; "SDP_Code" ve "SDP_Name" verilirse pakete SDP bilgisi eklenir
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// EYP dağıtım listesindeki bir kurum.
/// </summary>
public sealed class DagitimDto
{
    // Kurum KKK (DETSİS) kodu
    public string KKK { get; set; } = string.Empty;

    public string KurumAdi { get; set; } = string.Empty;

    // Oluştururken: GRG = Gereği, BLG = Bilgi
    public string? BilgiGeregiRumuz { get; set; }

    // Oluştururken: NRM = Normal, IVD = İvedi, CIV = Çok İvedi, YLDRM = Yıldırım, GNL = Günlü
    public string? IvedilikRumuz { get; set; }

    // Ayrıştırmada okunan dağıtım türü ve ivedilik
    public string? DagitimTuru { get; set; }
    public string? Ivedilik { get; set; }
}

/// <summary>
/// EYP'de evrakın ilgi tuttuğu başka bir belge.
/// </summary>
public sealed class IlgiDto
{
    // Boş bırakılırsa adaptör yeni GUID üretir
    public string? Id { get; set; }

    public string? BelgeNo { get; set; }

    public DateTime? Tarih { get; set; }

    // Ayrıştırmada okunan ilgi etiketi (ör. "a")
    public string? Etiket { get; set; }

    // Oluştururken ilginin etiketi olarak yazılır
    public string? IlgiAcikMesajId { get; set; }

    public string? Ad { get; set; }

    public string? Aciklama { get; set; }
}

/// <summary>
/// EYP eki. Dosya içeriği verilirse pakete gömülür; verilmezse (ör. fiziki ek) yalnızca üstveride tanımlanır.
/// </summary>
public sealed class EkDto
{
    // Boş bırakılırsa adaptör yeni GUID üretir
    public string? Id { get; set; }

    public string? BelgeNo { get; set; }

    // DED = Elektronik dosya, HRF = Harici referans, FZK = Fiziksel nesne
    public string? Tur { get; set; }

    public string? DosyaAdi { get; set; }

    public string? MimeTuru { get; set; }

    public string? Ad { get; set; }

    public int SiraNo { get; set; }

    public string? Aciklama { get; set; }

    public bool ImzaliMi { get; set; }

    // Pakete gömülecek dosya içeriği; ayrıştırmada doldurulmaz
    public byte[]? DosyaIcerigi { get; set; }
}
