using Microsoft.AspNetCore.Mvc;
using Reservation.DTO;
using Reservation.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Controllers
{
    [ApiController]
    public class ReservationController : Controller
    {
        readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("api/v1/reservations")]
        public async Task<IActionResult> GetUserReservetions([FromHeader(Name = "X-User-Name"), Required] string xUserName)
        {
            if (string.IsNullOrWhiteSpace(xUserName))
            {
                return BadRequest();

            }

            var reservations = await _reservationService.GetUserReservations(xUserName);

            return Ok(reservations);
        }

        [HttpGet("api/v1/reservationsCount")]
        public async Task<IActionResult> GetUserReservetionsCount([FromHeader(Name = "X-User-Name"), Required] string xUserName)
        {
            if (string.IsNullOrWhiteSpace(xUserName))
            {
                return BadRequest();

            }

            var count = await _reservationService.GetUserReservationCount(xUserName);

            return Ok(count);
        }

        [HttpPost("api/v1/reservations")]
        public async Task<IActionResult> CreateReservation([FromHeader(Name = "X-User-Name"), Required] string xUserName,[FromBody, Required] RentRequest request)
        {
            if (string.IsNullOrWhiteSpace(xUserName))
            {
                return BadRequest();

            }

            var reservation = await _reservationService.CreateReservation(xUserName, Guid.Parse(request.bookUid), Guid.Parse(request.libraryUid), request.tillDate.ToDateTime(TimeOnly.MaxValue));

            if (reservation == null)
            {
                return BadRequest();
            }

            return Ok(reservation);
        }

        [HttpPost("api/v1/reservations/return")]
        public async Task<IActionResult> ReturnReservation([FromBody, Required] ReturnRequest request)
        {
            var reservation = await _reservationService.CloseReservation(Guid.Parse(request.reservationGuid), request.returnDate.ToDateTime(TimeOnly.MinValue));

            if (reservation == null)
            {
                return NotFound(new { message = string.Format("Бронь с Guid = {0}", request.reservationGuid) });
            }

            return Ok(reservation);
        }


        [HttpDelete("api/v1/reservations")]
        public async Task<IActionResult> DeleteReservation([FromBody, Required] ReturnRequest request)
        {
            await _reservationService.DeleteReservation(Guid.Parse(request.reservationGuid));
            return Ok();
        }





    }

}