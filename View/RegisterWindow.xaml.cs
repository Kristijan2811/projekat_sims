using System.Windows;
using BookingApp.Repository;
using BookingApp.Service;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterWindow()
        {
            InitializeComponent();

            var userRepository = new UserRepository();
            var userService = new UserService(userRepository);

            _viewModel = new RegisterViewModel(userService);
            DataContext = _viewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // uzimamo vrednosti iz UI (posebno lozinku iz PasswordBox-a)
            _viewModel.Jmbg = JmbgTextBox.Text;
            _viewModel.FirstName = FirstNameTextBox.Text;
            _viewModel.LastName = LastNameTextBox.Text;
            _viewModel.PhoneNumber = PhoneTextBox.Text;
            _viewModel.Email = EmailTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            var user = _viewModel.TryRegister();

            if (user == null)
            {
                MessageBox.Show(
                    "Email ili lozinka vec postoje. Unesite druge vrednosti.",
                    "Registracija",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            MessageBox.Show(
                "Uspesna registracija! Sada se mozete prijaviti.",
                "Registracija",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            this.Close();
        }
    }
}
