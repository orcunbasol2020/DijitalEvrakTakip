# Dosyası yüklenmemiş evraklar için EYP üst yazısı olarak kullanılacak standart PDF'i üretir.
# A4 dikey: sol üstte Bakanlık logosu, sayfa ortasında belge sembolü ve altında
# "Bu Belge Fiziki Olarak Gönderilecektir." ibaresi.
# Ek kütüphane gerektirmez: logo JPEG'e çevrilip gömülür, yazı PDF'in yerleşik Times-Bold
# (Times New Roman) yazı tipiyle basılır, belge sembolü vektörel çizilir.
# Kullanım (Windows PowerShell 5.1, dosya UTF-8 olduğu için bu şekilde çalıştırılır):
#   & ([scriptblock]::Create((Get-Content -Raw -Encoding UTF8 .\fiziki_gonderim_pdf.ps1))) -LogoPath logo.png -OutputPath fiziki_gonderim.pdf
# Bu klasördeki fiziki_gonderim.pdf gömülü kaynak olarak derlenir; Bakanlık logosu Angular projesindeki
# src/assets/images/mfa_logo.png dosyasıdır. Logo veya metin değişirse PDF bu betikle yeniden üretilir.
param(
    [Parameter(Mandatory = $true)][string]$LogoPath,
    [Parameter(Mandatory = $true)][string]$OutputPath
)

Add-Type -AssemblyName System.Drawing

$pageWidth = 595.28
$pageHeight = 841.89
$topMargin = 45
$leftMargin = 50
$logoWidthPt = 60
$fontSize = 32
$lineGap = 12
$lines = @('Bu Belge Fiziki Olarak', 'Gönderilecektir.')
# Lacivert (RGB 0-1)
$ink = '0.07 0.16 0.35'

# --- Logo: beyaz zemine çizilip JPEG'e çevrilir (saydam PNG'ler de beyaz zeminle basılır) ---
$src = [System.Drawing.Image]::FromFile($LogoPath)
$pixel = 600
# Yalnızca küçültülür; düşük çözünürlüklü logo büyütülmez
$scale = [Math]::Min(1.0, [Math]::Min($pixel / $src.Width, $pixel / $src.Height))
$w = [int]($src.Width * $scale); $h = [int]($src.Height * $scale)
$bmp = New-Object System.Drawing.Bitmap $w, $h
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::White)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.DrawImage($src, 0, 0, $w, $h)
$g.Dispose(); $src.Dispose()

$jpegCodec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
$encParams = New-Object System.Drawing.Imaging.EncoderParameters 1
$encParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter ([System.Drawing.Imaging.Encoder]::Quality), 92L
$jpegStream = New-Object System.IO.MemoryStream
$bmp.Save($jpegStream, $jpegCodec, $encParams)
$bmp.Dispose()
$jpeg = $jpegStream.ToArray()

$logoHeightPt = $logoWidthPt * $h / $w
$logoX = $leftMargin
$logoY = $pageHeight - $topMargin - $logoHeightPt

# --- Yazı genişliği: Times-Bold AFM genişlikleri (1/1000 em) ---
# PowerShell hashtable'ı büyük/küçük harf ayırmadığı için ordinal sözlük kullanılır
$widths = New-Object 'System.Collections.Generic.Dictionary[string,int]' ([StringComparer]::Ordinal)
$afm = @(
    ' ', 250, '.', 250,
    'B', 667, 'F', 611, 'G', 778, 'O', 778,
    'a', 500, 'c', 444, 'd', 556, 'e', 444, 'g', 500, 'i', 278, 'k', 556,
    'l', 278, 'n', 556, 'r', 444, 't', 333, 'u', 556, 'z', 444, 'ö', 500
)
for ($j = 0; $j -lt $afm.Count; $j += 2) { $widths[$afm[$j]] = $afm[$j + 1] }
function Get-TextWidth([string]$text) {
    $sum = 0
    foreach ($ch in $text.ToCharArray()) {
        $key = [string]$ch
        if (-not $widths.ContainsKey($key)) { throw "Genişlik tablosunda olmayan karakter: '$key'" }
        $sum += $widths[$key]
    }
    return $sum * $fontSize / 1000
}

$inv = [System.Globalization.CultureInfo]::InvariantCulture
function F([double]$v) { $v.ToString('0.##', $inv) }

# Bezier ile çeyrek daire yaklaşımı katsayısı
$k = 0.5523

# --- Dikey yerleşim: belge sembolü + boşluk + iki satır yazı, sayfa ortasında bir grup ---
# Simge 60x78 tasarlanıp ölçeklenir
$docScale = 0.55
$docW = 60 * $docScale; $docH = 78 * $docScale; $docFold = 20 * $docScale
$gapIconText = 24
$textHeight = $lines.Count * $fontSize + ($lines.Count - 1) * $lineGap
$groupHeight = $docH + $gapIconText + $textHeight
$groupTop = $pageHeight / 2 + $groupHeight / 2

$cx = $pageWidth / 2
$by = $groupTop - $docH                        # belge sembolünün alt kenarı
$firstBaseline = $by - $gapIconText - $fontSize * 0.7

$content = New-Object System.Text.StringBuilder
function Add([string]$s) { [void]$content.AppendLine($s) }

