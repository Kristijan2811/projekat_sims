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

        // ➜ NOVO: pronalazak korisnika po lozinci
        public User GetByPassword(string password)
        {
            Load();
            return _users.FirstOrDefault(u => u.Password == password);
        }

        // ➜ NOVO: racunanje sledeceg Id-a
        private int NextId()
        {
            Load();

            if (_users.Count == 0)
            {
                return 1;
            }

            return _users.Max(u => u.Id) + 1;
        }

        // ➜ NOVO: dodavanje novog korisnika i cuvanje u CSV
        public User Add(User user)
        {
            Load();

            user.Id = NextId();
            _users.Add(user);

            _serializer.ToCSV(FilePath, _users);

            return user;
        }
    }
}
