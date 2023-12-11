namespace Reservation.DTO
{
    public class RentRequest
    {
        public string bookUid { get; set; }
        public string libraryUid { get; set; }
        public DateOnly tillDate { get; set; }
    }
}
