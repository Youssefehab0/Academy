using System;
using Academy.Domain.Enums;

namespace Academy.Infrastructure.Dto
{
    public class BookingReadDto
    {
        public int Id { get; set; }
        public CourseReadDto Course { get; set; } = null!;
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CancellationAllowedUntil { get; set; }
        public bool HasManualCancelRequest { get; set; }
        public PaymentReadDto? Payment { get; set; }
    }
}
