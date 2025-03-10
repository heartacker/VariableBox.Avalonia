using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VariableBox.Demo.ViewModels.Messages;

namespace VariableBox.Demo.ViewModels;

public partial class ManyVariableBoxViewModel : ObservableRecipient
{
    public ManyVariableBoxViewModel()
    {
        IsActive = true;
    }

    [ObservableProperty]
    public partial uint ValueHex { get; set; } = 0;

    [ObservableProperty]
    public partial uint Value2 { get; set; } = 0;

    partial void OnValue2Changing(uint oldValue, uint newValue)
    {
        Broadcast<uint>(oldValue, newValue, nameof(Value2)); // 一般配合 PropertyChangedMsssage 使用
    }


    [RelayCommand]
    public void Send()
    {
        // Send a message from some other module
        WeakReferenceMessenger.Default.Send(new UIntValueChangedMessage(ValueHex));
    }

    [RelayCommand]
    public void Read()
    {
        UIntRequestMessage msg = new UIntRequestMessage(1.5);
        var v = WeakReferenceMessenger.Default.Send(msg);
        ValueHex = v.Response;
    }
}