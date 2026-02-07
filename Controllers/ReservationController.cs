using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShopReservation.Data;
using BarberShopReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberShopReservation.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(ApplicationDbContext context, ILogger<ReservationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? serviceId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var reservations = _context.Reservations
                .Include(r => r.Service)
                .Include(r => r.TimeSlot)
                .Where(r => r.UserId == userId);

            if (serviceId.HasValue)
            {
                reservations = reservations.Where(r => r.ServiceId == serviceId);
            }

            return View(await reservations.ToListAsync());
        }

        public async Task<IActionResult> Create(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return NotFound();

            var timeSlots = await _context.TimeSlots
                .Where(t => t.ServiceId == serviceId && t.IsAvailable)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            ViewBag.Service = service;
            ViewBag.TimeSlots = timeSlots;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId, int timeSlotId, string? notes)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);
            if (timeSlot == null || !timeSlot.IsAvailable)
                return BadRequest("Selected time slot is not available");

            var reservation = new Reservation
            {
                ServiceId = serviceId,
                UserId = userId,
                TimeSlotId = timeSlotId,
                Status = "Pending",
                Notes = notes
            };

            _context.Add(reservation);
            timeSlot.IsAvailable = false;
            _context.Update(timeSlot);
            
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return BadRequest("Error creating reservation");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Service)
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (reservation.UserId != userId)
                return Forbid();

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? notes)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (reservation.UserId != userId)
                return Forbid();

            reservation.Notes = notes;
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (reservation.UserId != userId)
                return Forbid();

            if (reservation.Status == "Cancelled")
                return BadRequest("Reservation is already cancelled");

            reservation.Status = "Cancelled";
            if (reservation.TimeSlot != null)
                reservation.TimeSlot.IsAvailable = true;

            _context.Update(reservation);
            if (reservation.TimeSlot != null)
                _context.Update(reservation.TimeSlot);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
