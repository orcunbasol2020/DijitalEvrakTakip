namespace Cbddo.eYazisma.Tipler
{
    // Araclar.cs'teki IsNullOrWhiteSpace eklentisi yalnızca .NET Framework'te derleniyor (#if NETFRAMEWORK);
    // .NET 8 derlemesinde eksik kalınca hazır DLL bu metodu ImzaApiTest.dll'den çağırıyordu.
    internal static class Uyumluluk
    {
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);
    }
}
