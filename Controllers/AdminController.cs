using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Academy.Infrastructure.Data;
using Academy.Domain.Entities;
using Academy.Infrastructure.Dto;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AcademyDbContext _context;

        public AdminController(AcademyDbContext context)
        {
            _context = context;
        }

        // ================== Courses ==================

        // GET: api/admin/courses
        [HttpGet("courses")]
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

        // GET: api/admin/courses/{id}
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

        // POST: api/admin/courses
        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
        {
            var course = new Course
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                DescriptionEn = dto.DescriptionEn,
                DescriptionAr = dto.DescriptionAr,
                Price = dto.Price,
                Category = dto.Category,
                Level = dto.Level,
                Duration = dto.Duration,
                InstructorId = dto.InstructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(course);
        }

        // PUT: api/admin/courses/{id}
        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.NameEn = dto.NameEn;
            course.NameAr = dto.NameAr;
            course.DescriptionEn = dto.DescriptionEn;
            course.DescriptionAr = dto.DescriptionAr;
            course.Price = dto.Price;
            course.Category = dto.Category;
            course.Level = dto.Level;
            course.Duration = dto.Duration;
            course.InstructorId = dto.InstructorId;

            await _context.SaveChangesAsync();
            return Ok(course);
        }

        // DELETE: api/admin/courses/{id}
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Course deleted successfully." });
        }

        // ================== Instructors ==================

        // GET: api/admin/instructors
        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _context.Instructors
                .Select(i => new InstructorReadDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Bio = i.Bio,
                    Skills = i.Skills,
                    PhotoUrl = i.PhotoUrl
                })
                .ToListAsync();
            return Ok(instructors);
        }

        // POST: api/admin/instructors
        [HttpPost("instructors")]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorDto dto)
        {
            var instructor = new Instructor
            {
                Name = dto.Name,
                Bio = dto.Bio,
                Skills = dto.Skills,
                PhotoUrl = dto.PhotoUrl
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return Ok(instructor);
        }

        // PUT: api/admin/instructors/{id}
        [HttpPut("instructors/{id}")]
        public async Task<IActionResult> UpdateInstructor(int id, [FromBody] InstructorDto dto)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            instructor.Name = dto.Name;
            instructor.Bio = dto.Bio;
            instructor.Skills = dto.Skills;
            instructor.PhotoUrl = dto.PhotoUrl;

            await _context.SaveChangesAsync();
            return Ok(instructor);
        }

        // DELETE: api/admin/instructors/{id}
        [HttpDelete("instructors/{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Instructor deleted successfully." });
        }
    }
}
