using System.Globalization;
using Avalonia.Controls;
using Avalonia.Utilities;

namespace VariableBox.Controls;

public class VariableBoxInt : NumericUpDownBase<int>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxInt()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxInt>(int.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxInt>(int.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxInt>(1);
    }

    protected override bool ParseText(string? text, out int number) =>
        int.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(int? value)
    {
        return value?.ToString(FormatString, NumberFormat);
    }

    protected override int Zero => 0;

    protected override int? Add(int? a, int? b)
    {
        var result = a + b;
        return result < Value ? Maximum : result;
    }

    protected override int? Minus(int? a, int? b)
    {
        var result = a - b;
        return result > Value ? Minimum : result;
    }
}

public class VariableBoxUInt : NumericUpDownBase<uint>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxUInt()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxUInt>(uint.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxUInt>(uint.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxUInt>(1);
    }

    protected override bool ParseText(string? text, out uint number)
    {
        return uint.TryParse(text, ParsingNumberStyle, NumberFormat, out number);
    }

    protected override string? ValueToString(uint? value)
    {
        return value?.ToString(FormatString, NumberFormat);
    }

    protected override uint Zero => 0;

    protected override uint? Add(uint? a, uint? b)
    {
        var result = a + b;
        return result < Value ? Maximum : result;
    }

    protected override uint? Minus(uint? a, uint? b)
    {
        var result = a - b;
        return result > Value ? Minimum : result;
    }
}

public class VariableBoxDouble : NumericUpDownBase<double>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxDouble()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxDouble>(double.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxDouble>(double.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxDouble>(1);
    }

    protected override bool ParseText(string? text, out double number) =>
        double.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(double? value) => value?.ToString(FormatString, NumberFormat);

    protected override double Zero => 0;

    protected override double? Add(double? a, double? b) => a + b;

    protected override double? Minus(double? a, double? b) => a - b;
}

public class VariableBoxByte : NumericUpDownBase<byte>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxByte()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxByte>(byte.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxByte>(byte.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxByte>(1);
    }

    protected override bool ParseText(string? text, out byte number) =>
        byte.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(byte? value) => value?.ToString(FormatString, NumberFormat);

    protected override byte Zero => 0;

    protected override byte? Add(byte? a, byte? b)
    {
        var result = a + b;
        return (byte?)(result < Value ? Maximum : result);
    }

    protected override byte? Minus(byte? a, byte? b)
    {
        var result = a - b;
        return (byte?)(result > Value ? Minimum : result);
    }
}

public class VariableBoxSByte : NumericUpDownBase<sbyte>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxSByte()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxSByte>(sbyte.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxSByte>(sbyte.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxSByte>(1);
    }

    protected override bool ParseText(string? text, out sbyte number) =>
        sbyte.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(sbyte? value) => value?.ToString(FormatString, NumberFormat);

    protected override sbyte Zero => 0;

    protected override sbyte? Add(sbyte? a, sbyte? b)
    {
        var result = a + b;
        return (sbyte?)(result < Value ? Maximum : result);
    }

    protected override sbyte? Minus(sbyte? a, sbyte? b)
    {
        var result = a - b;
        return (sbyte?)(result > Value ? Minimum : result);
    }
}

public class VariableBoxShort : NumericUpDownBase<short>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxShort()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxShort>(short.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxShort>(short.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxShort>(1);
    }

    protected override bool ParseText(string? text, out short number) =>
        short.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(short? value) => value?.ToString(FormatString, NumberFormat);

    protected override short Zero => 0;

    protected override short? Add(short? a, short? b)
    {
        var result = a + b;
        return (short?)(result < Value ? Maximum : result);
    }

    protected override short? Minus(short? a, short? b)
    {
        var result = a - b;
        return (short?)(result > Value ? Minimum : result);
    }
}

public class VariableBoxUShort : NumericUpDownBase<ushort>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxUShort()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxUShort>(ushort.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxUShort>(ushort.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxUShort>(1);
    }

    protected override bool ParseText(string? text, out ushort number) =>
        ushort.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(ushort? value) => value?.ToString(FormatString, NumberFormat);

    protected override ushort Zero => 0;

    protected override ushort? Add(ushort? a, ushort? b)
    {
        var result = a + b;
        return (ushort?)(result < Value ? Maximum : result);
    }

    protected override ushort? Minus(ushort? a, ushort? b)
    {
        var result = a - b;
        return (ushort?)(result > Value ? Minimum : result);
    }
}

public class VariableBoxLong : NumericUpDownBase<long>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxLong()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxLong>(long.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxLong>(long.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxLong>(1);
    }

    protected override bool ParseText(string? text, out long number) =>
        long.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(long? value) => value?.ToString(FormatString, NumberFormat);

    protected override long Zero => 0;

    protected override long? Add(long? a, long? b)
    {
        var result = a + b;
        return result < Value ? Maximum : result;
    }

    protected override long? Minus(long? a, long? b)
    {
        var result = a - b;
        return result > Value ? Minimum : result;
    }
}

public class VariableBoxULong : NumericUpDownBase<ulong>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxULong()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxULong>(ulong.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxULong>(ulong.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxULong>(1);
    }

    protected override bool ParseText(string? text, out ulong number) =>
        ulong.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(ulong? value) => value?.ToString(FormatString, NumberFormat);

    protected override ulong Zero => 0;

    protected override ulong? Add(ulong? a, ulong? b) => a + b;

    protected override ulong? Minus(ulong? a, ulong? b) => a - b;
}

public class VariableBoxFloat : NumericUpDownBase<float>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxFloat()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxFloat>(float.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxFloat>(float.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxFloat>(1);
    }

    protected override bool ParseText(string? text, out float number) =>
        float.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(float? value) => value?.ToString(FormatString, NumberFormat);

    protected override float Zero => 0;

    protected override float? Add(float? a, float? b) => a + b;

    protected override float? Minus(float? a, float? b) => a - b;
}

public class VariableBoxDecimal : NumericUpDownBase<decimal>
{
    protected override Type StyleKeyOverride { get; } = typeof(NumericUpDown);

    static VariableBoxDecimal()
    {
        MaximumProperty.OverrideDefaultValue<VariableBoxDecimal>(decimal.MaxValue);
        MinimumProperty.OverrideDefaultValue<VariableBoxDecimal>(decimal.MinValue);
        StepProperty.OverrideDefaultValue<VariableBoxDecimal>(1);
    }

    protected override bool ParseText(string? text, out decimal number) =>
        decimal.TryParse(text, ParsingNumberStyle, NumberFormat, out number);

    protected override string? ValueToString(decimal? value) => value?.ToString(FormatString, NumberFormat);

    protected override decimal Zero => 0;

    protected override decimal? Add(decimal? a, decimal? b) => a + b;

    protected override decimal? Minus(decimal? a, decimal? b) => a - b;
}
