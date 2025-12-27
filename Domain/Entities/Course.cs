namespace Academy.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Duration { get; set; } = null!;

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;

        public ICollection<Booking>? Bookings { get; set; }
    }
}
