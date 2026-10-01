using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class SeedLanguages : Migration
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);

        private static readonly (Guid Id, string Name)[] Languages = new (Guid, string)[]
        {
            (Guid.Parse("1bbdb372-1f09-41be-b4e4-91ecdecf6f8c"), "Türkçe"),
            (Guid.Parse("f3f73db6-139d-431c-ad9c-2d597cb1a47a"), "İngilizce"),
            (Guid.Parse("367cd4e8-a233-4eb0-8977-a73a0b23e20d"), "Almanca"),
            (Guid.Parse("9a20e711-5744-438a-aa92-f745d8f7d49f"), "Fransızca"),
            (Guid.Parse("b833b7b3-2496-4d7c-86a4-a1783deacaf1"), "İspanyolca"),
            (Guid.Parse("12c20e9d-ff0c-4465-afe3-00557da2c08f"), "İtalyanca"),
            (Guid.Parse("5d493562-e612-428a-a6e5-1aed2448b58f"), "Portekizce"),
            (Guid.Parse("4a2e3d12-3a26-486a-9a6d-fa6352ee49c9"), "Rusça"),
            (Guid.Parse("c8d76ddc-47b1-42b0-9846-80ee1ece5aff"), "Arapça"),
            (Guid.Parse("f633011b-ac37-4c8e-90c6-7c9defbc6b23"), "Farsça"),
            (Guid.Parse("8262c297-10c8-4607-b869-b47b89f6e3a0"), "Urduca"),
            (Guid.Parse("cba07189-10e9-4153-a7f4-64404ed63351"), "Hintçe"),
            (Guid.Parse("903be7da-6bba-4d77-99b7-aaa0cd5d0352"), "Bengalce"),
            (Guid.Parse("90735175-f6aa-4333-ba14-e39db096ea49"), "Çince (Basitleştirilmiş)"),
            (Guid.Parse("f3f490a6-33fb-4815-b857-61bbd84d36bb"), "Çince (Geleneksel)"),
            (Guid.Parse("a65fe02d-3cce-43d2-be16-fb1d2e74c042"), "Japonca"),
            (Guid.Parse("5810ccc3-c275-4d97-b3ea-66c45735630e"), "Korece"),
            (Guid.Parse("fc2bccf7-e69d-4e4c-baa4-c52d37606d83"), "Vietnamca"),
            (Guid.Parse("1e2d57c6-ed8e-42c5-914c-8750014f44c2"), "Tayca"),
            (Guid.Parse("88c1a1e4-68c9-4d4b-bccc-7b8bc401c649"), "Endonezce"),
            (Guid.Parse("86c326cd-cbf9-438a-8768-8f983505f8ef"), "Malayca"),
            (Guid.Parse("694a1660-89bf-4921-8750-305740d4907f"), "Filipince (Tagalog)"),
            (Guid.Parse("d969e0dc-a604-4988-941b-0003e933b5f1"), "Hollandaca"),
            (Guid.Parse("4353a801-9d58-4379-b899-422615f4ac31"), "İsveççe"),
            (Guid.Parse("e75ab70c-e82b-4986-851b-cf95e5b926eb"), "Norveççe"),
            (Guid.Parse("af7580ef-5d38-4b02-93ee-6f8d2e9d1132"), "Danca"),
            (Guid.Parse("a67daa03-f82a-4ae3-b10c-25c1415e79d8"), "Fince"),
            (Guid.Parse("64ba0189-7e36-4242-a376-93d556bcb22c"), "Yunanca"),
            (Guid.Parse("9a686a82-37ab-4a2f-9c32-08de4b8e2029"), "Lehçe"),
            (Guid.Parse("677e75d7-3c73-4946-b9d5-e50b00e2721f"), "Çekçe"),
            (Guid.Parse("6deb0e08-8c58-4b79-a3eb-ed04c1ec8a36"), "Slovakça"),
            (Guid.Parse("c3371524-6265-4165-bce3-1b6a9ea424bd"), "Macarca"),
            (Guid.Parse("e19c73e7-d99c-4c7c-ad51-039e65373e78"), "Romence"),
            (Guid.Parse("6919af49-ca1a-49c4-a2eb-0ab464eb1771"), "Bulgarca"),
            (Guid.Parse("ad223200-7ff2-4e28-8bab-b29908df965c"), "Sırpça"),
            (Guid.Parse("b60bd330-e094-412f-998d-ca11e278ad7c"), "Hırvatça"),
            (Guid.Parse("bbe80494-3279-4bb2-a6d1-70d08ed818cd"), "Ukraynaca"),
            (Guid.Parse("0026aa12-9a81-4dfd-9b43-58ddbcefc93d"), "Belarusça"),
            (Guid.Parse("c2968c2a-7cc7-49f6-a4d2-70b9dd75cf83"), "Litvanca"),
            (Guid.Parse("032740e3-b16d-439d-a90e-15596122c34f"), "Letonca"),
            (Guid.Parse("caa8b825-c8c8-463e-ae51-776a1d3b8802"), "Estonca"),
            (Guid.Parse("21f7e5e5-6d4a-469c-a6c1-3f918a38b236"), "İbranice"),
            (Guid.Parse("1e47d72c-b48c-494c-a44d-299473dbd0ea"), "Ermenice"),
            (Guid.Parse("bd854293-de73-4af1-9713-958e77826ffc"), "Gürcüce"),
            (Guid.Parse("9fce7866-bb2d-4193-b0a5-d955253ee47c"), "Azerice"),
            (Guid.Parse("c3202f18-5e7d-48a4-bc5b-d927122f0a80"), "Kazakça"),
            (Guid.Parse("2077648e-8e3b-4080-b1d6-d87b94fe0b8a"), "Özbekçe"),
            (Guid.Parse("8eab4575-65e5-4c3d-a753-113b3986de33"), "Peştuca"),
            (Guid.Parse("dc985659-98fd-4f31-9c5c-3f2673af78e0"), "Swahili"),
            (Guid.Parse("b4ed305c-0495-4ca9-943c-74ac32d66a64"), "Nepalce"),
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var values = new object[Languages.Length, 7];
            for (int i = 0; i < Languages.Length; i++)
            {
                values[i, 0] = Languages[i].Id;
                values[i, 1] = Languages[i].Name;
                values[i, 2] = true;
                values[i, 3] = true;
                values[i, 4] = false;
                values[i, 5] = SeedDate;
                values[i, 6] = null;
            }

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name", "OcrSupport", "IsActive", "IsDeleted", "CreatedDate", "UpdateDate" },
                values: values);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var keys = new object[Languages.Length];
            for (int i = 0; i < Languages.Length; i++)
                keys[i] = Languages[i].Id;

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValues: keys);
        }
    }
}
