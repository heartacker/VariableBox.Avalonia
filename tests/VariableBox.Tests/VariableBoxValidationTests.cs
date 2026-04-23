using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using Avalonia.Threading;
using System.Linq;
using System.Collections.Generic;
using VariableBox;
using VariableBox.Common;

namespace VariableBox.Tests;

public class VariableBoxValidationTests
{
    private class MustBeEvenRule : IValidationRule<int>
    {
        public ValidationResult Validate(int value)
        {
            if (value % 2 != 0)
                return new ValidationResult(false, "Must be even");
            return ValidationResult.Success;
        }
    }

    [AvaloniaFact]
    public void Custom_Validation_Rule_Should_Block_Invalid_Value()
    {
        var control = new VariableBoxInt 
        { 
            Value = 10,
            ValidationRules = new List<IValidationRule<int>> { new MustBeEvenRule() }
        };
        var window = new Window { Content = control };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        
        var textBox = control.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
        Assert.NotNull(textBox);
        
        // Input an odd number
        textBox.Text = "11";
        Dispatcher.UIThread.RunJobs();
        
        // Should be invalid
        Assert.Contains(":invalid", control.Classes);
        Assert.False(control.IsEditingValid);
    }
}
