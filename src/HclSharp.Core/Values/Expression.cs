using System.Text.RegularExpressions;

namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform expression (e.g., "64 * 1024" or "length(var.list)").
/// The expression is rendered as-is without quoting.
/// </summary>
public record Expression : TerraformValue
{
    /// <summary>
    /// Pattern to detect potentially dangerous HCL injection attempts.
    /// Matches three patterns:
    /// 1. Closing brace followed by newline(s) and a Terraform block declaration
    /// 2. Newline followed by Terraform block declaration
    /// 3. String start with Terraform block declaration
    /// A block declaration is: keyword followed by quoted string(s) or opening brace.
    /// </summary>
    private static readonly Regex DangerousPatternRegex = new(
        @"(?:(?:}|^)[\r\n]+|^)\s*(?:resource|data|provider|terraform|module|output|locals|variable)\s+[""'{]",
        RegexOptions.Compiled | RegexOptions.Multiline);

    /// <summary>
    /// Gets the expression string to render.
    /// </summary>
    public string ExpressionString { get; init; }

    /// <summary>
    /// Initializes a new Expression with validation.
    /// </summary>
    /// <param name="expressionString">The expression string to render</param>
    public Expression(string expressionString)
    {
        ArgumentNullException.ThrowIfNull(expressionString);
        
        if (string.IsNullOrWhiteSpace(expressionString))
        {
            throw new ArgumentException("Expression string cannot be empty or whitespace.", nameof(expressionString));
        }

        // Check for dangerous patterns that could break HCL context
        if (DangerousPatternRegex.IsMatch(expressionString))
        {
            throw new ArgumentException(
                "Expression contains potentially dangerous characters or keywords that could break HCL syntax. " +
                "Expressions must not contain patterns that could inject new Terraform blocks.",
                nameof(expressionString));
        }

        ExpressionString = expressionString;
    }
}
