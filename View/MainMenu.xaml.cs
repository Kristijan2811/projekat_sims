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
    }
}
