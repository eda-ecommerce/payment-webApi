using Core.Models.DTOs.Payment;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

public class PaymentServiceTest
{
    private readonly PaymentService _sut; //system unit test
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new Mock<IPaymentRepository>();
    private readonly Mock<ILogger<PaymentService>> _logger = new Mock<ILogger<PaymentService>>();
    private readonly Mock<ILogger<PaymentsController>> _loggerController = new Mock<ILogger<PaymentsController>>();
    private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();


    public PaymentServiceTest()
    {
        _sut = new PaymentService(_logger.Object, _paymentRepoMock.Object, _configuration.Object);
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
    
    [Fact]
    public async Task UpdatePayment_ShouldUpdatePayment_WhenPaymentExists()
    {
        // Arrage
        // payment 1
        var newGuid = Guid.NewGuid();
        var newGuid1 = Guid.NewGuid();
        
        var payment1 = new Payment()
        {
            PaymentId = newGuid,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate =DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };
        var payment2 = new Payment()
        {
            PaymentId = newGuid1,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate =DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };
        
       var paymentWebhook = new PaymentWebhookDto()
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Now)
            
        };
       
       

        _paymentRepoMock.Setup(x => x.GetPayment(payment1.PaymentId)).ReturnsAsync(payment1);
        _paymentRepoMock.Setup(x => x.UpdatPayment(payment1));
            
        PaymentsController controller = new PaymentsController(_loggerController.Object, _sut);
        // Act
        var result = await controller.PayingAPayment(newGuid, paymentWebhook);

        // Assert
        var reservation = Assert.IsType<NoContentResult>(result);

    }
    
    [Fact]
    public async Task UpdatePayment_ShouldNotUpdatePayment_WhenThePaymentNotExists()
    {
        
    }
}

