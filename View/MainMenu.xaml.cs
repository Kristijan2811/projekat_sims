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
    }
}
