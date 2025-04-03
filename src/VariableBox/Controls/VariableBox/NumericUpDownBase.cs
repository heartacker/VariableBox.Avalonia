using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using VariableBox.Common;

namespace VariableBox.Controls;

[TemplatePart(PART_Spinner, typeof(ButtonSpinner))]
[TemplatePart(PART_TextBox, typeof(TextBox))]
[TemplatePart(PART_DragPanel, typeof(Panel))]
[TemplatePart(PART_RepeatRead, typeof(RepeatButton))]
[TemplatePart(PART_RepeatWrite, typeof(RepeatButton))]
[PseudoClasses(":editing", ":invalid")]
public abstract class NumericUpDown : TemplatedControl /* , Control */ /*, IClearControl*/
{
    public const string PART_Spinner = "PART_Spinner";
    public const string PART_TextBox = "PART_TextBox";
    public const string PART_DragPanel = "PART_DragPanel";

    protected internal ButtonSpinner? _spinner;
    protected internal TextBox? _textBox;
    protected internal Panel? _dragPanel;

    public const string PART_RepeatRead = "PART_RepeatRead";
    public const string PART_RepeatWrite = "PART_RepeatWrite";

    protected RepeatButton? _repeatReadButton;
    protected RepeatButton? _repeatWriteButton;

    private Point? _point;
    protected internal bool _updateFromTextInput;

    protected internal bool _canIncrease = true;

    protected internal bool _canDecrease = true;


    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":editing", IsEditing);
        PseudoClasses.Set(":invalid", !IsEditingValid);
    }

    public static readonly StyledProperty<bool> IsAllowDragProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsAllowDrag), defaultBindingMode: BindingMode.TwoWay);

    public bool IsAllowDrag
    {
        get => GetValue(IsAllowDragProperty);
        set => SetValue(IsAllowDragProperty, value);
    }

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsReadOnly), defaultBindingMode: BindingMode.TwoWay);

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<NumericUpDown>();
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

    public static readonly StyledProperty<object?> HeaderContentProperty =
        AvaloniaProperty.Register<NumericUpDown, object?>(nameof(HeaderContent));

    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<NumericUpDown, string?>(nameof(Watermark));

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public static readonly StyledProperty<NumberFormatInfo?> NumberFormatProperty =
        AvaloniaProperty.Register<NumericUpDown, NumberFormatInfo?>(nameof(NumberFormat), defaultValue: NumberFormatInfo.CurrentInfo);

    public NumberFormatInfo? NumberFormat
    {
        get => GetValue(NumberFormatProperty);
        set => SetValue(NumberFormatProperty, value);
    }

    public static readonly StyledProperty<string> FormatStringProperty =
        AvaloniaProperty.Register<NumericUpDown, string>(nameof(FormatString), string.Empty);

    public string FormatString
    {
        get => GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public static readonly StyledProperty<NumberStyles> ParsingNumberStyleProperty =
        AvaloniaProperty.Register<NumericUpDown, NumberStyles>(nameof(ParsingNumberStyle), defaultValue: NumberStyles.Any);

    public NumberStyles ParsingNumberStyle
    {
        get => GetValue(ParsingNumberStyleProperty);
        set => SetValue(ParsingNumberStyleProperty, value);
    }

    public static readonly StyledProperty<IValueConverter?> TextConverterProperty =
        AvaloniaProperty.Register<NumericUpDown, IValueConverter?>(nameof(TextConverter));

    public IValueConverter? TextConverter
    {
        get => GetValue(TextConverterProperty);
        set => SetValue(TextConverterProperty, value);
    }

    public static readonly StyledProperty<bool> IsAllowSpinProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsAllowSpin), true);

    public bool IsAllowSpin
    {
        get => GetValue(IsAllowSpinProperty);
        set => SetValue(IsAllowSpinProperty, value);
    }
    /// <summary>
    /// ! ButtonSpinner
    /// </summary>
    public static readonly StyledProperty<bool> ShowButtonSpinnerProperty =
        ButtonSpinner.ShowButtonSpinnerProperty.AddOwner<NumericUpDown>();

    public bool ShowButtonSpinner
    {
        get => GetValue(ShowButtonSpinnerProperty);
        set => SetValue(ShowButtonSpinnerProperty, value);
    }

    public static readonly StyledProperty<bool> IsShowReadButtonProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsShowReadButton), true);

    public bool IsShowReadButton
    {
        get => GetValue(IsShowReadButtonProperty);
        set => SetValue(IsShowReadButtonProperty, value);
    }

    public static readonly StyledProperty<bool> IsShowWriteButtonProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsShowWriteButton), true);

    public bool IsShowWriteButton
    {
        get => GetValue(IsShowWriteButtonProperty);
        set => SetValue(IsShowWriteButtonProperty, value);
    }

    public static readonly StyledProperty<bool> IsReadWriteButtonShowProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsReadWriteButtonShow), true);

    public bool IsReadWriteButtonShow
    {
        get => GetValue(IsReadWriteButtonShowProperty);
        protected set => SetValue(IsReadWriteButtonShowProperty, value);
    }

    public static readonly StyledProperty<bool> IsUpdateValueWhenLostFocusProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsUpdateValueWhenLostFocus), false);

    /// <summary>
    /// If true, the value will be updated when the user loses focus.
    /// </summary>
    public bool IsUpdateValueWhenLostFocus
    {
        get => GetValue(IsUpdateValueWhenLostFocusProperty);
        set => SetValue(IsUpdateValueWhenLostFocusProperty, value);
    }

    public bool IsEditingValid { get; protected set; }

