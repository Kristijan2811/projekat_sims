using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class ApartmentMultiDayReservationWindow : Window
    {
        private readonly ApartmentMultiDayReservationViewModel _viewModel;

        public ApartmentMultiDayReservationWindow(User loggedInUser)
        {
            InitializeComponent();

            var apartmentRepository = new ApartmentRepository();
            var hotelRepository = new HotelRepository();
            var reservationRepository = new ReservationRepository();
            var reservationService = new ReservationService(reservationRepository);

            _viewModel = new ApartmentMultiDayReservationViewModel(
                apartmentRepository,
                hotelRepository,
                reservationService,
                loggedInUser);

            DataContext = _viewModel;
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.ReserveRange();

            MessageBox.Show(
                message,
                "Rezervacija apartmana",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
