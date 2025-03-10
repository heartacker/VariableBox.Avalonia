using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VariableBox.Demo.ViewModels;

namespace VariableBox.Demo.Pages;

public partial class ManyVariableBox : UserControl
{
    public ManyVariableBox()
    {
        InitializeComponent();
        DataContext = new ManyVariableBoxViewModel();
    }
}