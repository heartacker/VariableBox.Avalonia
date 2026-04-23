namespace VariableBox.Common;

public record ValidationResult(bool IsValid, string? Message = null)
{
    public static ValidationResult Success { get; } = new(true);
}

public interface IValidationRule<T> where T : struct
{
    ValidationResult Validate(T value);
}
