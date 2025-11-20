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
    }
}

