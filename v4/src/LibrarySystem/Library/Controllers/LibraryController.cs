using Library.DTO;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    public class LibraryController : Controller
    {
        readonly ILibraryService _libraryService;


        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet("api/v1/libraries")]
        public async Task<IActionResult> GetCityLibraries([FromQuery, Required] string city, [FromQuery] int? page, [FromQuery] int? size)
        {
            var availableLibraries = await _libraryService.GetCityLibraries(page, size, city);

            return Ok(availableLibraries);
        }

        [HttpGet("/api/v1/{libraryUid}/books")]
        public async Task<IActionResult> GetLibraryBooks([FromRoute] string libraryUid, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] bool allShow = false)
        {
            var books = await _libraryService.GetLibraryBooks(page, size, Guid.Parse(libraryUid), allShow);

            return Ok(books);
        }

        [HttpGet("/api/v1/library/{libraryUid}")]
        public async Task<IActionResult> GetLibraryByGuid([FromRoute] string libraryUid)
        {
            var library = await _libraryService.GetLibraryByGuid(Guid.Parse(libraryUid));

            return Ok(library);
        }

        [HttpGet("/api/v1/library")]
        public async Task<IActionResult> GetLibraryById([FromQuery, Required] int libraryId)
        {
            var library = await _libraryService.GetLibraryById(libraryId);

            return Ok(library);
        }

        [HttpGet("/api/v1/book/{bookUid}")]
        public async Task<IActionResult> GetBookByGuid([FromRoute] string bookUid)
        {
            var book = await _libraryService.GetBookByGuid(Guid.Parse(bookUid));

            return Ok(book);
        }

        [HttpGet("/api/v1/book")]
        public async Task<IActionResult> GetBookById([FromQuery, Required] int bookId)
        {
            var book = await _libraryService.GetBookById(bookId);

            return Ok(book);
        }

        [HttpGet("/api/v1/library/checkBookAvailable")]
        public async Task<IActionResult> CheckLibraryBookAvailable([FromQuery, Required] string libraryUid, [FromQuery, Required] string bookUid)
        {
            var check = await _libraryService.CheckLibraryBookCount(Guid.Parse(bookUid), Guid.Parse(libraryUid));

            return Ok(check);
        }

        [HttpPost("/api/v1/library/rentBook")]
        public async Task<IActionResult> RentBookAction([FromBody, Required] RentRequest request, [FromQuery, Required] bool action)
        {
            var check = await _libraryService.RentBookAsync(Guid.Parse(request.bookUid), Guid.Parse(request.libraryUid), action);

            if (check)
                return Ok();
            else
                return BadRequest();
        }



    }
}