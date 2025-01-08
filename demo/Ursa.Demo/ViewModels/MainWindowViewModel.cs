using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;

namespace VariableBox.Demo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainViewViewModel MainViewViewModel { get; set; } = new MainViewViewModel();

    [RelayCommand]
    private void ToggleTheme()
    {
        var app = Application.Current;
        if (app is null) return;
        var theme = app.ActualThemeVariant;
        app.RequestedThemeVariant = theme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}