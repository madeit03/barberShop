using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShopReservation.Data;
using BarberShopReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberShopReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalReservations = await _context.Reservations.CountAsync();
            var pendingReservations = await _context.Reservations
                .Where(r => r.Status == "Pending")
                .CountAsync();
            var approvedReservations = await _context.Reservations
                .Where(r => r.Status == "Approved")
                .CountAsync();
            var totalServices = await _context.Services.CountAsync();

            ViewBag.TotalReservations = totalReservations;
            ViewBag.PendingReservations = pendingReservations;
            ViewBag.ApprovedReservations = approvedReservations;
            ViewBag.TotalServices = totalServices;

            var recentReservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Service)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(recentReservations);
        }

        // Services Management
        public async Task<IActionResult> Services()
        {
            return View(await _context.Services.ToListAsync());
        }

        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Services));
            }
            return View(service);
        }

        public async Task<IActionResult> EditService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Services));
            }
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Services));
        }

        // TimeSlots Management
        public async Task<IActionResult> TimeSlots(int? serviceId)
        {
            var timeSlots = _context.TimeSlots
                .Include(t => t.Service)
                .AsQueryable();

            if (serviceId.HasValue)
            {
                timeSlots = timeSlots.Where(t => t.ServiceId == serviceId);
            }

            return View(await timeSlots.ToListAsync());
        }

        public async Task<IActionResult> CreateTimeSlot()
        {
            ViewBag.Services = await _context.Services.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTimeSlot(TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                timeSlot.EndTime = timeSlot.StartTime.AddMinutes(
                    (await _context.Services.FindAsync(timeSlot.ServiceId))?.DurationMinutes ?? 30
                );
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TimeSlots));
            }
            ViewBag.Services = await _context.Services.ToListAsync();
            return View(timeSlot);
        }

        // Reservations Management
        public async Task<IActionResult> Reservations(string? status)
        {
            var reservations = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Service)
                .Include(r => r.TimeSlot)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations.Where(r => r.Status == status);
            }

            return View(await reservations.OrderByDescending(r => r.CreatedAt).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Approved";
                reservation.ApprovedAt = DateTime.UtcNow;
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Reservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                if (reservation.TimeSlot != null)
                    reservation.TimeSlot.IsAvailable = true;

                _context.Update(reservation);
                if (reservation.TimeSlot != null)
                    _context.Update(reservation.TimeSlot);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Reservations));
        }
    }
}
