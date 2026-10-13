/*
  ComTitle geçici doldurma (2026-10-13).
  Kaynak ComTitle şeması gelene kadar unvan adları DISNET dökümünden (btgm.txt) çıkarıldı: DISNET'ten birimi atanmış
  108 kişinin (UserDepartmentAssignments) satır sonundaki unvan metni, aynı kişinin ComAuthorizationUser.TitleId'siyle
  eşleştirildi, her TitleId için çoğunluk metni alındı. ", Birim Sorumlusu" görev olduğu için atıldı.
  Kapsam: aktif kullanıcılardaki 229 TitleId'nin 35'i. Diğer birimlerin dökümleri geldikçe eklenir.
  Kaynak tablo gelince bu satırlar silinip kaynaktan yüklenir; Id'ler aynı olduğu için kullanıcı eşlemesi bozulmaz.
  Tekrar çalıştırılabilir (MERGE); yalnızca bu listedeki Id'lere dokunur.
*/
IF DB_NAME() = N'EGovernmentCmsCommon'
BEGIN
    RAISERROR(N'Bu betik kaynak CMS veritabanında çalıştırılamaz.', 16, 1);
    RETURN;
END;

SET NOCOUNT ON;

MERGE ComTitle AS t
USING (VALUES
    -- Id, Ad                                         -- DISNET oyu
    (174849, N'Sözleşmeli Bilişim Personeli'),        -- 23 (1 kişi "Firma Personeli")
    (175280, N'Sözleşmeli Bilişim Personeli'),        -- 6; 174849 ile aynı metin, kaynakta ayrı unvan
    (175230, N'Firma Personeli'),                     -- 15
    (174439, N'Firma Personeli'),                     -- 1; 175230 ile aynı metin
    (175300, N'Ataşe (KİM) (Bilişim)'),               -- 10
    (175302, N'Ataşe (KİM) (Teknik)'),                -- 1
    (165130, N'İkinci Katip (KİM)'),                  -- 4
    (175278, N'İkinci Katip (KİM)'),                  -- 2; 165130 ile aynı metin
    (165137, N'Üçüncü Katip (KİM)'),                  -- 3
    (160054, N'Üçüncü Katip (MM)'),                   -- 1
    (165118, N'Başkatip (KİM)'),                      -- 2
    (160007, N'Başkatip (MM)'),                       -- 1
    (174901, N'Bakan Yardımcısı'),                    -- 1
    (160170, N'Genel Müdür Vekili'),                  -- 1
    (174486, N'Daire Başkanı'),                       -- 2
    (160026, N'Daire Başkanı (MM)'),                  -- 1
    (175288, N'Daire Başkanı T.'),                    -- 1; DISNET'te kısaltılmış, açılımı belirsiz
    (165134, N'Şube Müdürü (KİM)'),                   -- 1
    (165135, N'Şube Müdürü (merkezde)'),              -- 1
    (175297, N'Aday Konsolosluk ve İhtisas Memuru (Bilişim)'), -- 3
    (174488, N'Dışişleri Uzman Yardımcısı'),          -- 3
    (175320, N'Coğrafi Bilgi Sistemi Uzman Yardımcısı'), -- 1
    (160243, N'Uzman (Merkez Memuru)'),               -- 3
    (160204, N'Mütercim (Merkez)'),                   -- 1
    (174513, N'Memur'),                               -- 3
    (160233, N'Şef'),                                 -- 2
    (160047, N'Programcı'),                           -- 3
    (160010, N'Bilgisayar İşletmeni'),                -- 4
    (160237, N'Tekniker'),                            -- 1
    (174915, N'Teknisyen (Ş)'),                       -- 1
    (160061, N'Hizmetli'),                            -- 1
    (160232, N'Sürekli İşçi'),                        -- 1
    (174987, N'Sürekli İşçi-I'),                      -- 1
    (174946, N'Sürekli İşçi (Bilgisayar Teknisyeni)'), -- 1
    (174956, N'Sürekli İşçi (Büro Memuru-Sekreter)')  -- 1
) AS s (TitleId, DefaultName)
ON t.TitleId = s.TitleId
WHEN MATCHED AND (t.DefaultName <> s.DefaultName OR t.IsDeleted = 1) THEN
    UPDATE SET DefaultName = s.DefaultName, IsDeleted = 0
WHEN NOT MATCHED THEN
    INSERT (TitleId, DefaultName, DefaultShortName, IsDeleted) VALUES (s.TitleId, s.DefaultName, NULL, 0)
OUTPUT $action AS Islem, inserted.TitleId, inserted.DefaultName;
