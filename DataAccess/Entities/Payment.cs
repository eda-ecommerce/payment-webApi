
using System.ComponentModel.DataAnnotations;
using DataAccess.Entities;

public class Payment
{
    [Key]
    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public DateOnly CreatedDate { get; set; }
    public Status Status { get; set; } 
    
}
