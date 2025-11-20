using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class HotelSearchWindow : Window
    {
        private readonly HotelSearchViewModel _viewModel;

        public HotelSearchWindow()
        {
            InitializeComponent();

            var hotelRepository = new HotelRepository();
            var apartmentRepository = new ApartmentRepository();
            var hotelService = new HotelService(hotelRepository, apartmentRepository);

            _viewModel = new HotelSearchViewModel(hotelService);
            DataContext = _viewModel;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Search();
        }
    }
}
