using System;
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

namespace VariableBox;

/// <summary>
/// Non-generic base class for NumericUpDown controls, handling UI and template logic.
/// </summary>
[TemplatePart(PART_Spinner, typeof(ButtonSpinner))]
[TemplatePart(PART_TextBox, typeof(TextBox))]
[TemplatePart(PART_DragPanel, typeof(Panel))]
[TemplatePart(PART_RepeatRead, typeof(RepeatButton))]
[TemplatePart(PART_RepeatWrite, typeof(RepeatButton))]
public abstract partial class NumericUpDown : TemplatedControl
{
    public const string PART_Spinner = "PART_Spinner";
    public const string PART_TextBox = "PART_TextBox";
    public const string PART_DragPanel = "PART_DragPanel";
    public const string PART_RepeatRead = "PART_RepeatRead";
    public const string PART_RepeatWrite = "PART_RepeatWrite";

    protected internal ButtonSpinner? _spinner;
    protected internal TextBox? _textBox;
    protected internal Panel? _dragPanel;
    protected internal RepeatButton? _repeatReadButton;
    protected internal RepeatButton? _repeatWriteButton;

    protected internal bool _canIncrease = true;
    protected internal bool _canDecrease = true;
    private VariableBox.Behaviors.DragValueBehavior? _dragBehavior;
    protected internal bool _isSyncing;

