namespace VariableBox.Common;

public class ValidationResult
{
    public bool IsValid { get; }
    public string? Message { get; }

    public ValidationResult(bool isValid, string? message = null)
    {
        IsValid = isValid;
        Message = message;
    }

    public static ValidationResult Success { get; } = new(true);
}

public interface IValidationRule<T> where T : struct
{
    ValidationResult Validate(T value);
}
