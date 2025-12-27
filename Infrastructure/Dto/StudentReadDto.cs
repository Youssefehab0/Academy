namespace Academy.Infrastructure.Dto
{
    public class StudentReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
    }
}
