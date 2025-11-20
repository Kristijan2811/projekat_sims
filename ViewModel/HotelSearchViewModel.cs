using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class HotelSearchViewModel : INotifyPropertyChanged
    {
        private readonly HotelService _hotelService;

        public ObservableCollection<Hotel> Hotels { get; set; }

        // -------------- izbor tipa pretrage --------------

        // "Po imenu", "Po godini izgradnje", "Po broju zvezdica", "Pretraga po apartmanima"
        private string _selectedSearchOption;
        public string SelectedSearchOption
        {
            get => _selectedSearchOption;
            set
            {
                _selectedSearchOption = value;
                OnPropertyChanged();
            }
        }

        // Univerzalni tekst za ime / godinu / zvezdice
        public string SearchText { get; set; }

        // -------------- pretraga po apartmanima --------------

        // "Broj soba", "Broj gostiju", "Broj soba + broj gostiju"
        private string _selectedApartmentSearchOption;
        public string SelectedApartmentSearchOption
        {
            get => _selectedApartmentSearchOption;
            set
            {
                _selectedApartmentSearchOption = value;
                OnPropertyChanged();
            }
        }

        public string ApartmentRoomsText { get; set; }
        public string ApartmentGuestsText { get; set; }

        // "AND" ili "OR"
        private string _selectedLogicalOperator;
        public string SelectedLogicalOperator
        {
            get => _selectedLogicalOperator;
            set
            {
                _selectedLogicalOperator = value;
                OnPropertyChanged();
            }
        }

        public HotelSearchViewModel(HotelService hotelService)
        {
            _hotelService = hotelService;
            Hotels = new ObservableCollection<Hotel>();

            // podrazumevane vrednosti
            SelectedSearchOption = "Po imenu";
            SelectedApartmentSearchOption = "Broj soba";
            SelectedLogicalOperator = "AND";

            LoadAllHotels();
        }

        private void LoadAllHotels()
        {
            Hotels.Clear();
            var hotels = _hotelService.GetAll();
            foreach (var h in hotels)
            {
                Hotels.Add(h);
            }
        }

        public void Search()
        {
            Hotels.Clear();

            // 1) pretraga po osnovnim parametrima (ime/godina/zvezdice)
            if (SelectedSearchOption == "Po imenu")
            {
                var result = _hotelService.SearchByName(SearchText ?? "");
                foreach (var h in result) Hotels.Add(h);
                return;
            }

            if (SelectedSearchOption == "Po godini izgradnje")
            {
                if (int.TryParse(SearchText, out var year))
                {
                    var result = _hotelService.SearchByConstructionYear(year);
                    foreach (var h in result) Hotels.Add(h);
                }
                return;
            }

            if (SelectedSearchOption == "Po broju zvezdica")
            {
                if (int.TryParse(SearchText, out var stars))
                {
                    var result = _hotelService.SearchByStars(stars);
                    foreach (var h in result) Hotels.Add(h);
                }
                return;
            }

            // 2) pretraga po apartmanima
            if (SelectedSearchOption == "Pretraga po apartmanima")
            {
                // Broj soba
                if (SelectedApartmentSearchOption == "Broj soba")
                {
                    if (int.TryParse(ApartmentRoomsText, out var rooms))
                    {
                        var result = _hotelService.SearchByApartmentRooms(rooms);
                        foreach (var h in result) Hotels.Add(h);
                    }
                    return;
                }

                // Broj gostiju
                if (SelectedApartmentSearchOption == "Broj gostiju")
                {
                    if (int.TryParse(ApartmentGuestsText, out var guests))
                    {
                        var result = _hotelService.SearchByApartmentGuests(guests);
                        foreach (var h in result) Hotels.Add(h);
                    }
                    return;
                }

                // Broj soba + broj gostiju (sa AND / OR)
                if (SelectedApartmentSearchOption == "Broj soba + broj gostiju")
                {
                    if (int.TryParse(ApartmentRoomsText, out var rooms) &&
                        int.TryParse(ApartmentGuestsText, out var guests))
                    {
                        bool useAnd = SelectedLogicalOperator == "AND";
                        var result = _hotelService.SearchByApartmentRoomsAndGuests(rooms, guests, useAnd);
                        foreach (var h in result) Hotels.Add(h);
                    }
                    return;
                }
            }

            // ako nista nije pogođeno, vrati sve
            LoadAllHotels();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
