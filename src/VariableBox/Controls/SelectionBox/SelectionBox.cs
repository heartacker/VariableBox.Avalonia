using System;
using System.Collections.Generic;
using Avalonia;

namespace VariableBox;

public class ItemsUpDown : SelectionUpDownBase<object>
{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    public static readonly StyledProperty<IEnumerable<object>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ItemsUpDown, IEnumerable<object>?>(nameof(ItemsSource));

    public IEnumerable<object>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    static ItemsUpDown()
    {
        ItemsSourceProperty.Changed.AddClassHandler<ItemsUpDown>((o, e) => o.OnItemsSourceChanged(e));
    }

    private void OnItemsSourceChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is IEnumerable<object> items)
        {
            SetItems(items);
        }
    }
}
