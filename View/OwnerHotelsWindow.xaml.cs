using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerHotelsWindow : Window
    {
        private readonly OwnerHotelsViewModel _viewModel;

        public OwnerHotelsWindow(User loggedInOwner)
        {
            InitializeComponent();

            var hotelRepository = new HotelRepository();
            var apartmentRepository = new ApartmentRepository(); // ako treba kasnije
            var hotelService = new HotelService(hotelRepository, apartmentRepository);
            // prilagodi konstruktor HotelService-a kako trenutno izgleda kod tebe

            _viewModel = new OwnerHotelsViewModel(hotelService, loggedInOwner);
            DataContext = _viewModel;
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = _viewModel.ApproveSelectedHotel();
            MessageBox.Show(
                msg,
                "Potvrda hotela",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = _viewModel.RejectSelectedHotel();
            MessageBox.Show(
                msg,
                "Odbijanje hotela",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
