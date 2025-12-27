namespace Academy.Infrastructure.Dto
{
    public class StudentRegisterDto
    {
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
    }

    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
