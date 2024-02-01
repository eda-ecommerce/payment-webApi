using Core.Models.DTOs.Payment;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Presentation.Controllers;
using Xunit.Abstractions;

public class PaymentServiceTest
{
    private readonly PaymentService _sut; //system unit test
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new();
    private readonly Mock<ILogger<PaymentService>> _logger = new();
    private readonly Mock<ILogger<PaymentsController>> _loggerController = new();

    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(
            new Dictionary<string, string>
            {
                { "Kafka:Broker", "placeholder" },
                { "Kafka:Topic", "placeholder" },
                { "Kafka:GroupId", "placeholder" }
            }!)
        .Build();

    private readonly ITestOutputHelper output;


    public PaymentServiceTest(ITestOutputHelper output)
    {
        _sut = new PaymentService(_logger.Object, _paymentRepoMock.Object, _configuration);
        this.output = output;
    }

    [Fact]
    public async Task GetPayments_ShouldReturnPayments_WherePaymentsExists()
    {
        // Arrage
        // payment 1
        var payment1Id = Guid.NewGuid();
        var payment1OrderId = Guid.NewGuid();
        var payment1PaymentDate = DateOnly.FromDateTime(DateTime.Now);
        ;
        var payment1CreatedDate = DateOnly.FromDateTime(DateTime.Now);
        ;
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
        var payment2PaymentDate = DateOnly.FromDateTime(DateTime.Now);
        ;
        var payment2CreatedDate = DateOnly.FromDateTime(DateTime.Now);
        ;
        var payment2Status = Status.Unpayed;

        var payment2 = new Payment()
        {
            PaymentId = payment2Id,
            OrderId = payment2OrderId,
            PaymentDate = payment2PaymentDate,
            CreatedDate = payment2CreatedDate,
            Status = payment2Status
        };

        var paymentsList = new List<Payment>() { };
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
        var paymentsList = new List<Payment>() { };

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

        var payment1 = new Payment()
        {
            PaymentId = newGuid,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };
        var paymentUpdateDto = new PaymentUpdateDto()
        {
            PaymentId = newGuid,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };

        var paymentWebhook = new PaymentWebhookDto()
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Now)
        };

        _paymentRepoMock.Setup(x => x.GetPayment(payment1.PaymentId)).ReturnsAsync(payment1);
        _paymentRepoMock.Setup(x => x.UpdatPayment(payment1)).Returns(Task.CompletedTask);

        // Act
        await _sut.UpdatePayment(paymentUpdateDto);

        // Assert
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == $@"Paying was updated ${payment1.PaymentId}"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldNotUpdatePayment_UpdatedPaymentIsNotFound()
    {
        // Arrage
        // payment 1
        var Guid1 = Guid.NewGuid();
        var Guid2 = Guid.NewGuid();

        var payment1 = new Payment()
        {
            PaymentId = Guid1,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };
        var paymentUpdateDto2 = new PaymentUpdateDto()
        {
            PaymentId = Guid2,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };

        var paymentWebhook = new PaymentWebhookDto()
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Now)
        };

        _paymentRepoMock.Setup(x => x.GetPayment(payment1.PaymentId)).ReturnsAsync(payment1);
        _paymentRepoMock.Setup(x => x.UpdatPayment(payment1));


        // Act
        await _sut.UpdatePayment(paymentUpdateDto2);

        // Assert
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == $@"Payment not found: {paymentUpdateDto2.PaymentId}"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePayment_ShouldNotUpdatePayment_PaymantIdIsWrongFromPayment1()
    {
        // Arrage
        // payment 1
        var Guid1 = Guid.NewGuid();
        var Guid2 = Guid.NewGuid();

        var payment1 = new Payment()
        {
            PaymentId = Guid1,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };
        var payment2 = new Payment()
        {
            PaymentId = Guid2,
            OrderId = Guid.NewGuid(),
            PaymentDate = null,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Status = Status.Unpayed
        };

        var paymentWebhook = new PaymentWebhookDto()
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Now)
        };

        _paymentRepoMock.Setup(x => x.GetPayment(payment2.PaymentId)).ReturnsAsync(payment2);
        _paymentRepoMock.Setup(x => x.UpdatPayment(payment1));

        var controller = new PaymentsController(_loggerController.Object, _sut, _configuration);

        // Act
        var result = await controller.PayingAPayment(Guid1, paymentWebhook);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}