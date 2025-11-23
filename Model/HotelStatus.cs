namespace BookingApp.Model
{
    public enum HotelStatus
    {
        Pending,    // na cekanju (admin ga uneo, vlasnik jos nije potvrdio)
        Approved,   // vlasnik potvrdio -> hotel vidljiv za goste
        Rejected    // vlasnik odbio (admin pogresno povezao)
    }
}
