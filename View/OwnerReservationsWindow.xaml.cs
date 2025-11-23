using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerReservationsWindow : Window
    {
        private readonly OwnerReservationsViewModel _viewModel;

        public OwnerReservationsWindow(User loggedInOwner)
        {
            InitializeComponent();

            var reservationRepository = new ReservationRepository();
            var reservationService = new ReservationService(reservationRepository);
            var apartmentRepository = new ApartmentRepository();
            var hotelRepository = new HotelRepository();
            var userRepository = new UserRepository();

            _viewModel = new OwnerReservationsViewModel(
                reservationService,
                apartmentRepository,
                hotelRepository,
                userRepository,
                loggedInOwner);

            DataContext = _viewModel;
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.ApproveSelectedReservation();

            MessageBox.Show(
                message,
                "Potvrda rezervacije",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            var message = _viewModel.RejectSelectedReservation();

            MessageBox.Show(
                message,
                "Odbijanje rezervacije",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
