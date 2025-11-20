using System;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class Apartment : ISerializable
    {
        // Tehnički identifikator (za veze i internu upotrebu)
        public int Id { get; set; }

        // Ime apartmana (jedinstveno u okviru istog hotela)
        public string Name { get; set; }

        // Opis apartmana
        public string Description { get; set; }

        // Broj soba
        public int RoomCount { get; set; }

        // Maksimalan broj gostiju
        public int MaxGuests { get; set; }

        // Šifra hotela kome apartman pripada
        public string HotelCode { get; set; }

        // Prazan konstruktor – potreban za serializer, WPF itd.
        public Apartment()
        {
        }

        // Puni konstruktor (bez Id, Id ćeš dodeljivati u Repository-ju)
        public Apartment(
            string name,
            string description,
            int roomCount,
            int maxGuests,
            string hotelCode)
        {
            Name = name;
            Description = description;
            RoomCount = roomCount;
            MaxGuests = maxGuests;
            HotelCode = hotelCode;
        }

        // ---------------- ISerializable ----------------

        public string[] ToCSV()
        {
            // REDOSLED MORA DA SE POKLAPA SA FromCSV I CSV FAJLOM
            string[] csvValues =
            {
                Id.ToString(),
                Name,
                Description,
                RoomCount.ToString(),
                MaxGuests.ToString(),
                HotelCode
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            // isti redosled kao u ToCSV
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            Description = values[2];
            RoomCount = Convert.ToInt32(values[3]);
            MaxGuests = Convert.ToInt32(values[4]);
            HotelCode = values[5];
        }
    }
}
