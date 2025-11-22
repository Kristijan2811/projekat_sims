using System;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public enum ReservationStatus
    {
        Pending,    // poslat zahtev, ceka vlasnika
        Approved,   // vlasnik odobrio
        Rejected    // vlasnik odbio
    }

    public class Reservation : ISerializable
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public int GuestId { get; set; }
        public DateTime Date { get; set; }
        public ReservationStatus Status { get; set; }

        // NOVO: razlog odbijanja (koristi se kad je Status = Rejected)
        public string RejectionReason { get; set; } = string.Empty;

        public Reservation() { }

        public string[] ToCSV()
        {
            return new[]
            {
                Id.ToString(),
                ApartmentId.ToString(),
                GuestId.ToString(),
                Date.ToString("yyyy-MM-dd"),
                Status.ToString(),
                RejectionReason
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ApartmentId = Convert.ToInt32(values[1]);
            GuestId = Convert.ToInt32(values[2]);
            Date = DateTime.Parse(values[3]);
            Status = Enum.Parse<ReservationStatus>(values[4]);

            // zbog starih redova koji nemaju razlog (samo 5 kolona)
            if (values.Length >= 6)
            {
                RejectionReason = values[5];
            }
            else
            {
                RejectionReason = string.Empty;
            }
        }
    }
}
