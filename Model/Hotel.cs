using System;
using System;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class Hotel : ISerializable
    {
        // Tehnički identifikator (za veze sa drugim modelima)
        public int Id { get; set; }

        // Šifra hotela (jedinstveno)
        public string Code { get; set; }

        // Ime hotela
        public string Name { get; set; }

        // Godina izgradnje
        public int ConstructionYear { get; set; }

        // Broj zvezdica
        public int Stars { get; set; }

        // JMBG vlasnika kome hotel pripada
        public string OwnerJmbg { get; set; }

        // Prazan konstruktor – potreban za serializer, WPF itd.
        public Hotel()
        {
        }

        // Puni konstruktor (bez Id, Id ćeš dodeljivati u Repository-ju)
        public Hotel(
            string code,
            string name,
            int constructionYear,
            int stars,
            string ownerJmbg)
        {
            Code = code;
            Name = name;
            ConstructionYear = constructionYear;
            Stars = stars;
            OwnerJmbg = ownerJmbg;
        }

        // ---------------- ISerializable ----------------

        public string[] ToCSV()
        {
            // REDOSLED MORA DA SE POKLAPA SA FromCSV I CSV FAJLOM
            string[] csvValues =
            {
                Id.ToString(),
                Code,
                Name,
                ConstructionYear.ToString(),
                Stars.ToString(),
                OwnerJmbg
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            // isti redosled kao u ToCSV
            Id = Convert.ToInt32(values[0]);
            Code = values[1];
            Name = values[2];
            ConstructionYear = Convert.ToInt32(values[3]);
            Stars = Convert.ToInt32(values[4]);
            OwnerJmbg = values[5];
        }
    }
}
