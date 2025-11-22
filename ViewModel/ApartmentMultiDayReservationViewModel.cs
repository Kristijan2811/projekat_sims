using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class ApartmentMultiDayReservationViewModel : INotifyPropertyChanged
    {
        private readonly ApartmentRepository _apartmentRepository;
        private readonly HotelRepository _hotelRepository;
        private readonly ReservationService _reservationService;
        private readonly User _loggedInUser;

        public ObservableCollection<ApartmentReservationItem> Apartments { get; set; }

        private ApartmentReservationItem _selectedApartment;
        public ApartmentReservationItem SelectedApartment
        {
            get => _selectedApartment;
            set
            {
                _selectedApartment = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public ApartmentMultiDayReservationViewModel(
            ApartmentRepository apartmentRepository,
            HotelRepository hotelRepository,
            ReservationService reservationService,
            User loggedInUser)
        {
            _apartmentRepository = apartmentRepository;
            _hotelRepository = hotelRepository;
            _reservationService = reservationService;
            _loggedInUser = loggedInUser;

            Apartments = new ObservableCollection<ApartmentReservationItem>();
            LoadApartments();
        }

        private void LoadApartments()
        {
            Apartments.Clear();

            var hotels = _hotelRepository.GetAll();
            var apartments = _apartmentRepository.GetAll();

            foreach (var apartment in apartments)
            {
                var hotel = hotels.FirstOrDefault(h => h.Code == apartment.HotelCode);
                var hotelName = hotel?.Name ?? apartment.HotelCode;

                Apartments.Add(new ApartmentReservationItem
                {
                    ApartmentId = apartment.Id,
                    ApartmentName = apartment.Name,
                    HotelName = hotelName,
                    RoomCount = apartment.RoomCount,
                    MaxGuests = apartment.MaxGuests
                });
            }
        }

        // Vraca poruku koju ce View prikazati
        public string ReserveRange()
        {
            if (SelectedApartment == null)
            {
                return "Morate odabrati apartman.";
            }

            if (StartDate == null || EndDate == null)
            {
                return "Morate odabrati pocetni i krajnji datum.";
            }

            if (_loggedInUser == null)
            {
                return "Nijedan korisnik nije ulogovan.";
            }

            var start = StartDate.Value.Date;
            var end = EndDate.Value.Date;

            if (end < start)
            {
                return "Krajnji datum ne moze biti pre pocetnog.";
            }

            // zabrana proslih datuma (isto kao sto smo radili za jedan dan)
            if (start < DateTime.Today)
            {
                return "Ne mozete rezervisati datume koji su vec prosli.";
            }

            var reservations = _reservationService.CreateReservationForRange(
                SelectedApartment.ApartmentId,
                _loggedInUser.Id,
                start,
                end);

            if (reservations == null)
            {
                return "Apartman je zauzet u izabranom periodu.";
            }

            return "Rezervacija za vise dana je poslata vlasniku. Molimo sacekajte potvrdu.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
