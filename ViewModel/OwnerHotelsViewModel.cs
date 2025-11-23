using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class OwnerHotelItem
    {
        public int HotelId { get; set; }
        public string Code { get; set; }      // sifra
        public string Name { get; set; }
        public int ConstructionYear { get; set; }
        public int Stars { get; set; }
        public string StatusText { get; set; }

        public HotelStatus Status { get; set; }
    }

    public class OwnerHotelsViewModel : INotifyPropertyChanged
    {
        private readonly HotelService _hotelService;
        private readonly User _loggedInOwner;

        public ObservableCollection<OwnerHotelItem> Hotels { get; set; }

        // "Sve", "Na cekanju", "Prihvaceni"
        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                LoadHotels();
            }
        }

        private OwnerHotelItem _selectedHotel;
        public OwnerHotelItem SelectedHotel
        {
            get => _selectedHotel;
            set
            {
                _selectedHotel = value;
                OnPropertyChanged();
            }
        }

        public OwnerHotelsViewModel(HotelService hotelService, User loggedInOwner)
        {
            _hotelService = hotelService;
            _loggedInOwner = loggedInOwner;

            Hotels = new ObservableCollection<OwnerHotelItem>();

            SelectedFilter = "Sve"; // podrazumevani filter
        }

        private void LoadHotels()
        {
            Hotels.Clear();

            if (_loggedInOwner == null || _loggedInOwner.UserType != UserType.Owner)
            {
                return;
            }

            HotelStatus? filter = null;
            switch (SelectedFilter)
            {
                case "Na cekanju":
                    filter = HotelStatus.Pending;
                    break;
                case "Prihvaceni":
                    filter = HotelStatus.Approved;
                    break;
                case "Sve":
                default:
                    filter = null;
                    break;
            }

            var hotels = _hotelService.GetOwnerHotels(_loggedInOwner.Jmbg, filter);

            foreach (var hotel in hotels)
            {
                Hotels.Add(new OwnerHotelItem
                {
                    HotelId = hotel.Id,
                    Code = hotel.Code,
                    Name = hotel.Name,
                    ConstructionYear = hotel.ConstructionYear,
                    Stars = hotel.Stars,
                    Status = hotel.Status,
                    StatusText = hotel.Status.ToString()
                });
            }
        }

        public string ApproveSelectedHotel()
        {
            if (SelectedHotel == null)
            {
                return "Morate izabrati hotel.";
            }

            if (SelectedHotel.Status != HotelStatus.Pending)
            {
                return "Mozete potvrditi samo hotel koji je na cekanju.";
            }

            bool success = _hotelService.ApproveHotelByOwner(SelectedHotel.HotelId, _loggedInOwner.Jmbg);
            if (!success)
            {
                return "Hotel nije moguce potvrditi.";
            }

            LoadHotels();
            return "Hotel je uspesno potvrdjen.";
        }

        public string RejectSelectedHotel()
        {
            if (SelectedHotel == null)
            {
                return "Morate izabrati hotel.";
            }

            if (SelectedHotel.Status != HotelStatus.Pending)
            {
                return "Mozete odbiti samo hotel koji je na cekanju.";
            }

            bool success = _hotelService.RejectHotelByOwner(SelectedHotel.HotelId, _loggedInOwner.Jmbg);
            if (!success)
            {
                return "Hotel nije moguce odbiti.";
            }

            LoadHotels();
            return "Hotel je odbijen.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
