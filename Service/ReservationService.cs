using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Service
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository;

        public ReservationService(ReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        // NOVO: sve rezervacije (za vlasnika, admina itd.)
        public List<Reservation> GetAll()
        {
            return _reservationRepository.GetAll();
        }

        // Provera da li je apartman slobodan za dati datum
        // Po zadatku: ne dozvoli ako je apartman ZAUZET (Approved) tog dana.
        public bool IsApartmentAvailable(int apartmentId, DateTime date)
        {
            var reservations = _reservationRepository.GetByApartmentAndDate(apartmentId, date);

            // zauzet ako postoji makar jedna odobrena rezervacija
            return !reservations.Any(r => r.Status == ReservationStatus.Approved);
        }

        // Kreiranje nove rezervacije (Pending) ako je apartman slobodan
        public Reservation CreateReservation(int apartmentId, int guestId, DateTime date)
        {
            if (!IsApartmentAvailable(apartmentId, date))
            {
                // apartman je zauzet -> ne saljemo zahtev vlasniku
                return null;
            }

            var reservation = new Reservation
            {
                ApartmentId = apartmentId,
                GuestId = guestId,
                Date = date.Date,
                Status = ReservationStatus.Pending
            };

            return _reservationRepository.Add(reservation);
        }

        public List<Reservation> GetAllForGuest(int guestId)
        {
            return _reservationRepository
                .GetAll()
                .Where(r => r.GuestId == guestId)
                .ToList();
        }

        // ================== FILTRIRANJE ZA GOSTA (MOJE REZERVACIJE) ==================

        // Gostove rezervacije + opcioni filter po statusu (null = svi)
        public List<Reservation> GetAllForGuestFiltered(int guestId, ReservationStatus? statusFilter)
        {
            var query = _reservationRepository
                .GetAll()
                .Where(r => r.GuestId == guestId);

            if (statusFilter.HasValue)
            {
                query = query.Where(r => r.Status == statusFilter.Value);
            }

            return query
                .OrderBy(r => r.Date)
                .ToList();
        }

        // ================== OTKAZIVANJE (GOST) ==================

        // Gost moze da otkaze samo svoje rezervacije koje su Pending ili Approved
        public bool CancelReservation(int reservationId, int guestId)
        {
            var all = _reservationRepository.GetAll();
            var reservation = all
                .FirstOrDefault(r => r.Id == reservationId && r.GuestId == guestId);

            if (reservation == null)
            {
                return false;
            }

            if (reservation.Status != ReservationStatus.Pending &&
                reservation.Status != ReservationStatus.Approved)
            {
                // ne dozvoljavamo otkazivanje Rejected (ili nekog drugog statusa)
                return false;
            }

            _reservationRepository.Delete(reservation.Id);
            return true;
        }

        // ================== ODOBRAVANJE / ODBIJANJE (VLASNIK) ==================

        public bool ApproveReservation(int reservationId)
        {
            var all = _reservationRepository.GetAll();
            var reservation = all.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return false;
            }

            reservation.Status = ReservationStatus.Approved;
            reservation.RejectionReason = string.Empty; // ocisti razlog ako je postojao

            _reservationRepository.Update(reservation);
            return true;
        }

        public bool RejectReservation(int reservationId, string rejectionReason)
        {
            var all = _reservationRepository.GetAll();
            var reservation = all.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return false;
            }

            reservation.Status = ReservationStatus.Rejected;
            reservation.RejectionReason = rejectionReason ?? string.Empty;

            _reservationRepository.Update(reservation);
            return true;
        }

        // ================== REZERVACIJA NA VISE DANA ==================

        // Provera da li je apartman slobodan u opsegu datuma [startDate, endDate]
        public bool IsApartmentAvailableForRange(int apartmentId, DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            if (end < start)
            {
                return false;
            }

            var reservations = _reservationRepository
                .GetAll()
                .Where(r =>
                    r.ApartmentId == apartmentId &&
                    r.Date.Date >= start &&
                    r.Date.Date <= end &&
                    r.Status == ReservationStatus.Approved)   // zauzet = Approved u opsegu
                .ToList();

            // slobodan ako nema nijednu Approved rezervaciju u tom periodu
            return !reservations.Any();
        }

        // Kreiranje rezervacije na vise dana (vise jednodnevnih rezervacija)
        public List<Reservation> CreateReservationForRange(
            int apartmentId,
            int guestId,
            DateTime startDate,
            DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            if (end < start)
            {
                return null;
            }

            // ako je bilo koji dan zauzet (Approved) -> ne kreiramo nista
            if (!IsApartmentAvailableForRange(apartmentId, start, end))
            {
                return null;
            }

            var createdReservations = new List<Reservation>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var reservation = new Reservation
                {
                    ApartmentId = apartmentId,
                    GuestId = guestId,
                    Date = date,
                    Status = ReservationStatus.Pending
                };

                createdReservations.Add(_reservationRepository.Add(reservation));
            }

            return createdReservations;
        }
    }
}
