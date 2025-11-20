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

        public List<Hotel> GetAll()
        {
            Load();
            return _hotels;
        }

        // ostale metode (Add/Update/Delete...) mozes kasnije dodavati
    }
}

