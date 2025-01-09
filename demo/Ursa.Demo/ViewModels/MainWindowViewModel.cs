using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using CommunityToolkit.Mvvm.Input;

namespace VariableBox.Demo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainViewViewModel MainViewViewModel { get; set; } = new MainViewViewModel();

    [RelayCommand]
    private void ToggleTheme()
    {
        var app = Application.Current;
        if (app is null)
            return;
        var theme = app.ActualThemeVariant;
        app.RequestedThemeVariant =
            theme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    [RelayCommand]
    private void SwitchTheme(string themeName)
    {
        var app = Application.Current;
        if (app is null)
            return;

        Styles st = Application.Current.Styles[0] as Styles;
        app.Styles.Clear();

        switch (themeName)
        {
            case "Fluent":
                st = new FluentTheme();
                app.Styles.Add(st);
                app.Styles.Add(new VariableBox.Themes.FluentTheme());
                break;
            case "Simple":
                st = new SimpleTheme();
                app.Styles.Add(st);
                app.Styles.Add(new VariableBox.Themes.SimpleTheme());
                break;
            case "Semi":
                st = new Semi.Avalonia.SemiTheme();
                app.Styles.Add(st);
                app.Styles.Add(new VariableBox.Themes.SemiTheme());
                break;
        }

        Application.Current.Styles[0] = st;
    }
}
