using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class RegisterViewModel
    {
        private readonly UserService _userService;

        public string Jmbg { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public RegisterViewModel(UserService userService)
        {
            _userService = userService;
        }

        public User TryRegister()
        {
            // ovde eventualno mozes dodati jos validacija (prazna polja itd.)
            return _userService.RegisterGuest(
                Jmbg,
                Email,
                Password,
                FirstName,
                LastName,
                PhoneNumber);
        }
    }
}
