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
        // tacka 1.1
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

            // uspesna prijava
            return user;
        }

        // ================== REGISTRACIJA GOSTA ==================

        // Provera da li je email jedinstven
        public bool IsEmailUnique(string email)
        {
            return _userRepository.GetByEmail(email) == null;
        }

        // Provera da li je lozinka jedinstvena
        public bool IsPasswordUnique(string password)
        {
            return _userRepository.GetByPassword(password) == null;
        }

        // Registracija novog gosta
        public User RegisterGuest(
            string jmbg,
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber)
        {
            // pravilo iz zadatka za GOSTA:
            // email i lozinka moraju biti JEDINSTVENI
            if (!IsEmailUnique(email) || !IsPasswordUnique(password))
            {
                return null;    // ViewModel/UI ce prikazati poruku o gresci
            }

            var user = new User
            {
                Jmbg = jmbg,
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                UserType = UserType.Guest
            };

            return _userRepository.Add(user);
        }

        // ================== REGISTRACIJA VLASNIKA (ADMIN) ==================

        // pomozna metoda: da li postoji korisnik sa tim JMBG ili email-om
        public bool IsJmbgOrEmailTaken(string jmbg, string email)
        {
            return _userRepository.ExistsByJmbgOrEmail(jmbg, email);
        }

        // Registracija vlasnika (poziva je administrator)
        // pravilo iz zadatka: spreciti registraciju sa vec postojecim JMBG-om ili email-om
        public User RegisterOwner(
            string jmbg,
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber)
        {
            // JMBG ili email vec postoje -> ne dozvoljavamo registraciju
            if (IsJmbgOrEmailTaken(jmbg, email))
            {
                return null;
            }

            var user = new User
            {
                Jmbg = jmbg,
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                UserType = UserType.Owner
            };

            return _userRepository.Add(user);
        }
    }
}
