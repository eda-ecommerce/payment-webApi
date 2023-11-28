[Route("api/Payments")]
[ApiController]
public class PaymentsController : ControllerBase
{

    private readonly ILogger<PaymentsController> _logger;
    private readonly IPaymentService _paymentService;

    public PaymentsController(ILogger<PaymentsController> logger, IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        _logger.LogInformation($"Get payments request");

        List<PaymentDto> payments = null;
        payments = await _paymentService.GetPayments();

        return Ok(payments);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] PaymentUpdateDto paymentUpdateDto)
    {
        paymentUpdateDto.UserId = id;

        // Find payment
        var payment = await _paymentService.GetPayment(id);

        if (payment == null)
        {
            return NotFound($"Payment not found: {id}");
        }


        try
        {
            await _paymentService.UpdatePayment(paymentUpdateDto);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        return Ok();
    }
}

