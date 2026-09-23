using VitalityPortal.Services;

namespace VitalityPortal.Tests;

public sealed class CommissionMathTests
{
    [Fact]
    public void Vat_UsesFifteenPercentAndRoundsAwayFromZero()
    {
        var vat = CommissionMath.Vat(10.01m);

        Assert.Equal(1.50m, vat);
    }

    [Fact]
    public void Vat_ReturnsZeroForZeroAmount()
    {
        var vat = CommissionMath.Vat(0m);

        Assert.Equal(0m, vat);
    }
}
