using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerRegisterWindow : Window
    {
        private readonly OwnerRegisterViewModel _viewModel;

        public OwnerRegisterWindow()
        {
            InitializeComponent();

            var userRepository = new UserRepository();
            var userService = new UserService(userRepository);

            _viewModel = new OwnerRegisterViewModel(userService);
            DataContext = _viewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // lozinka iz PasswordBox-a
            _viewModel.Jmbg = JmbgTextBox.Text;
            _viewModel.FirstName = FirstNameTextBox.Text;
            _viewModel.LastName = LastNameTextBox.Text;
            _viewModel.PhoneNumber = PhoneTextBox.Text;
            _viewModel.Email = EmailTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            var user = _viewModel.TryRegisterOwner();

            if (user == null)
            {
                MessageBox.Show(
                    "Neuspesna registracija. Proverite da li JMBG ili email vec postoje ili da li su sva polja popunjena.",
                    "Registracija vlasnika",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            MessageBox.Show(
                "Vlasnik je uspesno registrovan.",
                "Registracija vlasnika",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            this.Close();
        }
    }
}
