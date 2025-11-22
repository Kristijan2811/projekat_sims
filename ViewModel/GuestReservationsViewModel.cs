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
    public class GuestReservationItem
    {
        public int ReservationId { get; set; }      // NOVO: Id rezervacije
        public DateTime Date { get; set; }
        public string HotelName { get; set; }
        public string ApartmentName { get; set; }
        public string StatusText { get; set; }
        public string RejectionReason { get; set; }

        // NOVO: pravi enum status (za logiku otkazivanja)
        public ReservationStatus Status { get; set; }
    }

    public class GuestReservationsViewModel : INotifyPropertyChanged
    {
        private readonly ReservationService _reservationService;
        private readonly ApartmentRepository _apartmentRepository;
        private readonly HotelRepository _hotelRepository;
        private readonly User _loggedInUser;

        public ObservableCollection<GuestReservationItem> Reservations { get; set; }

        // NOVO: selektovana rezervacija iz DataGrid-a
        private GuestReservationItem _selectedReservation;
        public GuestReservationItem SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                _selectedReservation = value;
                OnPropertyChanged();
            }
        }

        // "Sve", "Na cekanju", "Potvrdjene", "Odbijene"
        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                LoadReservations();
            }
        }

        public GuestReservationsViewModel(
            ReservationService reservationService,
            ApartmentRepository apartmentRepository,
            HotelRepository hotelRepository,
            User loggedInUser)
        {
            _reservationService = reservationService;
            _apartmentRepository = apartmentRepository;
            _hotelRepository = hotelRepository;
            _loggedInUser = loggedInUser;

            Reservations = new ObservableCollection<GuestReservationItem>();

            SelectedFilter = "Sve"; // podrazumevani filter
        }

        private void LoadReservations()
        {
            Reservations.Clear();

            if (_loggedInUser == null)
            {
                return;
            }

            ReservationStatus? filterStatus = null;

            switch (SelectedFilter)
            {
                case "Na cekanju":
                    filterStatus = ReservationStatus.Pending;
                    break;
                case "Potvrdjene":
                    filterStatus = ReservationStatus.Approved;
                    break;
                case "Odbijene":
                    filterStatus = ReservationStatus.Rejected;
                    break;
                case "Sve":
                default:
                    filterStatus = null;
                    break;
            }

            var reservations = _reservationService.GetAllForGuestFiltered(
                _loggedInUser.Id,
                filterStatus);

            var apartments = _apartmentRepository.GetAll();
            var hotels = _hotelRepository.GetAll();

            foreach (var reservation in reservations)
            {
                var apartment = apartments.FirstOrDefault(a => a.Id == reservation.ApartmentId);
                var hotel = apartment != null
                    ? hotels.FirstOrDefault(h => h.Code == apartment.HotelCode)
                    : null;

                string hotelName = hotel?.Name ?? "Nepoznat hotel";
                string apartmentName = apartment?.Name ?? "Nepoznat apartman";

                string statusText = reservation.Status.ToString(); // Pending/Approved/Rejected

                Reservations.Add(new GuestReservationItem
                {
                    ReservationId = reservation.Id,          // NOVO
                    Date = reservation.Date,
                    HotelName = hotelName,
                    ApartmentName = apartmentName,
                    StatusText = statusText,
                    RejectionReason = reservation.Status == ReservationStatus.Rejected
                        ? reservation.RejectionReason
                        : string.Empty,
                    Status = reservation.Status               // NOVO
                });
            }
        }

        // NOVO: poziva ga View kada kliknes na dugme "Otkazi"
        public string CancelSelectedReservation()
        {
            if (SelectedReservation == null)
            {
                return "Morate izabrati rezervaciju.";
            }

            // dozvoljeno samo Pending ili Approved
            if (SelectedReservation.Status != ReservationStatus.Pending &&
                SelectedReservation.Status != ReservationStatus.Approved)
            {
                return "Mozete otkazati samo rezervacije koje su na cekanju ili potvrdjene.";
            }

            bool success = _reservationService.CancelReservation(
                SelectedReservation.ReservationId,
                _loggedInUser.Id);

            if (!success)
            {
                return "Ovu rezervaciju nije moguce otkazati.";
            }

            // da se odmah osvezi prikaz
            LoadReservations();

            return "Rezervacija je uspesno otkazana.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
