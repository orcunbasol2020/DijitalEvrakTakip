using DijitalEvrakTakip.Application.Services;
using System.Globalization;
using System.Reflection;

namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// Versiyonu ve build tarihini giriş assembly'sinden (WebApi) okur.
/// Versiyon csproj'daki &lt;Version&gt;, build tarihi ise csproj'daki
/// "BuildDate" AssemblyMetadata özniteliğinden gelir.
/// </summary>
public sealed class AssemblyAppVersionProvider : IAppVersionProvider
{
    private const string BuildDateMetadataKey = "BuildDate";

    public string Version { get; }

    public DateTime? BuildDate { get; }

    public AssemblyAppVersionProvider()
    {
        var assembly = Assembly.GetEntryAssembly() ?? typeof(AssemblyAppVersionProvider).Assembly;

        Version = ResolveVersion(assembly);
        BuildDate = ResolveBuildDate(assembly);
    }

    private static string ResolveVersion(Assembly assembly)
    {
        // InformationalVersion "1.2.3+<git-sha>" biçiminde olabilir; sha kısmını atıyoruz.
        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            var plusIndex = informational.IndexOf('+');
            return plusIndex > 0 ? informational[..plusIndex] : informational;
        }

        return assembly.GetName().Version?.ToString(3) ?? "0.0.0";
    }

    private static DateTime? ResolveBuildDate(Assembly assembly)
    {
        var raw = assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => x.Key == BuildDateMetadataKey)?
            .Value;

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        return DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
            out var parsed)
            ? parsed
            : null;
    }
}
