namespace Academy.Infrastructure.Dto
{
    public class CourseDto
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public int InstructorId { get; set; }
    }
}
