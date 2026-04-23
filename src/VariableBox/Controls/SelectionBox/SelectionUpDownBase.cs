using Avalonia;
using Avalonia.Data;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VariableBox;

public abstract class SelectionUpDownBase<T> : NumericUpDown
{
    private readonly List<T> _items = new();
    
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(SelectedIndex), 0, defaultBindingMode: BindingMode.TwoWay);

    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly StyledProperty<T?> SelectedItemProperty =
        AvaloniaProperty.Register<NumericUpDown, T?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public T? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly StyledProperty<bool> WrapProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(Wrap), false);

    public bool Wrap
    {
        get => GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    protected IReadOnlyList<T> Items => _items;

    protected void SetItems(IEnumerable<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
        SyncIndexAndItem();
    }

    static SelectionUpDownBase()
    {
        SelectedIndexProperty.Changed.AddClassHandler<SelectionUpDownBase<T>>((o, e) => o.OnSelectedIndexChanged(e));
        SelectedItemProperty.Changed.AddClassHandler<SelectionUpDownBase<T>>((o, e) => o.OnSelectedItemChanged(e));
    }

    private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isSyncing) return;
        SyncIndexAndItem(fromIndex: true);
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isSyncing) return;
        SyncIndexAndItem(fromIndex: false);
    }

    private void SyncIndexAndItem(bool fromIndex = true)
    {
        if (_items.Count == 0) return;
        _isSyncing = true;
        try
        {
            if (fromIndex)
            {
                int index = Math.Clamp(SelectedIndex, 0, _items.Count - 1);
                if (index != SelectedIndex) SelectedIndex = index;
                SelectedItem = _items[index];
            }
            else
            {
                int index = _items.IndexOf(SelectedItem!);
                if (index != -1) SelectedIndex = index;
            }
            SyncTextAndValue(false, null, true);
            SetValidSpinDirection();
        }
        finally { _isSyncing = false; }
    }

    public override void Increase()
    {
        if (_items.Count == 0) return;
        int next = SelectedIndex + 1;
        if (next >= _items.Count)
        {
            if (Wrap) next = 0;
            else next = _items.Count - 1;
        }
        SelectedIndex = next;
    }

    public override void Decrease()
    {
        if (_items.Count == 0) return;
        int prev = SelectedIndex - 1;
        if (prev < 0)
        {
            if (Wrap) prev = _items.Count - 1;
            else prev = 0;
        }
        SelectedIndex = prev;
    }

    protected override void SetValidSpinDirection()
    {
        var d = ValidSpinDirections.None;
        _canIncrease = _canDecrease = false;

        if (!IsReadOnly && _items.Count > 0)
        {
            if (Wrap || SelectedIndex < _items.Count - 1)
            {
                d |= ValidSpinDirections.Increase;
                _canIncrease = true;
            }
            if (Wrap || SelectedIndex > 0)
            {
                d |= ValidSpinDirections.Decrease;
                _canDecrease = true;
            }
        }

        if (_spinner != null) _spinner.ValidSpinDirection = d;
    }

    protected override bool SyncTextAndValue(bool fromText, string? text, bool forceUpdate)
    {
        if (forceUpdate && _textBox != null)
        {
            _textBox.Text = SelectedItem?.ToString();
        }
        return true;
    }

    protected override void CheckContextIsChangedAndValid(string? text, out bool isEditing, out bool isEditingValid)
    {
        isEditing = false;
        isEditingValid = true;
    }

    protected override void OnRead() => RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(ReadRequestedEvent, this));
    protected override void OnWrite() { }
    public override void Clear() { SelectedIndex = 0; }
}
