using System.Globalization;
using VariableBox.Common;

namespace VariableBox.Tests;

public class NumericOperationsTests
{
    [Theory]
    [InlineData(2147483647, 1, 2147483647)] // Max + 1 -> Max
    [InlineData(-2147483648, -1, -2147483648)] // Min - 1 -> Min
    [InlineData(10, 5, 15)]
    [InlineData(10, -5, 5)]
    public void Int_Add_Subtract_Should_Handle_Overflow(int start, int delta, int expected)
    {
        var ops = NumericOperations.Int;
        var result = delta >= 0 ? ops.Add(start, delta) : ops.Subtract(start, -delta);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("100", 100, true)]
    [InlineData("-50", -50, true)]
    [InlineData("abc", 0, false)]
    [InlineData("FF", 255, true, NumberStyles.HexNumber)]
    public void Int_TryParse_Should_Work(string input, int expected, bool success, NumberStyles style = NumberStyles.Any)
    {
        var ops = NumericOperations.Int;
        var actualSuccess = ops.TryParse(input, style, CultureInfo.InvariantCulture, out var result);
        Assert.Equal(success, actualSuccess);
        if (success) Assert.Equal(expected, result);
    }

    [Fact]
    public void Long_Overflow_Should_Clamp()
    {
        var ops = NumericOperations.Long;
        Assert.Equal(long.MaxValue, ops.Add(long.MaxValue, 100));
        Assert.Equal(long.MinValue, ops.Subtract(long.MinValue, 100));
    }

    [Fact]
    public void UInt_Subtract_Below_Zero_Should_Clamp()
    {
        var ops = NumericOperations.UInt;
        Assert.Equal(0u, ops.Subtract(5u, 10u));
    }

    [Theory]
    [InlineData(10.5, 0.5, 11.0)]
    [InlineData(10.5, -0.5, 10.0)]
    public void Double_Add_Subtract_Should_Work(double start, double delta, double expected)
    {
        var ops = NumericOperations.Double;
        var result = delta >= 0 ? ops.Add(start, delta) : ops.Subtract(start, -delta);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Decimal_Precision_And_Overflow_Should_Work()
    {
        var ops = NumericOperations.Decimal;
        var val = 10.123456789m;
        Assert.Equal(11.123456789m, ops.Add(val, 1.0m));
        
        // Overflow test for decimal
        Assert.Equal(decimal.MaxValue, ops.Add(decimal.MaxValue, 1.0m));
        Assert.Equal(decimal.MinValue, ops.Subtract(decimal.MinValue, 1.0m));
    }

    [Theory]
    [InlineData(50, 0, 100, 50)]
    [InlineData(-10, 0, 100, 0)]
    [InlineData(150, 0, 100, 100)]
    public void Internal_Clamp_Should_Correctly_Limit_Values(int val, int min, int max, int expected)
    {
        var ops = NumericOperations.Int;
        Assert.Equal(expected, ops.Clamp(val, min, max));
    }

    [Fact]
    public void Byte_SByte_Overflow_Should_Clamp()
    {
        Assert.Equal((byte)255, NumericOperations.Byte.Add(250, 10));
        Assert.Equal((sbyte)127, NumericOperations.SByte.Add(120, 10));
        Assert.Equal((sbyte)-128, NumericOperations.SByte.Subtract(-120, 10));
    }
}
