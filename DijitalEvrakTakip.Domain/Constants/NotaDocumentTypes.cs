namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// Nota Raporlarında kullanılan gelen evrak türleri (IncomingDocument.DocumentTypeId). Bir evrağın nota
/// sayılması türüne değil gönderenin misyon olmasına bağlıdır (bkz. ExternalInstitutionTypes.Mission);
/// buradaki değer yalnızca özetteki eNotaCount için kullanılır. DocumentTypeId bir tabloya bağlı değil;
/// değerler ön yüzün evrak türü listesinden gelir, orada değişirse burası da değişmeli.
/// </summary>
public static class NotaDocumentTypes
{
    public const int ENota = 11;
}
