using RapidPay.Application.Services;
using Shouldly;

namespace RapidPay.Tests.Services;

public class FeeServiceTests
{
    [Fact]
    public void GetCurrentFee_ShouldReturnInitialFee()
    {
        // Arrange
        FeeService feeService = FeeService.Instance;

        // Act
        decimal initialFee = feeService.GetCurrentFee();

        // Assert
        initialFee.ShouldBeGreaterThanOrEqualTo(0);
        initialFee.ShouldBeLessThanOrEqualTo(2);
    }

    [Fact]
    public void UpdateFee_ShouldChangeFee()
    {
        // Arrange
        FeeService feeService = FeeService.Instance;
        decimal initialFee = feeService.GetCurrentFee();

        // Act
        SimulateFeeUpdate(feeService);

        // Assert
        decimal updatedFee = feeService.GetCurrentFee();
        updatedFee.ShouldNotBe(initialFee);
        updatedFee.ShouldBeGreaterThanOrEqualTo(0);
    }

    private void SimulateFeeUpdate(FeeService feeService)
    {
        // Use reflection to invoke the private UpdateFee method
        System.Reflection.MethodInfo? updateFeeMethod = typeof(FeeService).GetMethod("UpdateFee", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        updateFeeMethod!.Invoke(feeService, new object?[] { null });
    }
}