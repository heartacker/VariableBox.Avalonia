using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using VariableBox.Controls;
using VariableBox.Demo.ViewModels;

namespace VariableBox.Demo.Pages;

public partial class NumericUpDownDemo : UserControl
{
    NumericUpDownDemoViewModel vm;
    public NumericUpDownDemo()
    {
        InitializeComponent();
        DataContext = vm = new NumericUpDownDemoViewModel();
        numd.ValueChanged += Numd_ValueChanged;
        numd.ReadRequested += Numd_ReadRequested;
        //numd.HeaderDoubleTaped += Numd_HeaderDoubleTaped;
        numd.FontFamily= "Consolas";
    }

    bool hex_normal = true;
    private void Numd_HeaderDoubleTaped(object? sender, TappedEventArgs e)
    {
        hex_normal = !hex_normal;
        if (hex_normal)
        {
            vm.HeaderContent = "0x";
            vm.ParsingNumberStyle = System.Globalization.NumberStyles.AllowHexSpecifier;
            vm.FormatString = "{0:X8}"; // if write in axaml, please use {} frist。but in code, you dont need to add {}
        }
        else
        {
            vm.HeaderContent = "v:";
            vm.ParsingNumberStyle = System.Globalization.NumberStyles.Any;
            vm.FormatString = "{0:D10}";
        }
    }

    Random random = new Random();
    private void Numd_ReadRequested(object? sender, RoutedEventArgs e)
    {
        Trace.WriteLine(e.Source);
        Trace.WriteLine(sender as VariableBoxUInt);
        var val = (sender as VariableBoxUInt).Value;
        var a = (uint)random.Next(0, 100);
        vm.ReadRequestedUpdateText = $"ReadRequested,Old={val} => {a}";
        vm.Value = a;
    }


    int i = 0;
    private void Numd_ValueChanged(object? sender, ValueChangedEventArgs<uint> e)
    {
        vm.ValueChangedUpdateText = $"ValueChanged_{i++},Old={e.OldValue} => {e.NewValue}";
    }
}