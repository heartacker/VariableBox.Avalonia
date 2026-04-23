using System.Globalization;
using VariableBox.Common;

namespace VariableBox.Tests;

public class NumericOperationsTests
{
    [Theory]
    [InlineData(2147483647, 1, 2147483647)]
    [InlineData(-2147483648, -1, -2147483648)]
    [InlineData(10, 5, 15)]
    public void Int_Add_Subtract_Should_Handle_Overflow(int start, int delta, int expected)
    {
        var ops = NumericOperations.Int;
        var result = delta >= 0 ? ops.Add(start, delta) : ops.Subtract(start, -delta);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("100", 100, true)]
    [InlineData("0xFF", 255, true, NumberStyles.HexNumber)]
    [InlineData("h'FF", 255, true, NumberStyles.HexNumber)]
    [InlineData("0b1010", 10, true)]
    [InlineData("b'1111", 15, true)]
    [InlineData("1_000", 1000, true)]
    public void TryParse_Should_Handle_Special_Formats(string input, long expected, bool success, NumberStyles style = NumberStyles.Any)
    {
        var ops = NumericOperations.Long;
        var actualSuccess = ops.TryParse(input, style, CultureInfo.InvariantCulture, out var result);
        Assert.Equal(success, actualSuccess);
        if (success) Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(255, "X2", "FF")]
    [InlineData(10, "b8", "00001010")]
    public void Int_ToString_Should_Handle_Hex_And_Binary(int val, string format, string expected)
    {
        var ops = NumericOperations.Int;
        var result = ops.ToString(val, format, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.45, "F1", "123.5")]
    [InlineData(123.45, "{0:F2}", "123.45")]
    public void Double_ToString_Should_Handle_Standard_Formats(double val, string format, string expected)
    {
        var ops = NumericOperations.Double;
        var result = ops.ToString(val, format, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Double_Should_Parse_Scientific_Notation()
    {
        var ops = NumericOperations.Double;
        bool success = ops.TryParse("1e-9", NumberStyles.Any, CultureInfo.InvariantCulture, out double result);
        Assert.True(success);
        Assert.Equal(1e-9, result);
    }

    [Theory]
    [InlineData(50, 0, 100, 50)]
    [InlineData(-10, 0, 100, 0)]
    public void Internal_Clamp_Should_Correctly_Limit_Values(int val, int min, int max, int expected)
    {
        var ops = NumericOperations.Int;
        Assert.Equal(expected, ops.Clamp(val, min, max));
    }
}
