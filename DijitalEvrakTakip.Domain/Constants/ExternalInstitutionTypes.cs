namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// Dış kurum türleri (ExternalInstitution.Type). vw_ExternalInstitutions view'ında ComAgency.DataSourceId'den
/// türetilir: 1 = DETSİS (Türk kamu kurumları), 2 = Diplomatik Portal (yabancı misyonlar), 3 = uluslararası kuruluşlar.
/// </summary>
public static class ExternalInstitutionTypes
{
    /// <summary>Türkiye'deki yabancı misyon (DataSourceId 2). Buradan gelen her evrak, türü ne olursa olsun notadır.</summary>
    public const int Mission = 1;

    /// <summary>Türk kamu kurumu (DETSİS, DataSourceId 1)</summary>
    public const int Institution = 2;

    /// <summary>Dilekçe / talep: tek, sabit gönderen (bkz. ExternalInstitutionIds.Petition)</summary>
    public const int Petition = 3;

    /// <summary>Uluslararası kuruluş (DataSourceId 3). Nota sayılmaz.</summary>
    public const int InternationalOrganization = 4;
}

/// <summary>vw_ExternalInstitutions'ta ComAgency'den gelmeyen sabit kayıtlar.</summary>
public static class ExternalInstitutionIds
{
    /// <summary>"Dilekçe / Talep" göndereni; ComAgency'de karşılığı yok, view'a sabit satır olarak eklenir.</summary>
    public const int Petition = -1;
}
