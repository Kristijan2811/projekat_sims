using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class AddApartmentViewModel : INotifyPropertyChanged
    {
        private readonly ApartmentService _apartmentService;
        private readonly HotelService _hotelService;
        private readonly User _loggedInOwner;

        public ObservableCollection<Hotel> OwnerHotels { get; set; }

        private Hotel _selectedHotel;
        public Hotel SelectedHotel
        {
            get => _selectedHotel;
            set
            {
                _selectedHotel = value;
                OnPropertyChanged();
            }
        }

        // Podaci o apartmanu
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _roomCountText;
        public string RoomCountText
        {
            get => _roomCountText;
            set { _roomCountText = value; OnPropertyChanged(); }
        }

        private string _maxGuestsText;
        public string MaxGuestsText
        {
            get => _maxGuestsText;
            set { _maxGuestsText = value; OnPropertyChanged(); }
        }

        public AddApartmentViewModel(
            ApartmentService apartmentService,
            HotelService hotelService,
            User loggedInOwner)
        {
            _apartmentService = apartmentService;
            _hotelService = hotelService;
            _loggedInOwner = loggedInOwner;

            OwnerHotels = new ObservableCollection<Hotel>();

            LoadOwnerHotels();
        }

        private void LoadOwnerHotels()
        {
            OwnerHotels.Clear();

            if (_loggedInOwner == null || _loggedInOwner.UserType != UserType.Owner)
            {
                return;
            }

            // samo njegovi hoteli; ako hoces, moze i samo Approved
            var hotels = _hotelService.GetOwnerHotels(_loggedInOwner.Jmbg, null);

            foreach (var hotel in hotels)
            {
                OwnerHotels.Add(hotel);
            }

            if (OwnerHotels.Any())
            {
                SelectedHotel = OwnerHotels.First();
            }
        }

        // Vraca poruku koju ce View prikazati
        public string TryCreateApartment()
        {
            if (_loggedInOwner == null || _loggedInOwner.UserType != UserType.Owner)
            {
                return "Samo vlasnik moze da unosi apartmane.";
            }

            if (SelectedHotel == null)
            {
                return "Morate odabrati hotel.";
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                return "Morate uneti ime apartmana.";
            }

            if (!int.TryParse(RoomCountText, out int roomCount) || roomCount <= 0)
            {
                return "Broj soba mora biti pozitivan ceo broj.";
            }

            if (!int.TryParse(MaxGuestsText, out int maxGuests) || maxGuests <= 0)
            {
                return "Max broj gostiju mora biti pozitivan ceo broj.";
            }

            var apartment = _apartmentService.CreateApartment(
                Name.Trim(),
                Description?.Trim() ?? string.Empty,
                roomCount,
                maxGuests,
                SelectedHotel.Code,
                _loggedInOwner.Jmbg);

            if (apartment == null)
            {
                return "Apartman nije sacuvan. Proverite da li hotel zaista pripada vama i da li vec postoji apartman sa tim imenom u tom hotelu.";
            }

            // ako hoces, mozes i da ocistis formu
            Name = string.Empty;
            Description = string.Empty;
            RoomCountText = string.Empty;
            MaxGuestsText = string.Empty;

            return "Apartman je uspesno dodat.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
