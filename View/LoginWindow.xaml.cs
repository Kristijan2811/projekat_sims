using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            // 1. Repository
            var userRepository = new UserRepository();

            // 2. Service
            var userService = new UserService(userRepository);

            // 3. ViewModel
            _viewModel = new LoginViewModel(userService);

            // 4. Povezivanje View <- ViewModel
            DataContext = _viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Email moze iz bindinga, ali ovako si siguran:
            _viewModel.Email = EmailTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            var user = _viewModel.TryLogin();

            if (user == null)
            {
                MessageBox.Show("Pogresan email ili lozinka.", "Prijava",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(
                $"Uspesna prijava: {user.FirstName} {user.LastName} ({user.UserType})",
                "Prijava",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: ovde ces kasnije da otvoris prozor za Administratora/Gosta/Vlasnika
            // npr:
            // if (user.UserType == UserType.Administrator) { ... }
        }
    }
}
