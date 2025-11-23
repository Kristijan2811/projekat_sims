using System;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class Hotel : ISerializable
    {
        // Tehnicki identifikator (za veze sa drugim modelima)
        public int Id { get; set; }

        // Sifra hotela (jedinstveno)
        public string Code { get; set; }

        // Ime hotela
        public string Name { get; set; }

        // Godina izgradnje
        public int ConstructionYear { get; set; }

        // Broj zvezdica
        public int Stars { get; set; }

        // JMBG vlasnika kome hotel pripada
        public string OwnerJmbg { get; set; }

        // NOVO: status hotela (na cekanju / prihvacen / odbijen)
        public HotelStatus Status { get; set; }

        // Prazan konstruktor – potreban za serializer, WPF itd.
        public Hotel()
        {
        }

        // Puni konstruktor (bez Id, Id ces dodeljivati u Repository-ju)
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

            // Podrazumevano novi hotel je na cekanju dok ga vlasnik ne potvrdi
            Status = HotelStatus.Pending;
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
                OwnerJmbg,
                Status.ToString()   // NOVO: sedma kolona
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

            // CSV fajl koji vec imas ima samo 6 kolona -> tada nema statusa
            // da ne pukne deserializacija:
            if (values.Length >= 7)
            {
                Status = Enum.Parse<HotelStatus>(values[6]);
            }
            else
            {
                // stare hotele tretiramo kao Approved (vec postoje u sistemu)
                Status = HotelStatus.Approved;
            }
        }
    }
}
