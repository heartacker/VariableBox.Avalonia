using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Globalization;
using VariableBox.Controls;
using VariableBox.Demo.ViewModels.Messages;

namespace VariableBox.Demo.ViewModels;

public partial class NumericUpDownDemoViewModel : ObservableRecipient, IRecipient<UIntValueChangedMessage>, IRecipient<UIntRequestMessage>
{
    public NumericUpDownDemoViewModel()
    {
        Array_HorizontalContentAlignment = Enum.GetValues(typeof(HorizontalAlignment));
        Array_HorizontalAlignment = Enum.GetValues(typeof(HorizontalAlignment));
        Array_ParsingNumberStyle = Enum.GetValues(typeof(NumberStyles));
        // VariableBoxUInt numericUIntUpDown;
        // TextBox textBox;

        //! 不是接收者，就需要自己注册
        // WeakReferenceMessenger.Default.Register<UIntValueChangedMessage>(this); // 需要注册自己
        // WeakReferenceMessenger.Default.Register<UIntRequestMessage>(this);      // 需要注册自己

        //! 或者使用 ObservableRecipient, 并激活自己
        IsActive = true;
    }

    private double _oldWidth = 200;
    [ObservableProperty]
    public partial string? UserName { get; set; } = "Heartacker";

    [ObservableProperty]
    public partial bool AutoWidth { get; set; } = false;
    [ObservableProperty]
    public partial double Width { get; set; } = 200;//Double.NaN;

    [ObservableProperty]
    public partial uint Value { get; set; }

    [ObservableProperty]
    public partial string FontFamily { get; set; } = "Consolas";
    [ObservableProperty]
    public partial bool IsAllowDrag { get; set; } = false;
    [ObservableProperty]
    public partial bool IsReadOnly { get; set; } = false;

    [ObservableProperty]
    public partial Array Array_HorizontalAlignment { get; set; }
    [ObservableProperty]
    public partial HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

    [ObservableProperty]
    public partial Array Array_HorizontalContentAlignment { get; set; }
    [ObservableProperty]
    public partial HorizontalAlignment HorizontalContentAlignment { get; set; } = HorizontalAlignment.Center;
    [ObservableProperty]
    public partial object? HeaderContent { get; set; } = "0x";
    [ObservableProperty]
    public partial string Watermark { get; set; } = "Water mark";
    [ObservableProperty]
    public partial string FormatString { get; set; } = "X8";
    [ObservableProperty]
    public partial Array Array_ParsingNumberStyle { get; set; }
    [ObservableProperty]
    public partial NumberStyles ParsingNumberStyle { get; set; } = NumberStyles.AllowHexSpecifier;
    [ObservableProperty]
    public partial bool IsAllowSpin { get; set; } = true;
    [ObservableProperty]
    public partial bool ShowButtonSpinner { get; set; } = true;

    [ObservableProperty]
    public partial UInt32 Maximum { get; set; } = UInt32.MaxValue;
    [ObservableProperty]
    public partial UInt32 Minimum { get; set; } = UInt32.MinValue;
    [ObservableProperty]
    public partial UInt32 Step { get; set; } = 1;

    [ObservableProperty]
    public partial bool IsEnable { get; set; } = true;

    [ObservableProperty]
    public partial bool IsUpdateValueWhenLostFocus { get; set; } = false;

    [ObservableProperty]
    public partial string CommandUpdateText { get; set; } = "Command not Execute";

    [ObservableProperty]
    public partial string ValueChangedUpdateText { get; set; } = "ValueChanged not Execute";

    [ObservableProperty]
    public partial string ReadCommandUpdateText { get; set; } = "ReadCommand not Execute";

    [ObservableProperty]
    public partial string ReadRequestedUpdateText { get; set; } = "ReadRequested not Execute";

    [ObservableProperty]
    public partial bool IsShowReadButton { get; set; } = true;

    [ObservableProperty]
    public partial bool IsShowWriteButton { get; set; } = true;
    // [ObservableProperty] private bool _IsEnableEditingIndicator = true;


    // uint v = 0;
    [RelayCommand]
    // void Trythis()
    // void Trythis(object v)
    void Trythis(uint v)
    {
        CommandUpdateText = $"Command, Parameter={v}";
    }

    [RelayCommand]
    // void TrythisRead()
    // void TrythisRead(object v)
    void TrythisRead(uint v)
    {
        ReadCommandUpdateText = $"ReadCommand, Parameter={v}";
    }

    partial void OnAutoWidthChanged(bool value)
    {
        if (value)
        {
            _oldWidth = Width;
            Width = double.NaN;
        }
        else
        {
            Width = _oldWidth;
        }
    }

    partial void OnValueChanging(uint oldValue, uint newValue)
    {
        // Console.WriteLine(oldValue);
    }



    #region Messages

    [RelayCommand]
    void TrythisMessageRequest(uint v)
    {
        CommandUpdateText = $"TrythisMessage, Parameter={v}";
    }

    public void Receive(UIntValueChangedMessage message)
    {
        Value = message.Value;
        // ValueChangedUpdateText = $"Receive, Parameter={message.Value}";
    }

    public void Receive(UIntRequestMessage message)
    {
        message.Reply((uint)(Value * message.t));
    }
    #endregion

}