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
    public class OwnerReservationItem
    {
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set; }
        public string ApartmentName { get; set; }
        public string GuestName { get; set; }
        public string StatusText { get; set; }

        public ReservationStatus Status { get; set; }
    }

    public class OwnerReservationsViewModel : INotifyPropertyChanged
    {
        private readonly ReservationService _reservationService;
        private readonly ApartmentRepository _apartmentRepository;
        private readonly HotelRepository _hotelRepository;
        private readonly UserRepository _userRepository;
        private readonly User _loggedInOwner;

        public ObservableCollection<OwnerReservationItem> Reservations { get; set; }

        // hoteli kojima je ovaj korisnik vlasnik
        public ObservableCollection<Hotel> OwnerHotels { get; set; }

        private Hotel _selectedHotel;
        public Hotel SelectedHotel
        {
            get => _selectedHotel;
            set
            {
                _selectedHotel = value;
                OnPropertyChanged();
                LoadReservations();
            }
        }

        // "Sve", "Na cekanju", "Potvrdjene"
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

        private OwnerReservationItem _selectedReservation;
        public OwnerReservationItem SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                _selectedReservation = value;
                OnPropertyChanged();
            }
        }

        // tekst koji korisnik unosi kao razlog odbijanja
        private string _rejectionReasonText;
        public string RejectionReasonText
        {
            get => _rejectionReasonText;
            set
            {
                _rejectionReasonText = value;
                OnPropertyChanged();
            }
        }

        public OwnerReservationsViewModel(
            ReservationService reservationService,
            ApartmentRepository apartmentRepository,
            HotelRepository hotelRepository,
            UserRepository userRepository,
            User loggedInOwner)
        {
            _reservationService = reservationService;
            _apartmentRepository = apartmentRepository;
            _hotelRepository = hotelRepository;
            _userRepository = userRepository;
            _loggedInOwner = loggedInOwner;

            Reservations = new ObservableCollection<OwnerReservationItem>();
            OwnerHotels = new ObservableCollection<Hotel>();

            SelectedFilter = "Sve"; // podrazumevani filter

            LoadOwnerHotels();
        }

        private void LoadOwnerHotels()
        {
            OwnerHotels.Clear();

            if (_loggedInOwner == null || _loggedInOwner.UserType != UserType.Owner)
            {
                return;
            }

            // hoteli za koje je ovaj korisnik vlasnik (po JMBG-u)
            var hotels = _hotelRepository.GetByOwnerJmbg(_loggedInOwner.Jmbg);

            foreach (var hotel in hotels)
            {
                OwnerHotels.Add(hotel);
            }

            // podrazumevano odaberi prvi, ako postoji
            if (OwnerHotels.Any())
            {
                SelectedHotel = OwnerHotels.First();
            }
        }

        private void LoadReservations()
        {
            Reservations.Clear();

            if (_loggedInOwner == null ||
                _loggedInOwner.UserType != UserType.Owner ||
                SelectedHotel == null)
            {
                return;
            }

            // 1) Apartmani iz odabranog hotela
            var allApartments = _apartmentRepository.GetAll();
            var hotelApartments = allApartments
                .Where(a => a.HotelCode == SelectedHotel.Code)
                .ToList();

            var apartmentIds = hotelApartments
                .Select(a => a.Id)
                .ToHashSet();

            // 2) Sve rezervacije za te apartmane
            var allReservations = _reservationService.GetAll();

            // po zadatku: prikaz samo Pending + Approved
            var hotelReservations = allReservations
                .Where(r =>
                    apartmentIds.Contains(r.ApartmentId) &&
                    (r.Status == ReservationStatus.Pending ||
                     r.Status == ReservationStatus.Approved))
                .ToList();

            // 3) Filter po statusu
            ReservationStatus? filterStatus = null;

            switch (SelectedFilter)
            {
                case "Na cekanju":
                    filterStatus = ReservationStatus.Pending;
                    break;
                case "Potvrdjene":
                    filterStatus = ReservationStatus.Approved;
                    break;
                case "Sve":
                default:
                    filterStatus = null;
                    break;
            }

            if (filterStatus.HasValue)
            {
                hotelReservations = hotelReservations
                    .Where(r => r.Status == filterStatus.Value)
                    .ToList();
            }

            // 4) Potrebni su nam i gosti
            var allUsers = _userRepository.GetAll();

            // 5) Mapiramo u prikazne stavke
            foreach (var reservation in hotelReservations.OrderBy(r => r.Date))
            {
                var apartment = hotelApartments.FirstOrDefault(a => a.Id == reservation.ApartmentId);
                var guest = allUsers.FirstOrDefault(u => u.Id == reservation.GuestId);

                string apartmentName = apartment?.Name ?? "Nepoznat apartman";
                string guestName = guest != null
                    ? $"{guest.FirstName} {guest.LastName}"
                    : "Nepoznat gost";

                Reservations.Add(new OwnerReservationItem
                {
                    ReservationId = reservation.Id,
                    Date = reservation.Date,
                    HotelName = SelectedHotel.Name,
                    ApartmentName = apartmentName,
                    GuestName = guestName,
                    StatusText = reservation.Status.ToString(),
                    Status = reservation.Status
                });
            }
        }

        // ====== AKCIJE: POTVRDA / ODBIJANJE ======

        public string ApproveSelectedReservation()
        {
            if (SelectedReservation == null)
            {
                return "Morate izabrati rezervaciju.";
            }

            if (SelectedReservation.Status != ReservationStatus.Pending)
            {
                return "Mozete potvrditi samo rezervacije koje su na cekanju.";
            }

            bool success = _reservationService.ApproveReservation(SelectedReservation.ReservationId);

            if (!success)
            {
                return "Rezervaciju nije moguce potvrditi.";
            }

            LoadReservations();
            return "Rezervacija je uspesno potvrdjena.";
        }

        public string RejectSelectedReservation()
        {
            if (SelectedReservation == null)
            {
                return "Morate izabrati rezervaciju.";
            }

            if (SelectedReservation.Status != ReservationStatus.Pending)
            {
                return "Mozete odbiti samo rezervacije koje su na cekanju.";
            }

            if (string.IsNullOrWhiteSpace(RejectionReasonText))
            {
                return "Morate uneti razlog odbijanja.";
            }

            bool success = _reservationService.RejectReservation(
                SelectedReservation.ReservationId,
                RejectionReasonText.Trim());

            if (!success)
            {
                return "Rezervaciju nije moguce odbiti.";
            }

            // ocisti razlog i osvezi listu
            RejectionReasonText = string.Empty;
            LoadReservations();
            return "Rezervacija je odbijena.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
