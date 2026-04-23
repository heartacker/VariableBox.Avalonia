using System;
using Avalonia.Interactivity;

namespace VariableBox.Common;

public class ValueChangedEventArgs<T> : RoutedEventArgs where T : struct
{
    public T? OldValue { get; }
    public T? NewValue { get; }

    public ValueChangedEventArgs(RoutedEvent routedEvent, T? oldValue, T? newValue) : base(routedEvent)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
