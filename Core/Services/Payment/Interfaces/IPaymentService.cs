using Core.Models.DTOs.Payment;

public interface IPaymentService
{
    Task<List<PaymentDto>?> GetPayments();
    Task<PaymentDto?> GetPayment(Guid paymentID);
    Task<PaymentUpdateDto> UpdatePayment(Guid payimentId, PaymentWebhookDto paymentWebhook);
}