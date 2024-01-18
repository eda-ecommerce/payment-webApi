using DataAccess.Entities;

public class PaymentServiceTest
{
    private readonly PaymentService _sut; //system unit test
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new Mock<IPaymentRepository>();
    private readonly ILogger<PaymentService> _logger;
    private readonly IConfiguration _configuration;


    public PaymentServiceTest()
    {
        _sut = new PaymentService(_logger, _paymentRepoMock.Object, _configuration);
    }

    [Fact]
    public async Task GetPayments_ShouldReturnPayments_WherePaymentsExists()
    {
        // Arrage
        // payment 1
        var payment1Id = Guid.NewGuid();
        var payment1OrderId = Guid.NewGuid();
        var payment1PaymentDate = DateOnly.FromDateTime(DateTime.Now);;
        var payment1CreatedDate = DateOnly.FromDateTime(DateTime.Now);;
        var payment1Status = Status.Unpayed;
        
        var payment1 = new Payment()
        {
            PaymentId = payment1Id,
            OrderId = payment1OrderId,
            PaymentDate = payment1PaymentDate,
            CreatedDate = payment1CreatedDate,
            Status = payment1Status
            
        };

        // payment 2
        var payment2Id = Guid.NewGuid();
        var payment2OrderId = Guid.NewGuid();
        var payment2PaymentDate = DateOnly.FromDateTime(DateTime.Now);;
        var payment2CreatedDate = DateOnly.FromDateTime(DateTime.Now);;
        var payment2Status = Status.Unpayed;
        
        var payment2 = new Payment()
        {
            PaymentId = payment2Id,
            OrderId = payment2OrderId,
            PaymentDate = payment2PaymentDate,
            CreatedDate = payment2CreatedDate,
            Status = payment2Status
        };

        List<Payment> paymentsList = new List<Payment>() {};
        paymentsList.Add(payment1);
        paymentsList.Add(payment2);

        _paymentRepoMock.Setup(x => x.GetAllPayments())
            .ReturnsAsync(paymentsList);

        // Act
        var payments = await _sut.GetPayments();

        // Assert
        // Test payment 1
        payment1Id.Should().Be(payments[0].PaymentId.ToString());
        payment1OrderId.Should().Be(payments[0].OrderId.ToString());
        payment1PaymentDate.Should().Be(payments[0].PaymentDate);
        payment1CreatedDate.Should().Be(payments[0].CreatedDate);
        payment1Status.Should().Be(payments[0].Status);
        // Test payment 2
        payment2Id.Should().Be(payments[1].PaymentId.ToString());
        payment2OrderId.Should().Be(payments[1].OrderId.ToString());
        payment2PaymentDate.Should().Be(payments[1].PaymentDate);
        payment2CreatedDate.Should().Be(payments[1].CreatedDate);
        payment2Status.Should().Be(payments[1].Status);
    }

    [Fact]
    public async Task GetPayments_ShouldReturnNothing_WhenNoPaymentsExists()
    {
        // Arrage
        List<Payment> paymentsList = new List<Payment>() {};

        _paymentRepoMock.Setup(x => x.GetAllPayments())
            .ReturnsAsync(() => null);

        // Act
        var payments = await _sut.GetPayments();

        // Assert
        payments.Should().BeNull();
   
    }

    public async Task UpdatePayment_ShouldUpdatePayment_WhenThePaymentExists()
    {
        
    }
}

