﻿using System.Net;
using System.Text;
using Confluent.Kafka;
using Core.Models.DTOs.Payment;

namespace Presentation.Controllers;

[Route("api/Payments")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    private readonly string? kafka_topic;
    private readonly string? kafka_broker;

    public PaymentsController(ILogger<PaymentsController> logger, IPaymentService paymentService,
        IConfiguration configuration)
    {
        _logger = logger;
        _paymentService = paymentService;
        _configuration = configuration;

        // get topic from appsettings.json or env variable
        kafka_topic = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKATOPIC"))
            ? Environment.GetEnvironmentVariable("KAFKATOPIC")
            : _configuration.GetSection("Kafka").GetSection("Topic1").Value;
        kafka_broker = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKABROKER"))
            ? Environment.GetEnvironmentVariable("KAFKABROKER")
            : _configuration.GetSection("Kafka").GetSection("Broker").Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        _logger.LogInformation($"Get payments request");

        var payments = await _paymentService.GetPayments();

        return Ok(payments);
    }

    [HttpPost("paid/{id}")]
    public async Task<IActionResult> PayingAPayment(Guid id, [FromBody] PaymentWebhookDto paymentWebhookDto)
    {
        try
        {
            var paymentid = await _paymentService.GetPayment(id);
            if (paymentid == null) return NotFound();
            var paymentUpdateDto = await _paymentService.UpdatePayment(id, paymentWebhookDto);
            SendKafkaMessageForUpdatePayment(paymentUpdateDto);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }

    private async void SendKafkaMessageForUpdatePayment(PaymentUpdateDto paymentUpdateDto)
    {
        _logger.LogInformation($"Create Kafka message for payment: ${paymentUpdateDto.PaymentId}");
        // Produce messages
        var configProducer = new ProducerConfig
        {
            BootstrapServers = kafka_broker,
            ClientId = Dns.GetHostName()
        };

        // Create Kafka Header
        var header = new Headers();
        header.Add("source", Encoding.UTF8.GetBytes("payment"));
        header.Add("timestamp",
            Encoding.UTF8.GetBytes(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));
        header.Add("operation", Encoding.UTF8.GetBytes("updated"));

        var paymentDto = new PaymentDto()
        {
            PaymentId = paymentUpdateDto.PaymentId,
            OrderId = paymentUpdateDto.OrderId,
            PaymentDate = paymentUpdateDto.PaymentDate,
            CreatedDate = paymentUpdateDto.CreatedDate,
            Status = paymentUpdateDto.Status
        };
        using var producer = new ProducerBuilder<Null, string>(configProducer).Build();

        await producer.ProduceAsync(kafka_topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize<PaymentDto>(paymentDto),
            Headers = header
        });
        Console.WriteLine(JsonSerializer.Serialize<PaymentDto>(paymentDto));
        _logger.LogInformation($"Kafka message has been sent for payment: ${paymentUpdateDto.PaymentId}");
    }
}