using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class HotelsOverviewViewModel : INotifyPropertyChanged
    {
        private readonly HotelService _hotelService;

        public ObservableCollection<Hotel> Hotels { get; set; }

        // "Bez sortiranja", "Rastuce", "Opadajuce"
        private string _selectedSortOrder;
        public string SelectedSortOrder
        {
            get => _selectedSortOrder;
            set
            {
                _selectedSortOrder = value;
                OnPropertyChanged();
                LoadHotels();   // svaki put kad promenis opciju, ponovo ucitamo listu
            }
        }

        public HotelsOverviewViewModel(HotelService hotelService)
        {
            _hotelService = hotelService;
            Hotels = new ObservableCollection<Hotel>();

            // podrazumevano: bez sortiranja
            SelectedSortOrder = "Bez sortiranja";
            LoadHotels();
        }

        private void LoadHotels()
        {
            Hotels.Clear();

            var hotels = _hotelService.GetAll();

            if (SelectedSortOrder == "Rastuce")
            {
                hotels = _hotelService.GetAllSortedByStars(true);   // rastuce
            }
            else if (SelectedSortOrder == "Opadajuce")
            {
                hotels = _hotelService.GetAllSortedByStars(false);  // opadajuce
            }
            // ako je "Bez sortiranja" – ostane ono iz GetAll()

            foreach (var hotel in hotels)
            {
                Hotels.Add(hotel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
