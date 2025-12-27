using System;
using Academy.Domain.Enums;

namespace Academy.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public BookingStatus Status { get; set; } = BookingStatus.PendingApproval;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; set; }
        public DateTime? CancellationAllowedUntil { get; set; }
        public bool HasManualCancelRequest { get; set; }

        public Payment? Payment { get; set; }
    }
}
