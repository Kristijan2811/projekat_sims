using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class OwnerRegisterViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;

        public string Jmbg { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public OwnerRegisterViewModel(UserService userService)
        {
            _userService = userService;
        }

        public User TryRegisterOwner()
        {
            // minimalne provere, mozes da siris po zelji
            if (string.IsNullOrWhiteSpace(Jmbg) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                return null;
            }

            // VAŽNO: redosled argumenata mora da prati potpis u UserService.RegisterOwner
            return _userService.RegisterOwner(
                Jmbg,        // jmbg
                Email,       // email
                Password,    // password
                FirstName,   // firstName
                LastName,    // lastName
                PhoneNumber  // phoneNumber
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
