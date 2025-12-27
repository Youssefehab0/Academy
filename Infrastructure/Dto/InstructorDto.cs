namespace Academy.Infrastructure.Dto
{
    public class InstructorDto
    {
        public string Name { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string? PhotoUrl { get; set; }
    }
}
