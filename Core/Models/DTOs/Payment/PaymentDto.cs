using System.ComponentModel.DataAnnotations;
using DataAccess.Entities;


public class PaymentDto
{
    public Guid? PaymentId { get; set; }

    public Guid OrderId { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public DateOnly CreatedDate { get; set; }
    public Status Status { get; set; } 
    public string Type { get; set; }
}

