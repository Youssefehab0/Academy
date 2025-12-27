using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Academy.Infrastructure.Data;
using Academy.Domain.Entities;
using Academy.Domain.Enums;
using Academy.Infrastructure.Dto;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminBookingController : ControllerBase
    {
        private readonly AcademyDbContext _context;

        public AdminBookingController(AcademyDbContext context)
        {
            _context = context;
        }

        // GET: api/adminbooking/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Course).ThenInclude(c => c.Instructor)
                .Include(b => b.Payment)
                .Select(b => new BookingReadDto
                {
                    Id = b.Id,
                    Course = new CourseReadDto
                    {
                        Id = b.Course.Id,
                        NameEn = b.Course.NameEn,
                        NameAr = b.Course.NameAr,
                        DescriptionEn = b.Course.DescriptionEn,
                        DescriptionAr = b.Course.DescriptionAr,
                        Price = b.Course.Price,
                        Category = b.Course.Category,
                        Level = b.Course.Level,
                        Duration = b.Course.Duration,
                        Instructor = new InstructorReadDto
                        {
                            Id = b.Course.Instructor.Id,
                            Name = b.Course.Instructor.Name,
                            Bio = b.Course.Instructor.Bio,
                            Skills = b.Course.Instructor.Skills,
                            PhotoUrl = b.Course.Instructor.PhotoUrl
                        }
                    },
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    CancelledAt = b.CancelledAt,
                    CancellationAllowedUntil = b.CancellationAllowedUntil,
                    HasManualCancelRequest = b.HasManualCancelRequest,
                    Payment = b.Payment == null ? null : new PaymentReadDto
                    {
                        Id = b.Payment.Id,
                        Method = b.Payment.Method,
                        ReferenceNumber = b.Payment.ReferenceNumber,
                        ScreenshotUrl = b.Payment.ScreenshotUrl,
                        Status = b.Payment.Status,
                        CreatedAt = b.Payment.CreatedAt
                    }
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // PUT: api/adminbooking/bookings/{id}/approve
        [HttpPut("bookings/{id}/approve")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound("Booking not found.");

            booking.Status = BookingStatus.Approved;

            // إذا فيه دفع مرتبط، نقدر نغير حالة الدفع كـ Verified تلقائي
            if (booking.Payment != null)
                booking.Payment.Status = PaymentStatus.Verified;

            await _context.SaveChangesAsync();

            // return updated booking as DTO
            var updated = await _context.Bookings
                .Where(b => b.Id == id)
                .Include(b => b.Student)
                .Include(b => b.Course).ThenInclude(c => c.Instructor)
                .Include(b => b.Payment)
                .Select(b => new BookingReadDto
                {
                    Id = b.Id,
                    Course = new CourseReadDto
                    {
                        Id = b.Course.Id,
                        NameEn = b.Course.NameEn,
                        NameAr = b.Course.NameAr,
                        DescriptionEn = b.Course.DescriptionEn,
                        DescriptionAr = b.Course.DescriptionAr,
                        Price = b.Course.Price,
                        Category = b.Course.Category,
                        Level = b.Course.Level,
                        Duration = b.Course.Duration,
                        Instructor = new InstructorReadDto
                        {
                            Id = b.Course.Instructor.Id,
                            Name = b.Course.Instructor.Name,
                            Bio = b.Course.Instructor.Bio,
                            Skills = b.Course.Instructor.Skills,
                            PhotoUrl = b.Course.Instructor.PhotoUrl
                        }
                    },
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    CancelledAt = b.CancelledAt,
                    CancellationAllowedUntil = b.CancellationAllowedUntil,
                    HasManualCancelRequest = b.HasManualCancelRequest,
                    Payment = b.Payment == null ? null : new PaymentReadDto
                    {
                        Id = b.Payment.Id,
                        Method = b.Payment.Method,
                        ReferenceNumber = b.Payment.ReferenceNumber,
                        ScreenshotUrl = b.Payment.ScreenshotUrl,
                        Status = b.Payment.Status,
                        CreatedAt = b.Payment.CreatedAt
                    }
                })
                .FirstOrDefaultAsync();

            return Ok(updated);
        }

        // PUT: api/adminbooking/bookings/{id}/reject
        [HttpPut("bookings/{id}/reject")]
        public async Task<IActionResult> RejectBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound("Booking not found.");

            booking.Status = BookingStatus.Rejected;

            if (booking.Payment != null)
                booking.Payment.Status = PaymentStatus.Rejected;

            await _context.SaveChangesAsync();

            var updated = await _context.Bookings
                .Where(b => b.Id == id)
                .Include(b => b.Student)
                .Include(b => b.Course).ThenInclude(c => c.Instructor)
                .Include(b => b.Payment)
                .Select(b => new BookingReadDto
                {
                    Id = b.Id,
                    Course = new CourseReadDto
                    {
                        Id = b.Course.Id,
                        NameEn = b.Course.NameEn,
                        NameAr = b.Course.NameAr,
                        DescriptionEn = b.Course.DescriptionEn,
                        DescriptionAr = b.Course.DescriptionAr,
                        Price = b.Course.Price,
                        Category = b.Course.Category,
                        Level = b.Course.Level,
                        Duration = b.Course.Duration,
                        Instructor = new InstructorReadDto
                        {
                            Id = b.Course.Instructor.Id,
                            Name = b.Course.Instructor.Name,
                            Bio = b.Course.Instructor.Bio,
                            Skills = b.Course.Instructor.Skills,
                            PhotoUrl = b.Course.Instructor.PhotoUrl
                        }
                    },
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    CancelledAt = b.CancelledAt,
                    CancellationAllowedUntil = b.CancellationAllowedUntil,
                    HasManualCancelRequest = b.HasManualCancelRequest,
                    Payment = b.Payment == null ? null : new PaymentReadDto
                    {
                        Id = b.Payment.Id,
                        Method = b.Payment.Method,
                        ReferenceNumber = b.Payment.ReferenceNumber,
                        ScreenshotUrl = b.Payment.ScreenshotUrl,
                        Status = b.Payment.Status,
                        CreatedAt = b.Payment.CreatedAt
                    }
                })
                .FirstOrDefaultAsync();

            return Ok(updated);
        }

        // PUT: api/adminbooking/payments/{id}/confirm
        [HttpPut("payments/{id}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var payment = await _context.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return NotFound("Payment not found.");

            payment.Status = PaymentStatus.Verified;
            payment.Booking.Status = BookingStatus.Approved;

            await _context.SaveChangesAsync();

            var dto = new PaymentReadDto
            {
                Id = payment.Id,
                Method = payment.Method,
                ReferenceNumber = payment.ReferenceNumber,
                ScreenshotUrl = payment.ScreenshotUrl,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };

            return Ok(dto);
        }

        // PUT: api/adminbooking/payments/{id}/reject
        [HttpPut("payments/{id}/reject")]
        public async Task<IActionResult> RejectPayment(int id)
        {
            var payment = await _context.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return NotFound("Payment not found.");

            payment.Status = PaymentStatus.Rejected;
            payment.Booking.Status = BookingStatus.Rejected;

            await _context.SaveChangesAsync();

            var dto = new PaymentReadDto
            {
                Id = payment.Id,
                Method = payment.Method,
                ReferenceNumber = payment.ReferenceNumber,
                ScreenshotUrl = payment.ScreenshotUrl,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };

            return Ok(dto);
        }
    }
}
