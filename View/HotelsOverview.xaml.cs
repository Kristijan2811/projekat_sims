using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class HotelsOverview : Window
    {
        public HotelsOverview()
        {
            InitializeComponent();

            var hotelRepository = new HotelRepository();
            var apartmentRepository = new ApartmentRepository();
            var hotelService = new HotelService(hotelRepository, apartmentRepository);

            var viewModel = new HotelsOverviewViewModel(hotelService);
            DataContext = viewModel;
        }
    }
}
