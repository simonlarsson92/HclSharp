namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform expression (e.g., "64 * 1024" or "length(var.list)").
/// The expression is rendered as-is without quoting.
/// </summary>
/// <param name="ExpressionString">The expression string to render</param>
public record Expression(string ExpressionString) : TerraformValue;
