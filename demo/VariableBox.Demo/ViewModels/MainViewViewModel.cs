using CommunityToolkit.Mvvm.Messaging;

namespace VariableBox.Demo.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    // public MenuViewModel Menus { get; set; } = new MenuViewModel();

    private object? _content;

    public object? Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public MainViewViewModel()
    {
        WeakReferenceMessenger.Default.Register<MainViewViewModel, string>(this, OnNavigation);
    }


    private void OnNavigation(MainViewViewModel vm, string s)
    {
        Content = s switch
        {
            // MenuKeys.MenuKeyNumericUpDown => new NumericUpDownDemoViewModel(),
        };
    }
}