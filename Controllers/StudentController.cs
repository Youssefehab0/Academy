using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Academy.Infrastructure.Data;
using Academy.Domain.Entities;
using Academy.Domain.Enums;
using System.Security.Claims;
using Academy.Infrastructure.Dto;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly AcademyDbContext _context;

        public StudentController(AcademyDbContext context)
        {
            _context = context;
        }

        // Helper: Get current student id from JWT
        private int GetCurrentStudentId()
        {
            return int.Parse(User.Claims.First(c => c.Type == "Id").Value);
        }

        // ================== Courses ==================
        // GET: api/student/courses
        [HttpGet("courses")]
        [AllowAnonymous] // Optional: allow browsing without login
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Select(c => new CourseReadDto
                {
                    Id = c.Id,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    DescriptionEn = c.DescriptionEn,
                    DescriptionAr = c.DescriptionAr,
                    Price = c.Price,
                    Category = c.Category,
                    Level = c.Level,
                    Duration = c.Duration,
                    Instructor = new InstructorReadDto
                    {
                        Id = c.Instructor.Id,
                        Name = c.Instructor.Name,
                        Bio = c.Instructor.Bio,
                        Skills = c.Instructor.Skills,
                        PhotoUrl = c.Instructor.PhotoUrl
                    }
                })
                .ToListAsync();
            return Ok(courses);
        }

        // GET: api/student/courses/{id}
        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.Id == id)
                .Select(c => new CourseReadDto
                {
                    Id = c.Id,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    DescriptionEn = c.DescriptionEn,
                    DescriptionAr = c.DescriptionAr,
                    Price = c.Price,
                    Category = c.Category,
                    Level = c.Level,
                    Duration = c.Duration,
                    Instructor = new InstructorReadDto
                    {
                        Id = c.Instructor.Id,
                        Name = c.Instructor.Name,
                        Bio = c.Instructor.Bio,
                        Skills = c.Instructor.Skills,
                        PhotoUrl = c.Instructor.PhotoUrl
                    }
                })
                .FirstOrDefaultAsync();
            if (course == null) return NotFound();
            return Ok(course);
        }

        // ================== Bookings ==================
        // POST: api/student/bookings
        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            var studentId = GetCurrentStudentId();

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null) return NotFound("Course not found.");

            var booking = new Booking
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                Status = BookingStatus.PendingApproval,
                CreatedAt = DateTime.UtcNow,
                CancellationAllowedUntil = DateTime.UtcNow.AddDays(7), // Cancellation window
                HasManualCancelRequest = false
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // return booked DTO
            var bookingDto = await _context.Bookings
                .Where(b => b.Id == booking.Id)
                .Include(b => b.Course)
                .ThenInclude(c => c.Instructor)
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
                    Payment = null
                })
                .FirstOrDefaultAsync();

            return Ok(bookingDto);
        }

        // GET: api/student/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var studentId = GetCurrentStudentId();
            var bookings = await _context.Bookings
                .Where(b => b.StudentId == studentId)
                .Include(b => b.Course)
                .ThenInclude(c => c.Instructor)
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

        // PUT: api/student/bookings/{id}/cancel
        [HttpPut("bookings/{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var studentId = GetCurrentStudentId();
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null || booking.StudentId != studentId)
                return NotFound("Booking not found.");

            if (booking.Status == BookingStatus.Cancelled)
                return BadRequest("Booking already cancelled.");

            if (DateTime.UtcNow <= booking.CancellationAllowedUntil)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;
            }
            else
            {
                booking.HasManualCancelRequest = true; // needs admin approval
            }

            await _context.SaveChangesAsync();

            var bookingDto = await _context.Bookings
                .Where(b => b.Id == id)
                .Include(b => b.Course)
                .ThenInclude(c => c.Instructor)
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

            return Ok(bookingDto);
        }

        // ================== Payment ==================
        // POST: api/student/bookings/{id}/payment
        [HttpPost("bookings/{id}/payment")]
        public async Task<IActionResult> SubmitPayment(int id, [FromForm] PaymentDto dto)
        {
            var studentId = GetCurrentStudentId();
            var booking = await _context.Bookings.Include(b => b.Payment).FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.StudentId != studentId)
                return NotFound("Booking not found.");

            if (booking.Payment != null)
                return BadRequest("Payment already submitted.");

            string? screenshotUrl = null;
            if (dto.Screenshot != null)
            {
                var fileName = $"payment_{Guid.NewGuid()}{Path.GetExtension(dto.Screenshot.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Screenshot.CopyToAsync(stream);
                }
                screenshotUrl = $"/Uploads/{fileName}";
            }

            var payment = new Payment
            {
                BookingId = id,
                Method = dto.Method,
                ReferenceNumber = dto.ReferenceNumber,
                ScreenshotUrl = screenshotUrl,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var paymentDto = new PaymentReadDto
            {
                Id = payment.Id,
                Method = payment.Method,
                ReferenceNumber = payment.ReferenceNumber,
                ScreenshotUrl = payment.ScreenshotUrl,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };

            return Ok(paymentDto);
        }
    }
}
