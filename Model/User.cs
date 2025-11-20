
using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Jmbg { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }

        public User() { }

        public User(string jmbg, string email, string password,
                    string firstName, string lastName,
                    string phoneNumber, UserType userType)
        {
            Jmbg = jmbg;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            UserType = userType;
        }

        public string[] ToCSV()
        {
            return new[]
            {
                Id.ToString(),
                Jmbg,
                Email,
                Password,
                FirstName,
                LastName,
                PhoneNumber,
                UserType.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Jmbg = values[1];
            Email = values[2];
            Password = values[3];
            FirstName = values[4];
            LastName = values[5];
            PhoneNumber = values[6];
            UserType = Enum.Parse<UserType>(values[7]);
        }
    }
}
