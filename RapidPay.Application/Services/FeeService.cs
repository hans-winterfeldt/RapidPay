using RapidPay.Application.Interfaces;

namespace RapidPay.Application.Services;

public sealed class FeeService : IFeeService
{
    private static readonly Lazy<FeeService> _instance = new(() => new FeeService());
    private decimal _currentFee = (decimal)new Random().NextDouble() * 2; // Initial fee
    private readonly Timer _timer;

    private FeeService()
    {
        // Update the fee every hour
        _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    public static FeeService Instance => _instance.Value;

    public decimal GetCurrentFee()
    {
        return _currentFee;
    }

    private void UpdateFee(object? state)
    {
        decimal randomDecimal = (decimal)new Random().NextDouble() * 2;
        _currentFee *= randomDecimal;
    }
}
