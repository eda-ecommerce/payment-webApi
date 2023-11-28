public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> GetAllPayments()
    {
        var payments = await _context.Payments
            .ToListAsync();

        return payments;
    }
    public async Task<Payment> GetPayment(Guid paymentId)
    {
        var payment = await _context.Payments
            .Where(p => p.UserId == paymentId)
            .FirstOrDefaultAsync();
        return payment;
    }

    public async Task UpdatPayment(Payment payment)
    {
        var paymentToUpdate = await _context.Payments
            .Where(p => p.UserId == payment.UserId)
            .FirstOrDefaultAsync();

        paymentToUpdate = payment;

        _context.Update(paymentToUpdate);
        _context.SaveChanges();
    }

}

