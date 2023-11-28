public interface IPaymentRepository
{
    Task<List<Payment>> GetAllPayments();
    Task<Payment> GetPayment(Guid paymentId);
    Task UpdatPayment(Payment payment);
}

