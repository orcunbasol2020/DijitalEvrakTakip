namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// Dış kurum türleri (ExternalInstitution.Type). Değerler ön yüzün dış kurum türü listesinden gelir;
/// orada değişirse burası da değişmeli.
/// </summary>
public static class ExternalInstitutionTypes
{
    // Yabancı misyon: buradan gelen her evrak, türü ne olursa olsun notadır
    public const int Mission = 1;
    public const int Institution = 2;
    public const int Person = 3;
}
