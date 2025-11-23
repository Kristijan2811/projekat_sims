using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class AddApartmentWindow : Window
    {
        private readonly AddApartmentViewModel _viewModel;

        public AddApartmentWindow(User loggedInOwner)
        {
            InitializeComponent();

            var apartmentRepository = new ApartmentRepository();
            var hotelRepository = new HotelRepository();

            var apartmentService = new ApartmentService(apartmentRepository, hotelRepository);
            var hotelService = new HotelService(hotelRepository, apartmentRepository);

            _viewModel = new AddApartmentViewModel(apartmentService, hotelService, loggedInOwner);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Ako zelis, ovde mozes da 'osveziš' vrednosti iz UI, ali binding vec radi posao
            var message = _viewModel.TryCreateApartment();

            MessageBox.Show(
                message,
                "Unos apartmana",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
