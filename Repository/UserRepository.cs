using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class UserRepository
    {
        private const string FilePath = "../../../Resources/Data/users.csv";

        private readonly Serializer<User> _serializer;
        private List<User> _users;

        public UserRepository()
        {
            _serializer = new Serializer<User>();
            Load();
        }

        private void Load()
        {
            _users = _serializer.FromCSV(FilePath);
        }

        // NOVO: izdvojeno cuvanje u CSV
        private void Save()
        {
            _serializer.ToCSV(FilePath, _users);
        }

        public List<User> GetAll()
        {
            Load();
            return _users;
        }

        public User GetByEmail(string email)
        {
            Load();
            return _users.FirstOrDefault(u => u.Email == email);
        }

        // OVO TI REALNO NI NE TREBA, ali ako se negde koristi – ostavljam
        public User GetByPassword(string password)
        {
            Load();
            return _users.FirstOrDefault(u => u.Password == password);
        }

        // NOVO: pronalazak korisnika po JMBG-u
        public User GetByJmbg(string jmbg)
        {
            Load();
            return _users.FirstOrDefault(u => u.Jmbg == jmbg);
        }

        // NOVO: da li vec postoji korisnik sa tim JMBG ili email-om
        public bool ExistsByJmbgOrEmail(string jmbg, string email)
        {
            Load();
            return _users.Any(u => u.Jmbg == jmbg || u.Email == email);
        }

        // racunanje sledeceg Id-a
        private int NextId()
        {
            Load();

            if (_users.Count == 0)
            {
                return 1;
            }

            return _users.Max(u => u.Id) + 1;
        }

        // dodavanje novog korisnika i cuvanje u CSV
        public User Add(User user)
        {
            Load();

            user.Id = NextId();
            _users.Add(user);

            Save();    // sad koristimo zajednicku Save() metodu

            return user;
        }
    }
}
