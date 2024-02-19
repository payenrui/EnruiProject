using Enrui_Project1.Data;
using Enrui_Project1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enrui_Project1.Controllers
{
    [Authorize]
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Bookings);
        }

        [HttpGet("{id}")]
        public IActionResult GetByBookingID(int? id)
        {
            var bookings = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (bookings == null)
                return Problem(detail: "Booking with BookingID" + id + "is not found", statusCode: 404);

            return Ok(bookings);
        }
        [HttpGet("{BookingStatus}")]
        public IActionResult GetByBookingStatus(string? BookingStatus = "All")
        {
            switch (BookingStatus.ToLower())
            {
                case "all":
                    return Ok(_context.Bookings);
                case "Pending":
                    return Ok(_context.Bookings.Where(e => e.BookingStatus.ToLower() == "Pending"));
                case "Confirmed":
                    return Ok(_context.Bookings.Where(e => e.BookingStatus.ToLower() == "Confirmed"));
                case "Terminated":
                    return Ok(_context.Bookings.Where(e => e.BookingStatus.ToLower() == "Terminated"));
                default:
                    return Problem(detail: "Booking with bookingstatus" + BookingStatus + "is not found.", statusCode: 404);

            }
        }
        [HttpPost]
        public IActionResult Post(Bookings booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return CreatedAtAction("GetAll", new { id = booking.BookingID }, booking);
        }

        [HttpPut]
        public IActionResult Put(int? id, Bookings booking)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Employee with bookingid" + id + "is not found", statusCode: 404);

            entity.BookingStatus = booking.BookingStatus;
            entity.FacilityDescription = booking.FacilityDescription;
            entity.BookingDateTo = booking.BookingDateTo;
            entity.BookingDateFrom = booking.BookingDateFrom;
            entity.BookedBy = booking.BookedBy;

            _context.SaveChanges();
            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete(int? id, Bookings booking)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with BookingID" + id + "is not found", statusCode: 404);

            _context.Bookings.Remove(entity);
            _context.SaveChanges();

            return Ok(entity);
        }
    }
}
