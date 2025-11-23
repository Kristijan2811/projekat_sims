using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class ApartmentRepository
    {
        private const string FilePath = "../../../Resources/Data/apartments.csv";

        private readonly Serializer<Apartment> _serializer;
        private List<Apartment> _apartments;

        public ApartmentRepository()
        {
            _serializer = new Serializer<Apartment>();
            Load();
        }

        private void Load()
        {
            _apartments = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _apartments);
        }

        public List<Apartment> GetAll()
        {
            Load();
            return _apartments;
        }

        public List<Apartment> GetByHotelCode(string hotelCode)
        {
            Load();
            return _apartments
                .Where(a => a.HotelCode == hotelCode)
                .ToList();
        }

        // NOVO: dodavanje apartmana
        public Apartment Add(Apartment apartment)
        {
            Load();

            int nextId = _apartments.Count == 0
                ? 1
                : _apartments.Max(a => a.Id) + 1;

            apartment.Id = nextId;

            _apartments.Add(apartment);
            Save();

            return apartment;
        }
    }
}
