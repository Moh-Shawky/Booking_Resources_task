using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test_menu.Data;
using Test_menu.Models;

namespace Test_menu.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bookings
                    .Include(b => b.Resource)
                    .ToListAsync());
            
        } 




        [HttpPost]
        public async Task<IActionResult> Book(int id, DateTime DateFrom, DateTime DateTo, int Quantity)
        {
            // Find the resource by ID, then create a new booking or handle accordingly
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            if(Quantity> resource.Quantity)
            {
                TempData["ErrorMessage"] = "Not enough quantity available for booking.";
                return RedirectToAction("Booking", "Resources", new { id = id });

            }
            if(DateFrom < DateTime.Now || DateTo <= DateTime.Now || DateTo == DateFrom)
            {
                TempData["ErrorMessage"] = "Date is not correct please try again";
                return RedirectToAction("Booking", "Resources", new { id = id });
            }

            var overlappingBookings = await _context.Bookings
                .Where(b => b.ResourceId == id &&
                 ((DateFrom >= b.DateFrom && DateFrom < b.DateTo) ||
                 (DateTo > b.DateFrom && DateTo <= b.DateTo) ||
                 (DateFrom <= b.DateFrom && DateTo >= b.DateTo)))
                .ToListAsync();

            if (overlappingBookings.Any())
            {
                TempData["ErrorMessage"] = "The selected date range overlaps with an existing booking.";
                return RedirectToAction("Booking", "Resources", new { id = id });
            }

            // Create a new booking object with provided details
            var booking = new Booking
            {
                ResourceId = id,
                DateFrom = DateFrom,
                DateTo = DateTo,
                BookedQuantity = Quantity
            };

            _context.Bookings.Add(booking);
            resource.Quantity = resource.Quantity - Quantity;
            await _context.SaveChangesAsync();
            Console.WriteLine($"EMAIL SENT TO admin@admin.com FOR CREATED BOOKING WITH ID {booking.Id}");
            TempData["SuccessMessage"] = "Your booking has been done successfuly";
            return RedirectToAction("Index", "Resources");
        }

 
        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.Include(b => b.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null && booking.DateFrom > DateTime.Now)
            {
                var resource = await _context.Resources.FindAsync(booking.ResourceId);
                if (resource != null)
                {
                    resource.Quantity = resource.Quantity + booking.BookedQuantity;
                }
                _context.Bookings.Remove(booking);
            }
            else
            {
                TempData["ErrorMessage"] = "The selected book date is already done.";

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
