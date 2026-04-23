using System;
using VariableBox.Common;

namespace VariableBox;

public class VariableBoxByte : NumericUpDownBase<byte>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxByte() : base(NumericOperations.Byte) { }
}

public class VariableBoxSByte : NumericUpDownBase<sbyte>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxSByte() : base(NumericOperations.SByte) { }
}

public class VariableBoxShort : NumericUpDownBase<short>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxShort() : base(NumericOperations.Short) { }
}

public class VariableBoxUShort : NumericUpDownBase<ushort>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxUShort() : base(NumericOperations.UShort) { }
}

public class VariableBoxInt : NumericUpDownBase<int>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxInt() : base(NumericOperations.Int) { }
}

public class VariableBoxUInt : NumericUpDownBase<uint>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxUInt() : base(NumericOperations.UInt) { }
}

public class VariableBoxLong : NumericUpDownBase<long>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxLong() : base(NumericOperations.Long) { }
}

public class VariableBoxULong : NumericUpDownBase<ulong>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxULong() : base(NumericOperations.ULong) { }
}

public class VariableBoxFloat : NumericUpDownBase<float>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxFloat() : base(NumericOperations.Float) { }
}

public class VariableBoxDouble : NumericUpDownBase<double>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxDouble() : base(NumericOperations.Double) { }
}

public class VariableBoxDecimal : NumericUpDownBase<decimal>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public VariableBoxDecimal() : base(NumericOperations.Decimal) { }
}
