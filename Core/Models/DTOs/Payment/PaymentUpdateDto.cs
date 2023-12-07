

using DataAccess.Entities;

public class PaymentUpdateDto
{
    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public DateOnly CreatedDate { get; set; }
    public Status Status { get; set; } 
    
}

