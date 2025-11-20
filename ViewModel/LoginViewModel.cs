using BookingApp.Model;
using BookingApp.Service;

namespace BookingApp.ViewModel
{
    public class LoginViewModel
    {
        private readonly UserService _userService;

        public string Email { get; set; }
        public string Password { get; set; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
        }

        public User TryLogin()
        {
            return _userService.Login(Email, Password);
        }
    }
}
