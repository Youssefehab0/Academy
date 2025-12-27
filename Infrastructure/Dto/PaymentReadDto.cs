using System;
using Academy.Domain.Enums;

namespace Academy.Infrastructure.Dto
{
    public class PaymentReadDto
    {
        public int Id { get; set; }
        public PaymentMethod Method { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? ScreenshotUrl { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
