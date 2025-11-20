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

        // AKO HOCES, mozes da imas i GetAll kasnije
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
    }
}