    public static readonly StyledProperty<bool> IsAllowDragProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsAllowDrag), true);

    public bool IsAllowDrag
    {
        get => GetValue(IsAllowDragProperty);
        set => SetValue(IsAllowDragProperty, value);
    }

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsReadOnly), false);

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly StyledProperty<NumberFormatInfo?> NumberFormatProperty =
        AvaloniaProperty.Register<NumericUpDown, NumberFormatInfo?>(nameof(NumberFormat), NumberFormatInfo.CurrentInfo);

    public NumberFormatInfo? NumberFormat
    {
        get => GetValue(NumberFormatProperty);
        set => SetValue(NumberFormatProperty, value);
    }

    public static readonly StyledProperty<string> FormatStringProperty =
        AvaloniaProperty.Register<NumericUpDown, string>(nameof(FormatString), "");

    public string FormatString
    {
        get => GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public static readonly StyledProperty<NumberStyles> ParsingNumberStyleProperty =
        AvaloniaProperty.Register<NumericUpDown, NumberStyles>(nameof(ParsingNumberStyle), NumberStyles.Any);

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

    public static readonly StyledProperty<bool> IsUpdateValueWhenLostFocusProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(IsUpdateValueWhenLostFocus), false);

    public bool IsUpdateValueWhenLostFocus
    {
        get => GetValue(IsUpdateValueWhenLostFocusProperty);
        set => SetValue(IsUpdateValueWhenLostFocusProperty, value);
    }

    public static readonly DirectProperty<NumericUpDown, bool> IsEditingProperty =
        AvaloniaProperty.RegisterDirect<NumericUpDown, bool>(nameof(IsEditing), o => o.IsEditing, (o, v) => o.IsEditing = v);

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        protected set => SetAndRaise(IsEditingProperty, ref _isEditing, value);
    }

    public static readonly DirectProperty<NumericUpDown, bool> IsEditingValidProperty =
        AvaloniaProperty.RegisterDirect<NumericUpDown, bool>(nameof(IsEditingValid), o => o.IsEditingValid, (o, v) => o.IsEditingValid = v);

    private bool _isEditingValid = true;
    public bool IsEditingValid
    {
        get => _isEditingValid;
        protected set => SetAndRaise(IsEditingValidProperty, ref _isEditingValid, value);
    }

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
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<bool> ShowButtonSpinnerProperty =
        AvaloniaProperty.Register<NumericUpDown, bool>(nameof(ShowButtonSpinner), true);

    public bool ShowButtonSpinner
    {
        get => GetValue(ShowButtonSpinnerProperty);
        set => SetValue(ShowButtonSpinnerProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        AvaloniaProperty.Register<NumericUpDown, HorizontalAlignment>(nameof(HorizontalContentAlignment), HorizontalAlignment.Left);

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
        set => SetValue(IsReadWriteButtonShowProperty, value);
    }

    public static readonly StyledProperty<ICommand?> ReadCommandProperty =
        AvaloniaProperty.Register<NumericUpDown, ICommand?>(nameof(ReadCommand));

    public ICommand? ReadCommand
    {
        get => GetValue(ReadCommandProperty);
        set => SetValue(ReadCommandProperty, value);
    }

    public static readonly RoutedEvent<RoutedEventArgs> ReadRequestedEvent =
        RoutedEvent.Register<NumericUpDown, RoutedEventArgs>("ReadRequested", RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? ReadRequested
    {
        add => AddHandler(ReadRequestedEvent, value);
        remove => RemoveHandler(ReadRequestedEvent, value);
    }

    public static readonly RoutedEvent<TappedEventArgs> HeaderDoubleTapedEvent =
        RoutedEvent.Register<NumericUpDown, TappedEventArgs>("HeaderDoubleTaped", RoutingStrategies.Bubble);

    public event EventHandler<TappedEventArgs>? HeaderDoubleTaped
    {
        add => AddHandler(HeaderDoubleTapedEvent, value);
        remove => RemoveHandler(HeaderDoubleTapedEvent, value);
    }

    static NumericUpDown()
    {
        IsEditingProperty.Changed.AddClassHandler<NumericUpDown>((o, e) => o.PseudoClasses.Set(":editing", (bool)e.NewValue!));
        IsEditingValidProperty.Changed.AddClassHandler<NumericUpDown>((o, e) => o.PseudoClasses.Set(":invalid", !(bool)e.NewValue!));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _spinner = e.NameScope.Find<ButtonSpinner>(PART_Spinner);
        if (_spinner != null)
        {
            _spinner.Spin += (s, args) =>
            {
                if (!IsEditing && IsAllowSpin && !IsReadOnly)
                {
                    args.Handled = true;
                    if (args.Direction == SpinDirection.Increase) Increase();
                    else Decrease();
                }
            };
        }

        _textBox = e.NameScope.Find<TextBox>(PART_TextBox);
        if (_textBox != null)
        {
            _textBox.IsReadOnly = IsReadOnly;
            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.KeyDown += OnTextBoxKeyDown;
            _textBox.KeyDown += (s, args) =>
            {
                if (args.KeyModifiers == KeyModifiers.Control)
                {
                    if (args.Key == Key.Z)
                    {
                        UndoInternal();
                        args.Handled = true;
                    }
                    else if (args.Key == Key.Y)
                    {
                        RedoInternal();
                        args.Handled = true;
                    }
                }
            };
            _textBox.LostFocus += OnTextBoxLostFocus;
        }

        _dragPanel = e.NameScope.Find<Panel>(PART_DragPanel);
        if (_dragPanel != null)
        {
            _dragPanel.IsVisible = IsAllowDrag;
            if (_textBox != null)
            {
                _dragBehavior?.Detach();
                _dragBehavior = new VariableBox.Behaviors.DragValueBehavior(this, _dragPanel, _textBox);
                _dragBehavior.Attach();
            }
        }

        _repeatReadButton = e.NameScope.Find<RepeatButton>(PART_RepeatRead);
        _repeatWriteButton = e.NameScope.Find<RepeatButton>(PART_RepeatWrite);

        if (_repeatReadButton != null) _repeatReadButton.Click += OnReadInternal;
        if (_repeatWriteButton != null) _repeatWriteButton.Click += OnWriteInternal;

        SyncTextAndValue(false, null, true);
        SetValidSpinDirection();
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isSyncing) return;
        CheckContextIsChangedAndValid(_textBox?.Text, out bool isEditing, out bool isEditingValid);
        IsEditing = isEditing;
        IsEditingValid = isEditingValid;
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommitInput(true);
            IsEditing = false;
            e.Handled = true;
        }
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        if (IsUpdateValueWhenLostFocus)
        {
            CommitInput(true);
        }
        IsEditing = false;
    }

    private void OnReadInternal(object? sender, RoutedEventArgs e) => OnRead();
    private void OnWriteInternal(object? sender, RoutedEventArgs e)
    {
        if (CommitInput(true)) OnWrite();
    }

    protected virtual void UndoInternal() { }
    protected virtual void RedoInternal() { }

    protected abstract void SetValidSpinDirection();
    public abstract void Increase();
    public abstract void Decrease();
    protected virtual bool CommitInput(bool forceUpdate = false) => SyncTextAndValue(true, _textBox?.Text, forceUpdate);
    protected abstract bool SyncTextAndValue(bool fromText, string? text, bool forceUpdate);
    protected abstract void CheckContextIsChangedAndValid(string? text, out bool isEditing, out bool isEditingValid);
    protected abstract void OnRead();
    protected abstract void OnWrite();
    public abstract void Clear();
}
