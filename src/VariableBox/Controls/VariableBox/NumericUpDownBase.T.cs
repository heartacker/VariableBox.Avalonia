using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using VariableBox.Common;

namespace VariableBox;

/// <summary>
/// Generic base class for NumericUpDown controls, handling numeric logic.
/// </summary>
public abstract partial class NumericUpDownBase<T> : NumericUpDown where T : struct, IComparable<T>
{
    protected readonly INumericOperations<T> Operations;
    private readonly System.Collections.Generic.Stack<T?> _undoStack = new();
    private readonly System.Collections.Generic.Stack<T?> _redoStack = new();
    private bool _isUndoingRedoing;

    protected NumericUpDownBase(INumericOperations<T> operations)
    {
        Operations = operations;
        SetCurrentValue(MaximumProperty, Operations.MaxValue);
        SetCurrentValue(MinimumProperty, Operations.MinValue);
        SetCurrentValue(StepProperty, Operations.DefaultStep);
    }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;
        _isUndoingRedoing = true;
        try
        {
            _redoStack.Push(Value);
            Value = _undoStack.Pop();
        }
        finally { _isUndoingRedoing = false; }
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        _isUndoingRedoing = true;
        try
        {
            _undoStack.Push(Value);
            Value = _redoStack.Pop();
        }
        finally { _isUndoingRedoing = false; }
    }

    public static readonly StyledProperty<T?> ValueProperty =
        AvaloniaProperty.Register<NumericUpDown, T?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    public T? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<T> StepProperty =
        AvaloniaProperty.Register<NumericUpDown, T>(nameof(Step));

    public T Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly StyledProperty<T> MaximumProperty =
        AvaloniaProperty.Register<NumericUpDown, T>(nameof(Maximum));

    public T Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly StyledProperty<T> MinimumProperty =
        AvaloniaProperty.Register<NumericUpDown, T>(nameof(Minimum));

    public T Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<System.Collections.Generic.IEnumerable<IValidationRule<T>>?> ValidationRulesProperty =
        AvaloniaProperty.Register<NumericUpDown, System.Collections.Generic.IEnumerable<IValidationRule<T>>?>(nameof(ValidationRules));

    public System.Collections.Generic.IEnumerable<IValidationRule<T>>? ValidationRules
    {
        get => GetValue(ValidationRulesProperty);
        set => SetValue(ValidationRulesProperty, value);
    }

    public static readonly RoutedEvent<ValueChangedEventArgs<T>> ValueChangedEvent =
        RoutedEvent.Register<NumericUpDown, ValueChangedEventArgs<T>>(nameof(ValueChanged), RoutingStrategies.Bubble);

    public event EventHandler<ValueChangedEventArgs<T>>? ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    static NumericUpDownBase()
    {
        ValueProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.OnValueChanged(e));
        MaximumProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.SetValidSpinDirection());
        MinimumProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.SetValidSpinDirection());
        StepProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.SetValidSpinDirection());
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isSyncing) return;

        if (!_isUndoingRedoing)
        {
            _undoStack.Push((T?)e.OldValue);
            _redoStack.Clear();
        }

        SyncTextAndValue(false, null, true);
        SetValidSpinDirection();

        T? oldValue = e.OldValue is T ov ? (T?)ov : null;
        T? newValue = e.NewValue is T nv ? (T?)nv : null;

        RaiseEvent(new ValueChangedEventArgs<T>(ValueChangedEvent, oldValue, newValue));
        
        var parameter = CommandParameter ?? Value;
        if (Command != null && Command.CanExecute(parameter))
        {
            Command.Execute(parameter);
        }
    }

    protected override void SetValidSpinDirection()
    {
        var d = ValidSpinDirections.None;
        _canIncrease = _canDecrease = false;

        if (!IsReadOnly)
        {
            var v = Value ?? Operations.Zero;
            if (v.CompareTo(Maximum) < 0)
            {
                d |= ValidSpinDirections.Increase;
                _canIncrease = true;
            }
            if (v.CompareTo(Minimum) > 0)
            {
                d |= ValidSpinDirections.Decrease;
                _canDecrease = true;
            }
        }

        if (_spinner != null) _spinner.ValidSpinDirection = d;
    }

    public override void Increase()
    {
        var v = Value ?? Operations.Zero;
        Value = Operations.Clamp(Operations.Add(v, Step), Minimum, Maximum);
    }

    public override void Decrease()
    {
        var v = Value ?? Operations.Zero;
        Value = Operations.Clamp(Operations.Subtract(v, Step), Minimum, Maximum);
    }

    protected override bool SyncTextAndValue(bool fromText, string? text, bool forceUpdate)
    {
        if (_isSyncing) return true;
        _isSyncing = true;
        bool result = true;
        try
        {
            if (fromText)
            {
                if (Operations.TryParse(text, ParsingNumberStyle, NumberFormat, out T val))
                {
                    bool customValidationPassed = true;
                    if (ValidationRules != null)
                    {
                        foreach (var rule in ValidationRules)
                        {
                            if (!rule.Validate(val).IsValid)
                            {
                                customValidationPassed = false;
                                break;
                            }
                        }
                    }

                    if (customValidationPassed)
                    {
                        Value = Operations.Clamp(val, Minimum, Maximum);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            if (forceUpdate && _textBox != null)
            {
                _textBox.Text = Operations.ToString(Value, FormatString, NumberFormat);
            }
            SetValidSpinDirection();
        }
        finally
        {
            _isSyncing = false;
        }
        return result;
    }

    protected override void CheckContextIsChangedAndValid(string? text, out bool isEditing, out bool isEditingValid)
    {
        isEditingValid = Operations.TryParse(text, ParsingNumberStyle, NumberFormat, out T val);
        isEditing = !Equals(val, Value);

        if (isEditingValid && ValidationRules != null)
        {
            foreach (var rule in ValidationRules)
            {
                var result = rule.Validate(val);
                if (!result.IsValid)
                {
                    isEditingValid = false;
                    DataValidationErrors.SetErrors(this, new[] { result.Message ?? "Invalid value" });
                    break;
                }
            }
        }

        if (isEditingValid)
        {
            DataValidationErrors.ClearErrors(this);
        }
    }

    protected override void OnRead() => RaiseEvent(new RoutedEventArgs(ReadRequestedEvent, this));
    protected override void OnWrite() => RaiseEvent(new ValueChangedEventArgs<T>(ValueChangedEvent, Value, Value));

    protected override void UndoInternal() => Undo();
    protected override void RedoInternal() => Redo();

    public override void Clear() => Value = null;
}
