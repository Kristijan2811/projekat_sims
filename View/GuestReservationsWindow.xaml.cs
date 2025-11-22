using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class GuestReservationsWindow : Window
    {
        private readonly GuestReservationsViewModel _viewModel;

        public GuestReservationsWindow(User loggedInUser)
        {
            InitializeComponent();

            var reservationRepository = new ReservationRepository();
            var reservationService = new ReservationService(reservationRepository);
            var apartmentRepository = new ApartmentRepository();
            var hotelRepository = new HotelRepository();

            _viewModel = new GuestReservationsViewModel(
                reservationService,
                apartmentRepository,
                hotelRepository,
                loggedInUser);

            DataContext = _viewModel;
        }

        private void CancelReservationButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.CancelSelectedReservation();

            MessageBox.Show(
                message,
                "Otkazivanje rezervacije",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
