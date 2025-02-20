using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using VariableBox.Demo.ViewModels;

namespace VariableBox.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //this.DataContext = new MainWindowViewModel();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        //{
        //    var app = Application.Current;
        //    if (app is null) return;
        //    var theme = app.ActualThemeVariant;
        //    app.RequestedThemeVariant = theme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        //}
    }
}