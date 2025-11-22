using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class ReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/reservations.csv";

        private readonly Serializer<Reservation> _serializer;
        private List<Reservation> _reservations;

        public ReservationRepository()
        {
            _serializer = new Serializer<Reservation>();
            Load();
        }

        private void Load()
        {
            _reservations = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _reservations);
        }

        public List<Reservation> GetAll()
        {
            Load();
            return _reservations;
        }

        public List<Reservation> GetByApartmentAndDate(int apartmentId, DateTime date)
        {
            Load();
            var day = date.Date;

            return _reservations
                .Where(r => r.ApartmentId == apartmentId && r.Date.Date == day)
                .ToList();
        }

        private int NextId()
        {
            Load();
            return _reservations.Count == 0 ? 1 : _reservations.Max(r => r.Id) + 1;
        }

        public Reservation Add(Reservation reservation)
        {
            Load();

            reservation.Id = NextId();
            _reservations.Add(reservation);

            Save();
            return reservation;
        }

        public void Update(Reservation reservation)
        {
            Load();

            var index = _reservations.FindIndex(r => r.Id == reservation.Id);
            if (index != -1)
            {
                _reservations[index] = reservation;
                Save();
            }
        }

        // NOVO: brisanje rezervacije po Id-ju
        public void Delete(int id)
        {
            Load();

            var existing = _reservations.FirstOrDefault(r => r.Id == id);
            if (existing != null)
            {
                _reservations.Remove(existing);
                Save();
            }
        }
    }
}