# Logo
Add 'q'
Add "$(F $logoWidthPt) 0 0 $(F $logoHeightPt) $(F $logoX) $(F $logoY) cm"
Add '/Logo Do'
Add 'Q'

# Belge sembolü: sağ üst köşesi kıvrık sayfa (lacivert dolgu)
$x0 = $cx - $docW / 2; $x1 = $cx + $docW / 2
$y0 = $by; $y1 = $by + $docH
$f = $docFold
Add "q $ink rg"
Add "$(F $x0) $(F $y0) m $(F $x1) $(F $y0) l $(F $x1) $(F ($y1 - $f)) l $(F ($x1 - $f)) $(F $y1) l $(F $x0) $(F $y1) l h f"
Add 'Q'

# Kıvrık köşe: açık mavi üçgen, sayfadan ince beyaz boşlukla ayrılır
Add "q 0.55 0.66 0.84 rg 1 1 1 RG $(F (1.5 * $docScale)) w 1 j"
Add "$(F ($x1 - $f)) $(F $y1) m $(F ($x1 - $f)) $(F ($y1 - $f)) l $(F $x1) $(F ($y1 - $f)) l h B"
Add 'Q'

# Yazı satırları: beyaz, uçları yuvarlak; son satır kısa
$lineX0 = $x0 + 11 * $docScale
$lineFull = $x1 - 11 * $docScale
$rows = @(
    @{ Y = $y1 - 32 * $docScale; X1 = $lineFull },
    @{ Y = $y1 - 43 * $docScale; X1 = $lineFull },
    @{ Y = $y1 - 54 * $docScale; X1 = $lineFull },
    @{ Y = $y1 - 65 * $docScale; X1 = $cx + 4 * $docScale }
)
Add "q 1 1 1 RG $(F (4 * $docScale)) w 1 J"
foreach ($row in $rows) {
    Add "$(F $lineX0) $(F $row.Y) m $(F $row.X1) $(F $row.Y) l S"
}
# Başlık satırı: kıvrık köşenin solunda, kısa
Add "$(F $lineX0) $(F ($y1 - 16 * $docScale)) m $(F ($x1 - $f - 9 * $docScale)) $(F ($y1 - 16 * $docScale)) l S"
Add 'Q'

# İbare
Add "$ink rg"
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    $x = ($pageWidth - (Get-TextWidth $line)) / 2
    $y = $firstBaseline - $i * ($fontSize + $lineGap)
    $escaped = $line.Replace('\', '\\').Replace('(', '\(').Replace(')', '\)')
    Add "BT /F1 $fontSize Tf $(F $x) $(F $y) Td ($escaped) Tj ET"
}

$win1252 = [System.Text.Encoding]::GetEncoding(1252)
$contentBytes = $win1252.GetBytes($content.ToString())

# --- PDF nesneleri ---
$out = New-Object System.IO.MemoryStream
$offsets = New-Object System.Collections.Generic.List[long]
function Write-Ascii([string]$s) { $b = $win1252.GetBytes($s); $out.Write($b, 0, $b.Length) }
function Begin-Object([int]$n) { $offsets.Add($out.Position); Write-Ascii "$n 0 obj`n" }

Write-Ascii "%PDF-1.4`n%"
$out.Write([byte[]](0xE2, 0xE3, 0xCF, 0xD3), 0, 4)
Write-Ascii "`n"

Begin-Object 1; Write-Ascii "<< /Type /Catalog /Pages 2 0 R >>`nendobj`n"
Begin-Object 2; Write-Ascii "<< /Type /Pages /Kids [3 0 R] /Count 1 >>`nendobj`n"
Begin-Object 3
Write-Ascii "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 $(F $pageWidth) $(F $pageHeight)] /Resources << /Font << /F1 4 0 R >> /XObject << /Logo 5 0 R >> >> /Contents 6 0 R >>`nendobj`n"
Begin-Object 4; Write-Ascii "<< /Type /Font /Subtype /Type1 /BaseFont /Times-Bold /Encoding /WinAnsiEncoding >>`nendobj`n"
Begin-Object 5
Write-Ascii "<< /Type /XObject /Subtype /Image /Width $w /Height $h /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length $($jpeg.Length) >>`nstream`n"
$out.Write($jpeg, 0, $jpeg.Length)
Write-Ascii "`nendstream`nendobj`n"
Begin-Object 6
Write-Ascii "<< /Length $($contentBytes.Length) >>`nstream`n"
$out.Write($contentBytes, 0, $contentBytes.Length)
Write-Ascii "`nendstream`nendobj`n"
Begin-Object 7
Write-Ascii "<< /Title (Fiziki Gonderim) /Producer (Dijital Evrak Takip) >>`nendobj`n"

$xref = $out.Position
Write-Ascii "xref`n0 $($offsets.Count + 1)`n0000000000 65535 f `n"
foreach ($o in $offsets) { Write-Ascii ($o.ToString('0000000000') + " 00000 n `n") }
Write-Ascii "trailer`n<< /Size $($offsets.Count + 1) /Root 1 0 R /Info 7 0 R >>`nstartxref`n$xref`n%%EOF`n"

[System.IO.File]::WriteAllBytes($OutputPath, $out.ToArray())
Write-Output "Oluşturuldu: $OutputPath ($($out.Length) bayt)"
