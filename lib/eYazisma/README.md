# Cbddo.eYazisma

e-Yazışma 2.0 paket (EYP) kütüphanesi. `DijitalEvrakTakip.Infrastructure` bu klasördeki
`Cbddo.eYazisma.dll`'i referans eder (`Infrastructure/Eyp/EypService.cs`).

## Repoda olmayanlar

Kütüphanenin kaynağı (`src/`) ve derlenmiş `Cbddo.eYazisma.dll` lisans durumu netleşene kadar repoya
konmaz (.gitignore). Projeyi derlemek için bu klasöre yerelde yerleştirilir:

```
lib/eYazisma/
├── Cbddo.eYazisma.dll        ← yerelde (aşağıdaki gibi derlenir)
├── src/                      ← yerelde: e-Yazışma API kaynağının Tipler/, Xsd.cs, XsdAraclar.cs ve Xsd/ kısmı
└── build/                    ← repoda
```

Kaynak, DataConverter projesindeki `ExternalLibs\eImza` klasörüdür; oradan yalnızca `Tipler/`, `Xsd.cs`,
`XsdAraclar.cs` ve `Xsd/` alınır. İmza kısımları (ImzaApiTest, TÜBİTAK ma3api örnekleri ve kütüphaneleri)
gerekmez: paketi imzalamıyoruz, evrak kayıt birimi dışarıdan gelen evrakı paketler, imza sahtedir ve Atlas
imzayı doğrulamaz. DLL'in tek bağımlılığı System.IO.Packaging NuGet paketidir.

## Derleme

```
dotnet build lib/eYazisma/build/Cbddo.eYazisma.csproj -c Release
copy lib\eYazisma\build\bin\Release\net8.0\Cbddo.eYazisma.dll lib\eYazisma\
```

Ardından `DijitalEvrakTakip.UnitTest` içindeki `EypServiceUnitTest` çalıştırılır; paket yapısını ve özetleri
kütüphaneden bağımsız olarak doğrular.

`build/Uyumluluk.cs`: `Tipler/Araclar.cs`'teki `IsNullOrWhiteSpace` eklentisi yalnızca .NET Framework'te
derleniyor (`#if NETFRAMEWORK`). .NET 8 için aynı eklenti burada tanımlıdır. Eksik kaldığında hazır net8.0
derlemesi bu metodu ImzaApiTest.dll'den çağırıyor ve imza kütüphanelerini yüklemeye çalışıyordu; bu yüzden
hazır DLL değil, bu proje kullanılır.
