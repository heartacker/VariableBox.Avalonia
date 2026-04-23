using System;
using Avalonia;
using Avalonia.Dialogs;
using VariableBox.Demo;

namespace VariableBox.Demo.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseManagedSystemDialogs()
            .UsePlatformDetect()
            .LogToTrace();
}
