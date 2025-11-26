namespace HclSharp.Core.Values;

/// <summary>
/// Represents a literal value (string, number, boolean) in Terraform configuration.
/// </summary>
/// <param name="Value">The actual value (string, int, long, double, bool, etc.)</param>
public record LiteralValue(object Value) : TerraformValue;
