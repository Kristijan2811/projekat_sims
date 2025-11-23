using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class HotelRepository
    {
        private const string FilePath = "../../../Resources/Data/hotels.csv";

        private readonly Serializer<Hotel> _serializer;
        private List<Hotel> _hotels;

        public HotelRepository()
        {
            _serializer = new Serializer<Hotel>();
            Load();
        }

        private void Load()
        {
            _hotels = _serializer.FromCSV(FilePath);
        }

        // cuvanje trenutnog stanja u CSV
        private void Save()
        {
            _serializer.ToCSV(FilePath, _hotels);
        }

        public List<Hotel> GetAll()
        {
            Load();
            return _hotels;
        }

        // hoteli za datog vlasnika (po JMBG-u)
        public List<Hotel> GetByOwnerJmbg(string ownerJmbg)
        {
            Load();
            return _hotels
                .Where(h => h.OwnerJmbg == ownerJmbg)
                .ToList();
        }

        // NOVO: pronadji hotel po sifri (korisno za proveru jedinstvenosti)
        public Hotel GetByCode(string code)
        {
            Load();
            return _hotels.FirstOrDefault(h => h.Code == code);
        }

        // NOVO: update hotela (npr. promena Status-a)
        public void Update(Hotel hotel)
        {
            Load();
            var index = _hotels.FindIndex(h => h.Id == hotel.Id);
            if (index != -1)
            {
                _hotels[index] = hotel;
                Save();
            }
        }

        // NOVO: dodavanje novog hotela
        public Hotel Add(Hotel hotel)
        {
            Load();

            int nextId = _hotels.Count == 0
                ? 1
                : _hotels.Max(h => h.Id) + 1;

            hotel.Id = nextId;
            _hotels.Add(hotel);
            Save();

            return hotel;
        }
    }
}
