public  interface IPaymentService
{
    Task<List<PaymentDto>?> GetPayments();
    Task<PaymentDto?> GetPayment(Guid paymentID);
    Task UpdatePayment(PaymentUpdateDto paymentUpdateDto);
}

