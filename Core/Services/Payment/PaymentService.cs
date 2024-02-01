using Core.Models.DTOs.Payment;
using DataAccess.Entities;

namespace Core.Services.Payment;

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

    public async Task<PaymentUpdateDto> UpdatePayment(Guid paymentId, PaymentWebhookDto paymentWebhookDto)
    {
        // check if payment exists
        var payment = await _paymentRepository.GetPayment(paymentId);
        if (payment == null)
        {
            _logger.LogInformation($"Payment not found: {paymentId}");
            return new PaymentUpdateDto();
        }

        // update payment
        payment.PaymentDate = paymentWebhookDto.PaymentDate;
        payment.Status = Status.Payed;

        // create DTO to return
        var paymentUpdateDto = new PaymentUpdateDto()
        {
            PaymentId = payment.PaymentId,
            PaymentDate = paymentWebhookDto.PaymentDate,
            CreatedDate = payment.CreatedDate,
            OrderId = payment.OrderId,
            Status = payment.Status
        };

        await _paymentRepository.UpdatPayment(payment);

        _logger.LogInformation($"Paying was updated ${payment.PaymentId}");
        return paymentUpdateDto;
    }
}