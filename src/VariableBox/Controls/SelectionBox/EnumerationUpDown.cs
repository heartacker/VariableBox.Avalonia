using System;
using System.Linq;

namespace VariableBox;

public class EnumerationUpDown<TEnum> : SelectionUpDownBase<TEnum> where TEnum : struct, Enum
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    public EnumerationUpDown()
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        SetItems(values);
    }
}
