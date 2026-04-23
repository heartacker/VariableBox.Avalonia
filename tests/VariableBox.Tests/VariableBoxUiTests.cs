using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Threading;
using System.Linq;
using System.Threading.Tasks;
using VariableBox;

namespace VariableBox.Tests;

public class VariableBoxUiTests
{
    [AvaloniaFact]
    public void Value_Change_Should_Update_TextBox_Text()
    {
        var control = new VariableBoxInt { Value = 10 };
        var window = new Window { Content = control };
        window.Show();
        
        // Ensure all UI tasks and bindings are processed
        Dispatcher.UIThread.RunJobs();
        
        var textBox = control.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
        
        Assert.NotNull(textBox);
        
        // Wait for potential async binding/syncing
        Assert.Equal("10", textBox.Text);
        
        control.Value = 20;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("20", textBox.Text);
    }
}
