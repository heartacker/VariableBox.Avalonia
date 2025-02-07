using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
using VariableBox.Controls;

namespace VariableBox.Demo.ViewModels;

public partial class NumericUpDownDemoViewModel : ObservableObject
{
    private double _oldWidth = 200;
    [ObservableProperty]
    public partial string? UserName { get; set; } = "Heartacker";

    [ObservableProperty] private bool _AutoWidth = false;
    [ObservableProperty] private double _Width = 200;//Double.NaN;
    [ObservableProperty] private uint _Value;
    [ObservableProperty] private string _FontFamily = "Consolas";
    [ObservableProperty] private bool _IsAllowDrag = false;
    [ObservableProperty] private bool _IsReadOnly = false;

    [ObservableProperty] private Array _Array_HorizontalAlignment;
    [ObservableProperty] private HorizontalAlignment _HorizontalAlignment = HorizontalAlignment.Center;

    [ObservableProperty] private Array _Array_HorizontalContentAlignment;
    [ObservableProperty] private HorizontalAlignment _HorizontalContentAlignment = HorizontalAlignment.Center;
    [ObservableProperty] private object? _HeaderContent = "0x";
    [ObservableProperty] private string _Watermark = "Water mark";
    [ObservableProperty] private string _FormatString = "X8";
    [ObservableProperty] private Array _Array_ParsingNumberStyle;
    [ObservableProperty] private NumberStyles _ParsingNumberStyle = NumberStyles.AllowHexSpecifier;
    [ObservableProperty] private bool _IsAllowSpin = true;
    [ObservableProperty] private bool _ShowButtonSpinner = true;

    [ObservableProperty] private UInt32 _Maximum = UInt32.MaxValue;
    [ObservableProperty] private UInt32 _Minimum = UInt32.MinValue;
    [ObservableProperty] private UInt32 _Step = 1;

    [ObservableProperty] private bool _IsEnable = true;

    [ObservableProperty] private bool _IsUpdateValueWhenLostFocus = false;

    [ObservableProperty] private string _CommandUpdateText = "Command not Execute";

    [ObservableProperty] private string _ValueChangedUpdateText = "ValueChanged not Execute";

    [ObservableProperty] private string _ReadCommandUpdateText = "ReadCommand not Execute";
    [ObservableProperty] private string _ReadRequestedUpdateText = "ReadRequested not Execute";

    [ObservableProperty] private bool _IsShowReadButton = true;
    [ObservableProperty] private bool _IsShowWriteButton = true;
    [ObservableProperty] private bool _IsEnableEditingIndicator = true;


    uint v = 0;
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

    public NumericUpDownDemoViewModel()
    {
        Array_HorizontalContentAlignment = Enum.GetValues(typeof(HorizontalAlignment));
        Array_HorizontalAlignment = Enum.GetValues(typeof(HorizontalAlignment));
        Array_ParsingNumberStyle = Enum.GetValues(typeof(NumberStyles));
        // VariableBoxUInt numericUIntUpDown;
        // TextBox textBox;

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

}