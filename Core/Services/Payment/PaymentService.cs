using Confluent.Kafka;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConfiguration _configuration;

    public PaymentService(ILogger<PaymentService> logger, IPaymentRepository paymentRepository, IConfiguration configuration)
    {
        _logger = logger;
        _paymentRepository = paymentRepository;
        _configuration = configuration;
    }



    public async Task<List<PaymentDto>?> GetPayments()
    {
        var payments = await _paymentRepository.GetAllPayments();

        if (payments == null)
        {
            return null;
        }

        var paymentDto = payments.Adapt<List<PaymentDto>>();

        return paymentDto;
    }

    public async Task<PaymentDto?> GetPayment(Guid paymentId)
    {
        var payment = await _paymentRepository.GetPayment(paymentId);

        if (payment == null)
        {
            return null;
        }

        // Map to Dto
        var paymentDto = payment.Adapt<PaymentDto>();

        return paymentDto;
    }

    public async Task UpdatePayment(PaymentUpdateDto paymentUpdateDto)
    {
        // get topic from appsettings.json
        var kafka_topic = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKATOPIC")) ? Environment.GetEnvironmentVariable("KAFKATOPIC") : _configuration.GetSection("Kafka").GetSection("Topic1").Value; 
        // TODO: Add Environment variable Broker
        var kafka_broker = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKABROKER")) ? Environment.GetEnvironmentVariable("KAFKABROKER") : _configuration.GetSection("Kafka").GetSection("Broker").Value;
        _logger.LogInformation($" Topic: {kafka_topic}");

        var payment = await _paymentRepository.GetPayment((Guid)paymentUpdateDto.PaymentId);
        

        if (payment == null)
        {
            throw new Exception($"Payment not found: {paymentUpdateDto.PaymentId}");
        }

        payment.PaymentId = paymentUpdateDto.PaymentId;
        payment.OrderId = paymentUpdateDto.OrderId;
        payment.PaymentDate = paymentUpdateDto.PaymentDate;
        payment.CreatedDate = paymentUpdateDto.CreatedDate;
        payment.Status = paymentUpdateDto.Status;
        
        

        await _paymentRepository.UpdatPayment(payment);

        // Produce messages
        ProducerConfig configProducer = new ProducerConfig
        {
            BootstrapServers = kafka_broker,
            ClientId = Dns.GetHostName()
        };

        var kafkaPayment = new PaymentKafkaSchemaDto()
        {
            Source = "Payment-Service",
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
            Type = "updated",
            Payment = new PaymentDto()
            {
                PaymentId = paymentUpdateDto.PaymentId,
                OrderId = paymentUpdateDto.OrderId,
                PaymentDate = paymentUpdateDto.PaymentDate,
                CreatedDate = paymentUpdateDto.CreatedDate,
                Status = paymentUpdateDto.Status,
            }
        };

        using var producer = new ProducerBuilder<Null, string>(configProducer).Build();
        
        var result = await producer.ProduceAsync(kafka_topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize<PaymentKafkaSchemaDto>(kafkaPayment)
        });
        Console.WriteLine(JsonSerializer.Serialize<PaymentKafkaSchemaDto>(kafkaPayment));

        
    }
}