#if false
    public static readonly StyledProperty<bool> IsEditingVisiableProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsEditingVisiable), false);

    /// <summary>
    /// 指示当前是否正在编辑，如果为true，则不允许用户点击 <see cref="OnSpin"/>  和 read <see cref="OnReadBefore"/>
    ///  todo we should not allow user to spin or read, disable those control
    /// </summary>
    public bool IsEditingVisiable
    {
        get => GetValue(IsEditingVisiableProperty);
        protected set => SetValue(IsEditingVisiableProperty, value && IsEnableEditingIndicator);
    }
#endif

    public static readonly StyledProperty<bool> IsEditingProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsEditing), false);

    /// <summary>
    /// 指示当前是否正在编辑，如果为true，则不允许用户点击 <see cref="OnSpin"/>  和 read <see cref="OnReadBefore"/>
    /// </summary>
    public bool IsEditing
    {
        get => GetValue(IsEditingProperty);
        protected set => SetValue(IsEditingProperty, value);
    }

    public static readonly StyledProperty<bool> IsEnableEditingIndicatorProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsEnableEditingIndicator), true, false, BindingMode.TwoWay);

    public bool IsEnableEditingIndicator
    {
        get => GetValue(IsEnableEditingIndicatorProperty);
        set => SetValue(IsEnableEditingIndicatorProperty, value);
    }

    public event EventHandler<SpinEventArgs>? Spinned;

    static NumericUpDown()
    {
        NumberFormatProperty.Changed.AddClassHandler<NumericUpDown>((o, e) => o.OnFormatChange(e));
        FormatStringProperty.Changed.AddClassHandler<NumericUpDown>((o, e) => o.OnFormatChange(e));
        IsReadOnlyProperty.Changed.AddClassHandler<NumericUpDown, bool>((o, args) => o.OnIsReadOnlyChanged(args));
        TextConverterProperty.Changed.AddClassHandler<NumericUpDown>((o, e) => o.OnFormatChange(e));
        IsAllowDragProperty.Changed.AddClassHandler<NumericUpDown, bool>((o, e) => o.OnIsAllowDragChange(e));

        IsShowReadButtonProperty.Changed.AddClassHandler<NumericUpDown, bool>((o, e) => o.OnReadWriteShowChange(e));
        IsShowWriteButtonProperty.Changed.AddClassHandler<NumericUpDown, bool>((o, e) => o.OnReadWriteShowChange(e));

        // IsEnableEditingIndicatorProperty.Changed.AddClassHandler<NumericUpDown, bool>((o, e) => o.OnIsEnableEditingIndicatorChanged(e));
    }

    private void OnIsEnableEditingIndicatorChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        // IsEditingVisiable =
        // IsEditing && IsEnableEditingIndicator;
    }

    private void OnReadWriteShowChange(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        IsReadWriteButtonShow = IsShowReadButton || IsShowWriteButton;
    }

    private void OnIsAllowDragChange(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        IsVisibleProperty.SetValue(args.NewValue.Value, _dragPanel);
    }

    private void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        ChangeToSetSpinDirection(args, false);
        TextBox.IsReadOnlyProperty.SetValue(args.NewValue.Value, _textBox);
    }

    protected void ChangeToSetSpinDirection(AvaloniaPropertyChangedEventArgs e, bool afterInitialization = false)
    {
        if (afterInitialization)
        {
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }
        else
        {
            SetValidSpinDirection();
        }
    }

    protected virtual void OnFormatChange(AvaloniaPropertyChangedEventArgs arg)
    {
        if (IsInitialized)
        {
            SyncTextAndValue(false, null, true); // sync text update while OnFormatChange
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        Spinner.SpinEvent.RemoveHandler(OnSpin, _spinner);
        PointerPressedEvent.RemoveHandler(OnDragPanelPointerPressed, _dragPanel);
        PointerMovedEvent.RemoveHandler(OnDragPanelPointerMoved, _dragPanel);
        PointerReleasedEvent.RemoveHandler(OnDragPanelPointerReleased, _dragPanel);

        _spinner = e.NameScope.Find<ButtonSpinner>(PART_Spinner);
        Spinner.SpinEvent.AddHandler(OnSpin, _spinner);
        Spinner.GotFocusEvent.AddHandler(OnSpinWholeGotFocus, _spinner);

        _textBox = e.NameScope.Find<TextBox>(PART_TextBox);
        TextBox.IsReadOnlyProperty.SetValue(IsReadOnly, _textBox);

        // if (_textBox != null)
        // {
        //     _textBox.TextChanged += OnTextBoxTextChanged;
        // }

        TextBox.TextChangedEvent.AddHandler(OnTextBoxTextChanged, _textBox);
        KeyDownEvent.AddHandler(OnTextBoxKeyDown, RoutingStrategies.Direct | RoutingStrategies.Bubble, true, _textBox);
        DoubleTappedEvent.AddHandler(OnTextBoxDoubleTapped, RoutingStrategies.Direct | RoutingStrategies.Bubble, true, _textBox);

        _dragPanel = e.NameScope.Find<Panel>(PART_DragPanel);
        IsVisibleProperty.SetValue(IsAllowDrag, _dragPanel);
        PointerPressedEvent.AddHandler(OnDragPanelPointerPressed, _dragPanel);
        PointerMovedEvent.AddHandler(OnDragPanelPointerMoved, _dragPanel);
        PointerReleasedEvent.AddHandler(OnDragPanelPointerReleased, _dragPanel);

        OnApplyTemplateReadWrite(e);
    }

    private void OnSpinWholeGotFocus(object sender, GotFocusEventArgs e)
    {
        //_textBox?.Focus();
    }

    protected void OnApplyTemplateReadWrite(TemplateAppliedEventArgs e)
    {
        RepeatButton.ClickEvent.RemoveHandler(OnReadBefore, _repeatReadButton);
        RepeatButton.ClickEvent.RemoveHandler(OnWriteBefore, _repeatWriteButton);

        _repeatReadButton = e.NameScope.Find<RepeatButton>(PART_RepeatRead);
        _repeatWriteButton = e.NameScope.Find<RepeatButton>(PART_RepeatWrite);

        RepeatButton.ClickEvent.AddHandler(OnReadBefore, _repeatReadButton);
        RepeatButton.ClickEvent.AddHandler(OnWriteBefore, _repeatWriteButton);
    }

    private void OnTextBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        // is double tapped on HeaderContent
        if (e.Source is TextBlock)
        {
            OnHeaderDoubleTaped(sender, e);
        }
    }
    protected abstract void OnHeaderDoubleTaped(object? sender, TappedEventArgs e);

    private void OnWriteBefore(object sender, RoutedEventArgs e)
    {
        var commitSuccess = CommitInput(true);
        e.Handled = !commitSuccess;
        if (commitSuccess && !IsEditing)
            OnWrite(sender, e);
    }

    protected abstract void OnWrite(object sender, RoutedEventArgs e);

    private void OnReadBefore(object sender, RoutedEventArgs e)
    {
        if (IsEditing)
            return;
        OnRead(sender, e);
    }

    protected abstract void OnRead(object sender, RoutedEventArgs e);

    protected abstract void CheckContextIsChangedAndValid(string? text, ref bool isediting, ref bool valid);

    bool ed = false;
    bool edv = false;

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        CheckContextIsChangedAndValid((sender as TextBox)?.Text, ref ed, ref edv);

        // IsEditingVisiable =
        IsEditing = ed;
        IsEditingValid = edv;
        UpdatePseudoClasses();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        if (IsAllowDrag && _dragPanel is not null)
        {
            _dragPanel.IsVisible = true;
        }
        if (IsUpdateValueWhenLostFocus)
        {
            CommitInput(true);
        }
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine(e.Key);
        if (e.Key == Key.Left || e.Key == Key.Right ||
            e.Key == Key.Up || e.Key == Key.Down ||
            e.Key == Key.Enter
            )
        {
            this.OnKeyDown(e);
            //e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // System.Diagnostics.Trace.WriteLine(e.Key);
        if (e.Key == Key.Escape)
        {
            SyncTextAndValue(fromTextToValue: false, text: null, forceTextUpdate: true); // recover the value
            if (IsAllowDrag && _dragPanel is not null)
            {
                _dragPanel.IsVisible = true;
                // _dragPanel.Focus();
                _textBox?.ClearSelection();
                //_spinner?.Focus();
            }
        }

        //* write
        if (e.Key == Key.Enter || (e.Key == Key.Right && e.KeyModifiers == KeyModifiers.Alt))
        {
            // var _fmtedString = ConvertValueToText(Value);
            // if (IsEditing)
            {
                //! if value changed, fire a changed,
                //! if value not changed but text is changed, SyncTextAndValue without trigger
                var commitSuccess = CommitInput(true);
                OnTextBoxTextChanged(_textBox, null);
                e.Handled = commitSuccess;
                _textBox?.ClearSelection();
                //_spinner?.Focus();
            }
            // if value changed, trigger had been fired above,
            // if value not changed, fire a changed anyway
            if (!IsEditing && e.KeyModifiers == KeyModifiers.Alt)
            {
                OnWrite(this, e);
            }
            e.Handled = true;
        }

        //*read
        else if (e.Key == Key.Left && e.KeyModifiers == KeyModifiers.Alt)
        {
            OnReadBefore(this, e);
            e.Handled = true;
        }
        //* up and down
        else
        {
            // e.Handled = true;
        }
        // ignore key whhile alt is pressed
        if (e.KeyModifiers == KeyModifiers.Alt) e.Handled = true;
    }

    private void OnDragPanelPointerPressed(object sender, PointerPressedEventArgs e)
    {
        _point = e.GetPosition(this);
        if (e.ClickCount == 2 && _dragPanel is not null && IsAllowDrag)
        {
            IsVisibleProperty.SetValue(false, _dragPanel);
            _textBox?.Focus();
            TextBox.IsReadOnlyProperty.SetValue(IsReadOnly, _textBox);
        }
        else
        {
            _textBox?.Focus();
            TextBox.IsReadOnlyProperty.SetValue(true, _textBox);
        }
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (IsReadOnly)
            return;
        _textBox?.RaiseEvent(e);
    }

    private void OnDragPanelPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _point = null;
    }

    private void OnDragPanelPointerMoved(object sender, PointerEventArgs e)
    {
        if (!IsAllowDrag || IsReadOnly)
            return;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        var point = e.GetPosition(this);
        var delta = point - _point;
        if (delta is null)
        {
            return;
        }
        int d = GetDelta(delta.Value);
        if (d > 0)
        {
            if (_canIncrease)
                Increase();
        }
        else if (d < 0)
        {
            if (_canDecrease)
                Decrease();
        }
        _point = point;
    }

    private int GetDelta(Point point)
    {
        bool horizontal = Math.Abs(point.X) > Math.Abs(point.Y);
        var value = horizontal ? point.X : -point.Y;
        return value switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
    }

    private void OnSpin(object sender, SpinEventArgs e)
    {
        if (IsEditing) return;
        if (IsAllowSpin && !IsReadOnly)
        {
            var spin = !e.UsingMouseWheel;
            spin |= _textBox is { IsFocused: true };
            if (spin)
            {
                e.Handled = true;
                var handler = Spinned;
                handler?.Invoke(this, e);
                if (e.Direction == SpinDirection.Increase)
                {
                    Increase();
                }
                else
                {
                    Decrease();
                }
            }
        }
    }

    protected abstract void SetValidSpinDirection();

    protected abstract void Increase();

    protected abstract void Decrease();

    // 在 text 变化后，回车写入
    protected virtual bool CommitInput(bool forceTextUpdate = false)
    {
        return SyncTextAndValue(fromTextToValue: true, _textBox?.Text, forceTextUpdate);
    }

    /// <summary>
    /// 同步更新 value 和显示的 text
    /// </summary>
    /// <param name="fromTextToValue">指示是否从text 的变化更新到值，还是值反过来</param>
    /// <param name="text">text 2 value 的时候，text</param>
    /// <param name="forceTextUpdate"></param>
    /// <returns></returns>
    protected abstract bool SyncTextAndValue(bool fromTextToValue = false, string? text = null,
        bool forceTextUpdate = false);

    public abstract void Clear();
}

