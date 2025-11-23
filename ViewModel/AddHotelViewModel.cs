using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class AddHotelViewModel : INotifyPropertyChanged
    {
        private readonly HotelAdminService _hotelAdminService;

        public string Code { get; set; }
        public string Name { get; set; }
        public string ConstructionYearText { get; set; }
        public string StarsText { get; set; }
        public string OwnerJmbg { get; set; }

        public AddHotelViewModel(HotelAdminService hotelAdminService)
        {
            _hotelAdminService = hotelAdminService;
        }

        // Vraca poruku za prikaz u MessageBox-u
        public string TryCreateHotel()
        {
            if (string.IsNullOrWhiteSpace(Code) ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(ConstructionYearText) ||
                string.IsNullOrWhiteSpace(StarsText) ||
                string.IsNullOrWhiteSpace(OwnerJmbg))
            {
                return "Sva polja moraju biti popunjena.";
            }

            if (!int.TryParse(ConstructionYearText, out int year) || year <= 0)
            {
                return "Godina izgradnje mora biti pozitivan ceo broj.";
            }

            if (!int.TryParse(StarsText, out int stars) || stars < 1 || stars > 5)
            {
                return "Broj zvezdica mora biti ceo broj od 1 do 5.";
            }

            var hotel = _hotelAdminService.CreateHotel(
                Code.Trim(),
                Name.Trim(),
                year,
                stars,
                OwnerJmbg.Trim());

            if (hotel == null)
            {
                return "Hotel nije sacuvan. Proverite da li je sifra jedinstvena, da li postoji vlasnik sa datim JMBG-om i da li je vlasnik pravog tipa.";
            }

            // mozes i da ocistis formu
            Code = string.Empty;
            Name = string.Empty;
            ConstructionYearText = string.Empty;
            StarsText = string.Empty;
            OwnerJmbg = string.Empty;
            OnPropertyChanged(nameof(Code));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ConstructionYearText));
            OnPropertyChanged(nameof(StarsText));
            OnPropertyChanged(nameof(OwnerJmbg));

            return "Hotel je uspesno dodat sa statusom 'Na cekanju'. Vlasnik mora da ga odobri da bi bio vidljiv gostima.";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
