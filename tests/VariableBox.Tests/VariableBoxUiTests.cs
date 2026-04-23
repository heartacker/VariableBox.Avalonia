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

    [AvaloniaFact]
    public void Invalid_Input_Should_Set_Invalid_PseudoClass()
    {
        var control = new VariableBoxInt { Value = 10 };
        var window = new Window { Content = control };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        
        var textBox = control.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
        Assert.NotNull(textBox);
        
        // Simulate user typing invalid characters
        textBox.Text = "abc";
        Dispatcher.UIThread.RunJobs();
        
        Assert.Contains(":invalid", control.Classes);
        Assert.True(control.IsEditing);
    }

    [AvaloniaFact]
    public void Valid_Input_Update_Should_Set_Editing_But_Not_Invalid()
    {
        var control = new VariableBoxInt { Value = 10 };
        var window = new Window { Content = control };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        
        var textBox = control.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
        Assert.NotNull(textBox);
        
        // Simulate user changing value but not yet committing
        textBox.Text = "50";
        Dispatcher.UIThread.RunJobs();
        
        Assert.True(control.IsEditing);
        Assert.DoesNotContain(":invalid", control.Classes);
        // Underlying value should NOT change until committed
        Assert.Equal(10, control.Value);
    }
}
