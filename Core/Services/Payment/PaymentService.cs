public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConfiguration _configuration;

    public PaymentService(ILogger<PaymentService> logger, IPaymentRepository paymentRepository,
        IConfiguration configuration)
    {
        _logger = logger;
        _paymentRepository = paymentRepository;
        _configuration = configuration;
    }

    public async Task<List<PaymentDto>?> GetPayments()
    {
        var payments = await _paymentRepository.GetAllPayments();

        if (payments == null) return null;

        var paymentDto = payments.Adapt<List<PaymentDto>>();

        return paymentDto;
    }

    public async Task<PaymentDto?> GetPayment(Guid paymentId)
    {
        var payment = await _paymentRepository.GetPayment(paymentId);

        if (payment == null) return null;

        // Map to Dto
        var paymentDto = payment.Adapt<PaymentDto>();

        return paymentDto;
    }

    public async Task UpdatePayment(PaymentUpdateDto paymentUpdateDto)
    {
        var payment = await _paymentRepository.GetPayment(paymentUpdateDto.PaymentId);

        if (payment == null) throw new Exception($"Payment not found: {paymentUpdateDto.PaymentId}");

        payment.PaymentId = paymentUpdateDto.PaymentId;
        payment.OrderId = paymentUpdateDto.OrderId;
        payment.PaymentDate = paymentUpdateDto.PaymentDate;
        payment.CreatedDate = paymentUpdateDto.CreatedDate;
        payment.Status = paymentUpdateDto.Status;

        await _paymentRepository.UpdatPayment(payment);

        _logger.LogInformation($"Paying was updated ${payment.PaymentId}");
    }
}