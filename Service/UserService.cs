using System.Collections.Generic;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        // PRIJAVA NA SISTEM
        //tacka 1.1
        public User Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
            {
                // nema korisnika sa tim email-om
                return null;
            }

            if (user.Password != password)
            {
                // lozinka pogresna
                return null;
            }

            // uspešna prijava
            return user;
        }
    }
}
