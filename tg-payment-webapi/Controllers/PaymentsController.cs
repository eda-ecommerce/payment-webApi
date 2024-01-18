using Core.Models.DTOs.Payment;
using DataAccess.Entities;
//using Newtonsoft.Json.Linq;

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

    // [HttpPut("Update/{id}")]
    // public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] PaymentUpdateDto paymentUpdateDto)
    // {
    //     paymentUpdateDto.PaymentId = id;
    //
    //     // Find payment
    //     var payment = await _paymentService.GetPayment(id);
    //
    //     if (payment == null)
    //     {
    //         return NotFound($"Payment not found: {id}");
    //     }
    //
    //
    //     try
    //     {
    //         await _paymentService.UpdatePayment(paymentUpdateDto);
    //     }
    //     catch (Exception e)
    //     {
    //         return NotFound(e.Message);
    //     }
    //
    //     return Ok();
    // }
    
    [HttpPost("paid/{id}")]
    public async Task<IActionResult> PayingAPayment( Guid id, [FromBody] PaymentWebhookDto paymentWebhookDto)
    {
        // Process the incoming event payload
        // Save the event to the database
        var payment = await _paymentService.GetPayment(id);
        
        if (payment == null)
        {
            return NotFound($"Payment not found: {id}");
        }

        var paymentUpdateDto = new PaymentUpdateDto()
        {
            PaymentId = id,
            PaymentDate = paymentWebhookDto.PaymentDate,
            CreatedDate = payment.CreatedDate,
            OrderId = payment.OrderId,
            Status = Status.Payed
            
        };
        
        try
        {
            await _paymentService.UpdatePayment(paymentUpdateDto);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
        
        
        _logger.LogInformation($"Paying a payment");
        _logger.LogInformation($"Paying a payment  ${id}"  );
        _logger.LogInformation($"paymentWebhookDto ${paymentWebhookDto.PaymentDate}"  );

        return Ok();
    }
}

