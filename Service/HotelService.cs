using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Service
{
    public class HotelService
    {
        private readonly HotelRepository _hotelRepository;
        private readonly ApartmentRepository _apartmentRepository;

        public HotelService(HotelRepository hotelRepository, ApartmentRepository apartmentRepository)
        {
            _hotelRepository = hotelRepository;
            _apartmentRepository = apartmentRepository;
        }

        // Obican prikaz svih hotela
        public List<Hotel> GetAll()
        {
            return _hotelRepository.GetAll();
        }

        // NOVO: sortiranje po broju zvezdica sa izborom smera
        public List<Hotel> GetAllSortedByStars(bool ascending)
        {
            var hotels = _hotelRepository.GetAll();

            return ascending
                ? hotels.OrderBy(h => h.Stars).ToList()              // rastuće
                : hotels.OrderByDescending(h => h.Stars).ToList();   // opadajuće
        }

        // Stara verzija – ako je negde još pozivaš bez parametra,
        // podrazumevano sortira OPADAJUĆE
        public List<Hotel> GetAllSortedByStars()
        {
            return GetAllSortedByStars(false);
        }

        // ---------- PRETRAGA ----------

        // 1) Pretraga po imenu (case-insensitive, delimično poklapanje)
        public List<Hotel> SearchByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
                return new List<Hotel>();

            var lower = namePart.ToLower();

            return _hotelRepository
                .GetAll()
                .Where(h => h.Name != null &&
                            h.Name.ToLower().Contains(lower))
                .ToList();
        }

        // 2) Pretraga po godini izgradnje
        public List<Hotel> SearchByConstructionYear(int year)
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.ConstructionYear == year)
                .ToList();
        }

        // 3) Pretraga po broju zvezdica
        public List<Hotel> SearchByStars(int stars)
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.Stars == stars)
                .ToList();
        }

        // 4.1) Pretraga po apartmanima - broj soba
        public List<Hotel> SearchByApartmentRooms(int roomCount)
        {
            var hotels = _hotelRepository.GetAll();
            var result = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                var apartments = _apartmentRepository.GetByHotelCode(hotel.Code);
                if (apartments.Any(a => a.RoomCount == roomCount))
                {
                    result.Add(hotel);
                }
            }

            return result;
        }

        // 4.2) Pretraga po apartmanima - broj gostiju
        public List<Hotel> SearchByApartmentGuests(int maxGuests)
        {
            var hotels = _hotelRepository.GetAll();
            var result = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                var apartments = _apartmentRepository.GetByHotelCode(hotel.Code);
                if (apartments.Any(a => a.MaxGuests == maxGuests))
                {
                    result.Add(hotel);
                }
            }

            return result;
        }

        // 4.3) Pretraga po apartmanima - broj soba + broj gostiju, sa AND / OR
        public List<Hotel> SearchByApartmentRoomsAndGuests(
            int roomCount,
            int maxGuests,
            bool useAndOperator)   // true = AND (&), false = OR (|)
        {
            var hotels = _hotelRepository.GetAll();
            var result = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                var apartments = _apartmentRepository.GetByHotelCode(hotel.Code);

                bool match;
                if (useAndOperator)
                {
                    // 2 & 3 -> apartman sa 2 sobe I 3 gosta
                    match = apartments.Any(a =>
                        a.RoomCount == roomCount && a.MaxGuests == maxGuests);
                }
                else
                {
                    // 2 | 3 -> apartman sa 2 sobe ILI 3 gosta
                    match = apartments.Any(a =>
                        a.RoomCount == roomCount || a.MaxGuests == maxGuests);
                }

                if (match)
                {
                    result.Add(hotel);
                }
            }

            return result;
        }
    }
}
