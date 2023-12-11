namespace Reservation.DTO
{
    public class ReservationResponse
    {
        public Guid Reservation_uid { get; set; }
        public string UserName { get; set; }
        public Guid Library_uid { get; set; }
        public Guid Book_uid { get; set; }
        public string Status { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime Till_date { get; set; }
    }
}
