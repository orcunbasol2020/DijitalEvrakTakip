namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// Sorgularda kullanılan SQL Server karşılaştırma kuralları. Veritabanının varsayılanı SQL_Latin1_General_CP1_CI_AS:
/// orada "i" ile "İ", "ı" ile "I" farklı harf sayılır.
/// </summary>
public static class SqlCollations
{
    /// <summary>
    /// Türkçe, büyük / küçük harfe duyarsız: "valiliği" = "VALİLİĞİ", "ılgaz" = "ILGAZ". Ad aramalarında
    /// EF.Functions.Collate(kolon, Turkish).Contains(metin) biçiminde kullanılır.
    /// </summary>
    public const string Turkish = "Turkish_CI_AS";
}
