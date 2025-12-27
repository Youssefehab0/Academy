namespace Academy.Infrastructure.Dto
{
    public class InstructorReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string? PhotoUrl { get; set; }
    }
}
