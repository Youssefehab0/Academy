using System;
using Academy.Domain.Enums;

namespace Academy.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public PaymentMethod Method { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? ScreenshotUrl { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
