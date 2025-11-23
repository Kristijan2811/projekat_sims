using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class AddHotelWindow : Window
    {
        private readonly AddHotelViewModel _viewModel;

        public AddHotelWindow()
        {
            InitializeComponent();

            var hotelRepository = new HotelRepository();
            var userRepository = new UserRepository();
            var hotelAdminService = new HotelAdminService(hotelRepository, userRepository);

            _viewModel = new AddHotelViewModel(hotelAdminService);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.TryCreateHotel();

            MessageBox.Show(
                message,
                "Unos hotela",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
