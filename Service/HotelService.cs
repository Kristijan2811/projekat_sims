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

        // ===== HELPER: hoteli koji su vidljivi gostima (samo Approved) =====

        private List<Hotel> GetAllApproved()
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.Status == HotelStatus.Approved)
                .ToList();
        }

        // ===== GOSTI – PRIKAZ I PRETRAGA (SAMO ODOBRENI HOTELI) =====

        // Obican prikaz svih hotela (vidljivih u sistemu) -> samo Approved
        public List<Hotel> GetAll()
        {
            return GetAllApproved();
        }

        // Sortiranje po broju zvezdica sa izborom smera (samo Approved)
        public List<Hotel> GetAllSortedByStars(bool ascending)
        {
            var hotels = GetAllApproved();

            return ascending
                ? hotels.OrderBy(h => h.Stars).ToList()              // rastuce
                : hotels.OrderByDescending(h => h.Stars).ToList();   // opadajuce
        }

        // Stara verzija – ako je negde jos pozivas bez parametra,
        // podrazumevano sortira OPADAJUCE (samo Approved)
        public List<Hotel> GetAllSortedByStars()
        {
            return GetAllSortedByStars(false);
        }

        // ---------- PRETRAGA ----------

        // 1) Pretraga po imenu (case-insensitive, delimicno poklapanje)
        public List<Hotel> SearchByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
                return new List<Hotel>();

            var lower = namePart.ToLower();

            return GetAllApproved()
                .Where(h => h.Name != null &&
                            h.Name.ToLower().Contains(lower))
                .ToList();
        }

        // 2) Pretraga po godini izgradnje
        public List<Hotel> SearchByConstructionYear(int year)
        {
            return GetAllApproved()
                .Where(h => h.ConstructionYear == year)
                .ToList();
        }

        // 3) Pretraga po broju zvezdica
        public List<Hotel> SearchByStars(int stars)
        {
            return GetAllApproved()
                .Where(h => h.Stars == stars)
                .ToList();
        }

        // 4.1) Pretraga po apartmanima - broj soba
        public List<Hotel> SearchByApartmentRooms(int roomCount)
        {
            var hotels = GetAllApproved();
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
            var hotels = GetAllApproved();
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
            var hotels = GetAllApproved();
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

        // ============ VLASNIK - PREGLED I POTVRDA / ODBIJANJE HOTELA ============

        // Hoteli za datog vlasnika (po JMBG-u) + opcioni filter po statusu
        // OVO je za vlasnika – ovde vidimo i Pending i Rejected, ne filtriramo samo na Approved
        public List<Hotel> GetOwnerHotels(string ownerJmbg, HotelStatus? statusFilter)
        {
            var hotels = _hotelRepository.GetByOwnerJmbg(ownerJmbg);

            if (statusFilter.HasValue)
            {
                hotels = hotels
                    .Where(h => h.Status == statusFilter.Value)
                    .ToList();
            }

            return hotels
                .OrderBy(h => h.Name)
                .ToList();
        }

        // Vlasnik potvrdjuje hotel (na cekanju)
        public bool ApproveHotelByOwner(int hotelId, string ownerJmbg)
        {
            var all = _hotelRepository.GetAll();
            var hotel = all.FirstOrDefault(h => h.Id == hotelId && h.OwnerJmbg == ownerJmbg);
            if (hotel == null)
            {
                return false;
            }

            hotel.Status = HotelStatus.Approved;
            _hotelRepository.Update(hotel);
            return true;
        }

        // Vlasnik odbija hotel (ako ga je admin pogresno spojio)
        public bool RejectHotelByOwner(int hotelId, string ownerJmbg)
        {
            var all = _hotelRepository.GetAll();
            var hotel = all.FirstOrDefault(h => h.Id == hotelId && h.OwnerJmbg == ownerJmbg);
            if (hotel == null)
            {
                return false;
            }

            hotel.Status = HotelStatus.Rejected;
            _hotelRepository.Update(hotel);
            return true;
        }
    }
}
