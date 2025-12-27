using Academy.Domain.Enums;

namespace Academy.Infrastructure.Dto
{
    public class PaymentDto
    {
        public PaymentMethod Method { get; set; }
        public string? ReferenceNumber { get; set; }
        public IFormFile? Screenshot { get; set; }
    }
}
