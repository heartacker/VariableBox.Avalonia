using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace VariableBox.Demo.ViewModels;

public partial class ManyVariableBoxViewModel : ObservableRecipient
{
    public ManyVariableBoxViewModel()
    {
        // IsActive = true;
    }

    [ObservableProperty]
    public partial uint ValueHex { get; set; } = 0;


    [RelayCommand]
    public void Send()
    {
        // Send a message from some other module
        Broadcast<uint>(0, ValueHex, "demo");
        WeakReferenceMessenger.Default.Send(new UIntValueChangedMessage(ValueHex));
    }

    [RelayCommand]
    public void Read()
    {
        // Send a message from some other module
        // WeakReferenceMessenger.Default.Send(new UIntValueChangedMessage(ValueHex));
    }
}