public abstract class NumericUpDownBase<T> : NumericUpDown where T : struct, IComparable<T>
{
    protected static string TrimString(string? text, NumberStyles numberStyles)
    {
        if (text is null)
            return string.Empty;
        text = text.Trim();
        if (text.Contains("_")) // support _ like 0x1024_1024(hex), 10_24 (normal)
        {
            text = text.Replace("_", "");
        }

        if ((numberStyles & NumberStyles.AllowHexSpecifier) != 0)
        {
            if (text.StartsWith("0x") || text.StartsWith("0X")) // support 0x hex while user input
            {
                text = text.Substring(2);
            }
            else if (text.StartsWith("h'") || text.StartsWith("H'")) // support verilog hex while user input
            {
                text = text.Substring(2);
            }
            else if (text.StartsWith("h") || text.StartsWith("H")) // support  hex while user input
            {
                text = text.Substring(1);
            }
        }
#if NET8_0_OR_GREATER
        else if ((numberStyles & NumberStyles.AllowBinarySpecifier) != 0)
        {
            if (text.StartsWith("0b") || text.StartsWith("0B")) // support 0b bin while user input
            {
                text = text.Substring(2);
            }
            else if (text.StartsWith("b'") || text.StartsWith("B'")) // support verilog bin while user input
            {
                text = text.Substring(2);
            }
            else if (text.StartsWith("b") || text.StartsWith("B")) // support bin while user input
            {
                text = text.Substring(1);
            }
        }

#endif
        return text;
    }

