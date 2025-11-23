using System.Windows;
using BookingApp.Model;

namespace BookingApp.View
{
    public partial class MainMenu : Window
    {
        private readonly User _loggedInUser;

        // Konstruktor koji koristimo kad se korisnik uloguje
        public MainMenu(User loggedInUser)
        {
            InitializeComponent();
            _loggedInUser = loggedInUser;
            // ovde kasnije mozes da prilagodis meni po tipu korisnika
        }

        // Prazan konstruktor za XAML designer / slucajeve bez prosledjenog user-a
        public MainMenu() : this(null)
        {
        }

        private void ShowHotelsButton_Click(object sender, RoutedEventArgs e)
        {
            var hotelsOverview = new HotelsOverview();
            hotelsOverview.Show();
        }

        private void SearchHotelsButton_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new HotelSearchWindow();
            searchWindow.Show();
        }

        private void ReserveApartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // dozvoljeno samo gostima
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Guest)
            {
                MessageBox.Show(
                    "Samo gosti mogu da rezervisu apartmane.",
                    "Rezervacija apartmana",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var reservationWindow = new ApartmentReservationWindow(_loggedInUser);
            reservationWindow.Show();
        }

        private void ReserveApartmentMultiDayButton_Click(object sender, RoutedEventArgs e)
        {
            // isto pravilo: samo gosti
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Guest)
            {
                MessageBox.Show(
                    "Samo gosti mogu da rezervisu apartmane.",
                    "Rezervacija apartmana",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var reservationWindow = new ApartmentMultiDayReservationWindow(_loggedInUser);
            reservationWindow.Show();
        }

        private void ShowGuestReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            // samo gosti imaju svoje rezervacije
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Guest)
            {
                MessageBox.Show(
                    "Samo gosti imaju svoje rezervacije.",
                    "Moje rezervacije",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new GuestReservationsWindow(_loggedInUser);
            window.Show();
        }

        // rezervacije iz perspektive vlasnika
        private void OwnerReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Owner)
            {
                MessageBox.Show(
                    "Samo vlasnici mogu da vide rezervacije za svoje apartmane.",
                    "Rezervacije (vlasnik)",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new OwnerReservationsWindow(_loggedInUser);
            window.Show();
        }

        // upravljanje sopstvenim hotelima (potvrda / odbijanje)
        private void OwnerHotelsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Owner)
            {
                MessageBox.Show(
                    "Samo vlasnici mogu da upravljaju svojim hotelima.",
                    "Moji hoteli",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new OwnerHotelsWindow(_loggedInUser);
            window.Show();
        }

        // unos apartmana
        private void AddApartmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Owner)
            {
                MessageBox.Show(
                    "Samo vlasnici mogu da unose apartmane.",
                    "Unos apartmana",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new AddApartmentWindow(_loggedInUser);
            window.Show();
        }

        // registracija vlasnika (samo admin)
        private void RegisterOwnerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Administrator)
            {
                MessageBox.Show(
                    "Samo administrator moze da registruje vlasnike.",
                    "Registracija vlasnika",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new OwnerRegisterWindow();
            window.Show();
        }

        // NOVO: unos hotela (samo admin)
        private void AddHotelAdminButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || _loggedInUser.UserType != UserType.Administrator)
            {
                MessageBox.Show(
                    "Samo administrator moze da unosi hotele.",
                    "Unos hotela",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var window = new AddHotelWindow();
            window.Show();
        }
    }
}
