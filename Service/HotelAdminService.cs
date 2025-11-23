using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Service
{
    public class HotelAdminService
    {
        private readonly HotelRepository _hotelRepository;
        private readonly UserRepository _userRepository;

        public HotelAdminService(HotelRepository hotelRepository, UserRepository userRepository)
        {
            _hotelRepository = hotelRepository;
            _userRepository = userRepository;
        }

        // Admin kreira hotel.
        // Vraca null ako:
        // - sifra nije jedinstvena
        // - ne postoji korisnik sa tim JMBG-om
        // - korisnik postoji ali nije vlasnik
        public Hotel CreateHotel(
            string code,
            string name,
            int constructionYear,
            int stars,
            string ownerJmbg)
        {
            if (string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(ownerJmbg))
            {
                return null;
            }

            // sifra mora biti jedinstvena
            var allHotels = _hotelRepository.GetAll();
            if (allHotels.Any(h => h.Code == code))
            {
                return null;
            }

            // vlasnik mora postojati i biti tip Owner
            var owner = _userRepository.GetByJmbg(ownerJmbg);
            if (owner == null || owner.UserType != UserType.Owner)
            {
                return null;
            }

            var hotel = new Hotel
            {
                Code = code,
                Name = name,
                ConstructionYear = constructionYear,
                Stars = stars,
                OwnerJmbg = ownerJmbg,
                Status = HotelStatus.Pending   // po zadatku: ceka odobrenje vlasnika
            };

            return _hotelRepository.Add(hotel);
        }
    }
}
