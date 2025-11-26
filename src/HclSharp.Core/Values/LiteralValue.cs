namespace HclSharp.Core.Values;

/// <summary>
/// Represents a literal value (string, number, boolean) in Terraform configuration.
/// </summary>
public record LiteralValue : TerraformValue
{
    /// <summary>
    /// The actual value (string, int, long, double, bool, etc.)
    /// </summary>
    public object Value { get; init; }

    /// <summary>
    /// Initializes a new LiteralValue with validation.
    /// </summary>
    /// <param name="value">The actual value</param>
    public LiteralValue(object value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        Value = value;
    }
}
