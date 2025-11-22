using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class ApartmentReservationWindow : Window
    {
        private readonly ApartmentReservationViewModel _viewModel;

        public ApartmentReservationWindow(User loggedInUser)
        {
            InitializeComponent();

            var apartmentRepository = new ApartmentRepository();
            var hotelRepository = new HotelRepository();
            var reservationRepository = new ReservationRepository();
            var reservationService = new ReservationService(reservationRepository);

            _viewModel = new ApartmentReservationViewModel(
                apartmentRepository,
                hotelRepository,
                reservationService,
                loggedInUser);

            DataContext = _viewModel;
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.Reserve();

            MessageBox.Show(
                message,
                "Rezervacija apartmana",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