    #region Values

    public static readonly StyledProperty<T?> LastEditingValidValueProperty = AvaloniaProperty.Register<NumericUpDownBase<T>, T?>(
        nameof(LastEditingValidValue), defaultBindingMode: BindingMode.TwoWay);

    public T? LastEditingValidValue
    {
        get => GetValue(LastEditingValidValueProperty);
        set => SetValue(LastEditingValidValueProperty, value);
    }

    public static readonly StyledProperty<T?> ValueProperty = AvaloniaProperty.Register<NumericUpDownBase<T>, T?>(
        nameof(Value), defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    public T? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        if (property == ValueProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }

    public static readonly StyledProperty<T> StepProperty = AvaloniaProperty.Register<NumericUpDownBase<T>, T>(
        nameof(Step));

    public T Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly StyledProperty<T> MaximumProperty = AvaloniaProperty.Register<NumericUpDownBase<T>, T>(
        nameof(Maximum), defaultBindingMode: BindingMode.TwoWay, coerce: CoerceMaximum);

    public T Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly StyledProperty<T> MinimumProperty = AvaloniaProperty.Register<NumericUpDownBase<T>, T>(
        nameof(Minimum), defaultBindingMode: BindingMode.TwoWay, coerce: CoerceMinimum);

    public T Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    #endregion

    #region Max and Min Coerce
    private static T CoerceMaximum(AvaloniaObject instance, T value)
    {
        if (instance is NumericUpDownBase<T> n)
        {
            return n.CoerceMaximum(value);
        }

        return value;
    }

    private T CoerceMaximum(T value)
    {
        if (value.CompareTo(Minimum) < 0)
        {
            return Minimum;
        }
        return value;
    }

    private static T CoerceMinimum(AvaloniaObject instance, T value)
    {
        if (instance is NumericUpDownBase<T> n)
        {
            return n.CoerceMinimum(value);
        }

        return value;
    }

    private T CoerceMinimum(T value)
    {
        if (value.CompareTo(Maximum) > 0)
        {
            return Maximum;
        }
        return value;
    }

    #endregion


    /// <summary>
    /// Defines the <see cref="ValueChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<ValueChangedEventArgs<T>> ValueChangedEvent =
        RoutedEvent.Register<NumericUpDownBase<T>, ValueChangedEventArgs<T>>(nameof(ValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised when the <see cref="Value"/> changes.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<T>>? ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    #region Command

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<NumericUpDown, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<NumericUpDown, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    #endregion

    #region ClearCommand

    public static readonly StyledProperty<ICommand?> ClearProperty =
        AvaloniaProperty.Register<NumericUpDown, ICommand?>(nameof(Clear));

    #endregion

    #region ReadCommand

    public static readonly StyledProperty<ICommand?> ReadCommandProperty =
        AvaloniaProperty.Register<NumericUpDown, ICommand?>(nameof(ReadCommand));

    public ICommand? ReadCommand
    {
        get => GetValue(ReadCommandProperty);
        set => SetValue(ReadCommandProperty, value);
    }

    public static readonly StyledProperty<object?> ReadCommandParameterProperty =
        AvaloniaProperty.Register<NumericUpDown, object?>(nameof(ReadCommandParameter));

    public object? ReadCommandParameter
    {
        get => this.GetValue(ReadCommandParameterProperty);
        set => this.SetValue(ReadCommandParameterProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ReadRequested"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ReadRequestedEvent =
        RoutedEvent.Register<NumericUpDown, RoutedEventArgs>(nameof(ReadRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised when the Read required, like Read Button Click.
    /// <br/>
    /// [!!!] If you update the <see cref="Value"/> int the <see cref="ReadRequested"/>, <see cref="ValueChanged"/> Will
    /// not Raise
    /// </summary>
    public event EventHandler<RoutedEventArgs>? ReadRequested
    {
        add => AddHandler(ReadRequestedEvent, value);
        remove => RemoveHandler(ReadRequestedEvent, value);
    }

    #endregion

    #region HeaderDoubleTapedCommand

    public static readonly StyledProperty<ICommand?> HeaderDoubleTapedCommandProperty =
        AvaloniaProperty.Register<NumericUpDown, ICommand?>(nameof(HeaderDoubleTapedCommand));

    public ICommand? HeaderDoubleTapedCommand
    {
        get => GetValue(HeaderDoubleTapedCommandProperty);
        set => SetValue(HeaderDoubleTapedCommandProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="HeaderDoubleTaped"/> event.
    /// </summary>
    public static readonly RoutedEvent<TappedEventArgs> HeaderDoubleTapedEvent =
        RoutedEvent.Register<NumericUpDown, TappedEventArgs>(nameof(HeaderDoubleTaped), RoutingStrategies.Bubble);

    public event EventHandler<TappedEventArgs>? HeaderDoubleTaped
    {
        add => AddHandler(HeaderDoubleTapedEvent, value);
        remove => RemoveHandler(HeaderDoubleTapedEvent, value);
    }

    #endregion

    #region InvokeCommand

    private void InvokeCommand(ICommand? command, object? cp)
    {
        if (command != null && command.CanExecute(cp))
        {
            command.Execute(cp);
        }
    }

    private void RaiseEventCommand(ValueChangedEventArgs<T> e)
    {
        InvokeCommand(this.Command, this.CommandParameter ?? e.NewValue);
        RaiseEvent(e);
    }

    private void RaiseReadEventCommand(RoutedEventArgs e)
    {
        InvokeCommand(this.ReadCommand, this.ReadCommandParameter ?? Value);
        RaiseEvent(e);
    }

    private void RaiseHeaderDoubleTapedEventCommand(TappedEventArgs e)
    {
        InvokeCommand(this.HeaderDoubleTapedCommand, Value);
        RaiseEvent(e);
    }
    #endregion

    static NumericUpDownBase()
    {
        StepProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.ChangeToSetSpinDirection(e));
        MaximumProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.OnConstraintChanged(e));
        MinimumProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.OnConstraintChanged(e));
        ValueProperty.Changed.AddClassHandler<NumericUpDownBase<T>>((o, e) => o.OnValueChanged(e));
    }

    private void OnConstraintChanged(
        AvaloniaPropertyChangedEventArgs avaloniaPropertyChangedEventArgs
    )
    {
        if (IsInitialized)
        {
            SetValidSpinDirection();
        }
        if (Value.HasValue)
        {
            SetCurrentValue(ValueProperty, Clamp(Value, Maximum, Minimum));
        }
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (IsInitialized)
        {
            SyncTextAndValue(false, null, true);
            SetValidSpinDirection();

            if (isReading)
                return;

            T? oldValue = args.GetOldValue<T?>();
            T? newValue = args.GetNewValue<T?>();
            var e = new ValueChangedEventArgs<T>(ValueChangedEvent, oldValue, newValue);
            RaiseEventCommand(e);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_textBox != null)
        {
            _textBox.Text = ConvertValueToText(Value);
        }
        SetValidSpinDirection();
    }

    protected virtual T? Clamp(T? value, T max, T min)
    {
        if (value is null)
        {
            return null;
        }
        if (value.Value.CompareTo(max) > 0)
        {
            return max;
        }
        if (value.Value.CompareTo(min) < 0)
        {
            return min;
        }
        return value;
    }

    protected override void SetValidSpinDirection()
    {
        var validDirection = ValidSpinDirections.None;
        _canIncrease = false;
        _canDecrease = false;
        if (!IsReadOnly)
        {
            if (Value is null)
            {
                validDirection = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
            }
            if (Value.HasValue && Value.Value.CompareTo(Maximum) < 0)
            {
                validDirection |= ValidSpinDirections.Increase;
                _canIncrease = true;
            }

            if (Value.HasValue && Value.Value.CompareTo(Minimum) > 0)
            {
                validDirection |= ValidSpinDirections.Decrease;
                _canDecrease = true;
            }
        }
        if (_spinner != null)
        {
            _spinner.ValidSpinDirection = validDirection;
        }
    }


    protected override void Increase()
    {
        T? value;
        if (Value is not null)
        {
            value = Add(Value.Value, Step);
        }
        else
        {
            value = IsSet(MinimumProperty) ? Minimum : Zero;
        }
        SetCurrentValue(ValueProperty, Clamp(value, Maximum, Minimum));
    }

    protected override void Decrease()
    {
        T? value;
        if (Value is not null)
        {
            value = Minus(Value.Value, Step);
        }
        else
        {
            value = IsSet(MaximumProperty) ? Maximum : Zero;
        }

        SetCurrentValue(ValueProperty, Clamp(value, Maximum, Minimum));
    }

    protected bool isReading = false;
    protected override void OnRead(object sender, RoutedEventArgs e)
    {
        isReading = true;
        e = new RoutedEventArgs(ReadRequestedEvent, this);
        RaiseReadEventCommand(e);
        isReading = false;
    }

    protected override void OnWrite(object sender, RoutedEventArgs e)
    {
        var ve = new ValueChangedEventArgs<T>(ValueChangedEvent, Value, Value);
        RaiseEventCommand(ve);
    }

    protected override void OnHeaderDoubleTaped(object? sender, TappedEventArgs e)
    {
        e.RoutedEvent = HeaderDoubleTapedEvent;
        RaiseHeaderDoubleTapedEventCommand(e);
    }

    private bool _isSyncingTextAndValue;

    protected override bool SyncTextAndValue(bool fromTextToValue = false, string? text = null, bool forceTextUpdate = false)
    {
        if (_isSyncingTextAndValue) return true;
        _isSyncingTextAndValue = true;
        var parsedTextIsValid = true;
        try
        {
            if (fromTextToValue)
            {
                var ischanging = true;
                var newValue = PraseContentToValue(text, ref ischanging, ref parsedTextIsValid);
                if (parsedTextIsValid)
                {
                    SetCurrentValue(ValueProperty, newValue);
                }
            }

            if (!_updateFromTextInput)
            {
                if (forceTextUpdate)
                {
                    var newText = ConvertValueToText(Value);
                    if (_textBox != null && !Equals(_textBox.Text, newText))
                    {
                        _textBox.Text = newText;
                        _textBox.CaretIndex = newText?.Length ?? 0;
                    }
                }
            }

            if (_updateFromTextInput && !parsedTextIsValid)
            {
                if (_spinner is not null)
                {
                    _spinner.ValidSpinDirection = ValidSpinDirections.None;
                }
            }
            else
            {
                SetValidSpinDirection();
            }
        }
        finally
        {
            _isSyncingTextAndValue = false;
        }
        return parsedTextIsValid;
    }

    protected override void CheckContextIsChangedAndValid(string? text, ref bool isediting, ref bool valid)
    {
        var lv = PraseContentToValue(text, ref isediting, ref valid);
        if (valid)
        {
            LastEditingValidValue = lv;
        }
    }

    protected T? PraseContentToValue(string? text, ref bool haveChanged, ref bool parsedTextIsValid)
    {
        T? newValue = default(T);
        haveChanged = true;
        parsedTextIsValid = true;
        try
        {
            newValue = ConvertTextToValue(text);
            if (!Equals(newValue, Value))
            {
                if (Equals(Clamp(newValue, Maximum, Minimum), newValue)) // check and same
                {
                    parsedTextIsValid = true;
                }
                else
                {
                    parsedTextIsValid = false;
                }
            }
            else
            {
                haveChanged = false;
            }
        }
        catch
        {
            parsedTextIsValid = false;
        }

        return newValue;
    }

    protected virtual T? ConvertTextToValue(string? text)
    {
        T? result;
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidDataException("Input string IsNullOrWhiteSpace.");
        }
        if (TextConverter != null)
        {
            var valueFromText = TextConverter.Convert(text, typeof(T?), null, CultureInfo.CurrentCulture);
            return (T?)valueFromText;
        }
        else
        {
            text = TrimString(text, ParsingNumberStyle);
            if (!ParseText(text, out var outputValue))
            {
                throw new InvalidDataException("Input string was not in a correct format.");
            }

            result = outputValue;
        }
        return result;
    }

    protected virtual string? ConvertValueToText(T? value)
    {
        if (TextConverter is not null)
        {
            return TextConverter.ConvertBack(Value, typeof(int), null, CultureInfo.CurrentCulture)?.ToString();
        }

        if (FormatString.Contains("{0"))
        {
            return string.Format(NumberFormat, FormatString, value);
        }

        return ValueToString(value);
    }


    protected abstract bool ParseText(string? text, out T number);
    protected abstract string? ValueToString(T? value);
    protected abstract T Zero { get; }
    protected abstract T? Add(T? a, T? b);
    protected abstract T? Minus(T? a, T? b);

    public override void Clear()
    {
        // SetCurrentValue(ValueProperty, EmptyInputValue);
        SyncTextAndValue(false, forceTextUpdate: true);
    }
}