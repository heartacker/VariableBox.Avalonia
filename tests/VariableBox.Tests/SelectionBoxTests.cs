using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using Avalonia.Threading;
using System.Linq;
using VariableBox;

namespace VariableBox.Tests;

public class SelectionBoxTests
{
    public enum TestEnum
    {
        OptionA,
        OptionB,
        OptionC
    }

    private class TestEnumUpDown : EnumerationUpDown<TestEnum> { }

    [AvaloniaFact]
    public void EnumerationUpDown_Should_Cycle_Through_Values()
    {
        var control = new TestEnumUpDown();
        var window = new Window { Content = control };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        
        var textBox = control.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
        Assert.NotNull(textBox);
        
        // Initial state
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(TestEnum.OptionA, control.SelectedItem);
        Assert.Equal("OptionA", textBox.Text);
        
        // Step up
        control.Increase();
        Assert.Equal(1, control.SelectedIndex);
        Assert.Equal(TestEnum.OptionB, control.SelectedItem);
        Assert.Equal("OptionB", textBox.Text);
        
        // Wrap test
        control.Wrap = true;
        control.Increase(); // B -> C
        control.Increase(); // C -> A (wrap)
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(TestEnum.OptionA, control.SelectedItem);
    }
}
