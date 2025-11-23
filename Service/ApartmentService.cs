using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Service
{
    public class ApartmentService
    {
        private readonly ApartmentRepository _apartmentRepository;
        private readonly HotelRepository _hotelRepository;

        public ApartmentService(ApartmentRepository apartmentRepository, HotelRepository hotelRepository)
        {
            _apartmentRepository = apartmentRepository;
            _hotelRepository = hotelRepository;
        }

        // Kreira apartman ako:
        // - hotel postoji
        // - hotel pripada ovom vlasniku (ownerJmbg)
        // - ime apartmana je jedinstveno u okviru tog hotela
        // Vraca null ako nesto od toga nije ispunjeno.
        public Apartment CreateApartment(
            string name,
            string description,
            int roomCount,
            int maxGuests,
            string hotelCode,
            string ownerJmbg)
        {
            // 1) hotel mora da postoji i da pripada ovom vlasniku
            var ownerHotels = _hotelRepository.GetByOwnerJmbg(ownerJmbg);
            var hotel = ownerHotels.FirstOrDefault(h => h.Code == hotelCode);
            if (hotel == null)
            {
                // hotel ne postoji ili nije od ovog vlasnika
                return null;
            }

            // 2) ime apartmana mora biti jedinstveno unutar hotela
            var apartmentsInHotel = _apartmentRepository.GetByHotelCode(hotelCode);
            if (apartmentsInHotel.Any(a => a.Name == name))
            {
                // postoji vec apartman sa tim imenom u ovom hotelu
                return null;
            }

            // 3) kreiramo apartman
            var apartment = new Apartment
            {
                Name = name,
                Description = description,
                RoomCount = roomCount,
                MaxGuests = maxGuests,
                HotelCode = hotelCode
            };

            return _apartmentRepository.Add(apartment);
        }
    }
}
