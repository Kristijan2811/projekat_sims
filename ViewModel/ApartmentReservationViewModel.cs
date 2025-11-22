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
    public class ApartmentReservationItem
    {
        public int ApartmentId { get; set; }
        public string ApartmentName { get; set; }
        public string HotelName { get; set; }
        public int RoomCount { get; set; }
        public int MaxGuests { get; set; }
    }

    public class ApartmentReservationViewModel : INotifyPropertyChanged
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

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public ApartmentReservationViewModel(
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
        public string Reserve()
        {
            if (SelectedApartment == null)
            {
                return "Morate odabrati apartman.";
            }

            if (SelectedDate == null)
            {
                return "Morate odabrati datum.";
            }

            if (_loggedInUser == null)
            {
                return "Nijedan korisnik nije ulogovan.";
            }

            var chosenDate = SelectedDate.Value.Date;

            // NOVO: zabrana rezervacije za datume u proslosti
            if (chosenDate < DateTime.Today)
            {
                return "Ne mozete rezervisati datum koji je vec prosao.";
            }

            var reservation = _reservationService.CreateReservation(
                SelectedApartment.ApartmentId,
                _loggedInUser.Id,
                chosenDate);

            if (reservation == null)
            {
                return "Apartman je zauzet za odabrani datum.";
            }

            return "Rezervacija je poslata vlasniku. Molimo sacekajte potvrdu.";
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
