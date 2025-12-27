namespace Academy.Domain.Entities
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string? PhotoUrl { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}
