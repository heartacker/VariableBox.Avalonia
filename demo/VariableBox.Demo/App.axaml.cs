using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using VariableBox.Demo.ViewModels;
using VariableBox.Demo.Views;
using VariableBox.Demo.Pages;

namespace VariableBox.Demo;

public partial class App : Application
{
    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
                Height = 720,
                Width = 960,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView() { DataContext = new MainViewViewModel() };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
