using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Academy.Domain.Entities;
using Academy.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Academy.Infrastructure.Dto;
using System.Security.Cryptography;

namespace Academy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AcademyDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Student> _studentHasher;
        private readonly PasswordHasher<Admin> _adminHasher;

        public AuthController(AcademyDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _studentHasher = new PasswordHasher<Student>();
            _adminHasher = new PasswordHasher<Admin>();
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto dto)
        {
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email))
                return BadRequest("Email already exists.");

            var student = new Student
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Phone = dto.Phone,
                Email = dto.Email,
                AcademicYear = dto.AcademicYear
            };

            student.PasswordHash = _studentHasher.HashPassword(student, dto.Password);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student registered successfully." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Unauthorized("Invalid email or password.");

            // Normalize input
            string email = dto.Email.Trim().ToLower();
            string password = dto.Password.Trim();

            // =======================
            // Try Student
            // =======================
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student != null && !string.IsNullOrEmpty(student.PasswordHash))
            {
                var verifyResult = _studentHasher.VerifyHashedPassword(
                    student,
                    student.PasswordHash,
                    password
                );

                bool passwordValid =
                    verifyResult == PasswordVerificationResult.Success ||
                    (verifyResult == PasswordVerificationResult.Failed &&
                     student.PasswordHash == password); // legacy plain-text support

                if (passwordValid)
                {
                    // Re-hash if legacy plain text
                    if (verifyResult == PasswordVerificationResult.Failed)
                    {
                        student.PasswordHash = _studentHasher.HashPassword(student, password);
                    }

                    var (token, refreshToken, refreshExpiry) =
                        GenerateJwtToken(student.Email, "Student", student.Id);

                    student.RefreshToken = refreshToken;
                    student.RefreshTokenExpiry = refreshExpiry;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        token,
                        refreshToken,
                        role = "Student"
                    });
                }
            }

            // =======================
            // Try Admin
            // =======================
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null && !string.IsNullOrEmpty(admin.PasswordHash))
            {
                var verifyResult = _adminHasher.VerifyHashedPassword(
                    admin,
                    admin.PasswordHash,
                    password
                );

                bool passwordValid =
                    verifyResult == PasswordVerificationResult.Success ||
                    (verifyResult == PasswordVerificationResult.Failed &&
                     admin.PasswordHash == password); // legacy plain-text support

                if (passwordValid)
                {
                    // Re-hash if legacy plain text
                    if (verifyResult == PasswordVerificationResult.Failed)
                    {
                        admin.PasswordHash = _adminHasher.HashPassword(admin, password);
                    }

                    var (token, refreshToken, refreshExpiry) =
                        GenerateJwtToken(admin.Email, "Admin", admin.Id);

                    admin.RefreshToken = refreshToken;
                    admin.RefreshTokenExpiry = refreshExpiry;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        token,
                        refreshToken,
                        role = "Admin"
                    });
                }
            }

            return Unauthorized("Invalid email or password.");
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            // Try Student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.RefreshToken == dto.RefreshToken);
            if (student != null && student.RefreshTokenExpiry.HasValue && student.RefreshTokenExpiry > DateTime.UtcNow)
            {
                var (token, refreshToken, refreshExpiry) = GenerateJwtToken(student.Email, "Student", student.Id);
                student.RefreshToken = refreshToken;
                student.RefreshTokenExpiry = refreshExpiry;
                await _context.SaveChangesAsync();
                return Ok(new { token, refreshToken, role = "Student" });
            }

            // Try Admin
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.RefreshToken == dto.RefreshToken);
            if (admin != null && admin.RefreshTokenExpiry.HasValue && admin.RefreshTokenExpiry > DateTime.UtcNow)
            {
                var (token, refreshToken, refreshExpiry) = GenerateJwtToken(admin.Email, "Admin", admin.Id);
                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiry = refreshExpiry;
                await _context.SaveChangesAsync();
                return Ok(new { token, refreshToken, role = "Admin" });
            }

            return Unauthorized("Invalid or expired refresh token.");
        }

        // JWT Token Generator returns access token and refresh token
        private (string AccessToken, string RefreshToken, DateTime RefreshExpiry) GenerateJwtToken(string email, string role, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("Id", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Create refresh token
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshExpiry = DateTime.UtcNow.AddDays(7);

            return (accessToken, refreshToken, refreshExpiry);
        }
    }

    // DTO for refresh request
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}
