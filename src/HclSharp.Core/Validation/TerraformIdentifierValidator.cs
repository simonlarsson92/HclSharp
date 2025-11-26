using System.Text.RegularExpressions;

namespace HclSharp.Core.Validation;

/// <summary>
/// Provides validation for Terraform identifiers (block names, attribute keys, resource types, etc).
/// </summary>
public static class TerraformIdentifierValidator
{
    /// <summary>
    /// Valid Terraform identifier pattern: must start with a letter or underscore,
    /// followed by letters, digits, underscores, or hyphens.
    /// </summary>
    private static readonly Regex ValidIdentifierRegex = new(
        @"^[a-zA-Z_][a-zA-Z0-9_-]*$",
        RegexOptions.Compiled);

    /// <summary>
    /// Pattern to detect dangerous characters that could break HCL syntax.
    /// Includes: newlines, quotes, braces, special characters.
    /// </summary>
    private static readonly Regex DangerousCharactersRegex = new(
        @"[\r\n\{\}\""\']",
        RegexOptions.Compiled);

    /// <summary>
    /// Validates a Terraform identifier (resource name, block name, attribute key, etc).
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <param name="parameterName">The name of the parameter (for exception messages)</param>
    /// <param name="allowEmpty">Whether to allow empty strings (default: false)</param>
    /// <exception cref="ArgumentNullException">Thrown when identifier is null</exception>
    /// <exception cref="ArgumentException">Thrown when identifier is invalid</exception>
    public static void ValidateIdentifier(string identifier, string parameterName, bool allowEmpty = false)
    {
        ArgumentNullException.ThrowIfNull(identifier, parameterName);

        if (string.IsNullOrWhiteSpace(identifier))
        {
            if (allowEmpty && identifier == string.Empty)
            {
                return;
            }
            throw new ArgumentException("Identifier cannot be empty or whitespace.", parameterName);
        }

        // Check for dangerous characters first
        if (DangerousCharactersRegex.IsMatch(identifier))
        {
            throw new ArgumentException(
                $"Identifier '{identifier}' contains invalid characters. " +
                "Identifiers cannot contain newlines, quotes, or braces.",
                parameterName);
        }

        // Validate against Terraform identifier rules
        if (!ValidIdentifierRegex.IsMatch(identifier))
        {
            throw new ArgumentException(
                $"Identifier '{identifier}' is not a valid Terraform identifier. " +
                "Identifiers must start with a letter or underscore, followed by letters, digits, underscores, or hyphens.",
                parameterName);
        }
    }

    /// <summary>
    /// Validates an attribute key (more permissive than identifiers, allows dots for nested paths).
    /// </summary>
    /// <param name="attributeKey">The attribute key to validate</param>
    /// <param name="parameterName">The name of the parameter (for exception messages)</param>
    /// <exception cref="ArgumentNullException">Thrown when attributeKey is null</exception>
    /// <exception cref="ArgumentException">Thrown when attributeKey is invalid</exception>
    public static void ValidateAttributeKey(string attributeKey, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(attributeKey, parameterName);

        if (string.IsNullOrWhiteSpace(attributeKey))
        {
            throw new ArgumentException("Attribute key cannot be empty or whitespace.", parameterName);
        }

        // Check for dangerous characters (allow dots for nested paths)
        if (Regex.IsMatch(attributeKey, @"[\r\n\{\}\""\']"))
        {
            throw new ArgumentException(
                $"Attribute key '{attributeKey}' contains invalid characters. " +
                "Attribute keys cannot contain newlines, quotes, or braces.",
                parameterName);
        }

        // Each segment should be a valid identifier
        var segments = attributeKey.Split('.');
        foreach (var segment in segments)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                throw new ArgumentException(
                    $"Attribute key '{attributeKey}' contains empty segments.",
                    parameterName);
            }

            if (!ValidIdentifierRegex.IsMatch(segment))
            {
                throw new ArgumentException(
                    $"Attribute key segment '{segment}' in '{attributeKey}' is not valid. " +
                    "Each segment must start with a letter or underscore, followed by letters, digits, underscores, or hyphens.",
                    parameterName);
            }
        }
    }

    /// <summary>
    /// Validates a provider source (e.g., "hashicorp/aws").
    /// </summary>
    /// <param name="source">The provider source to validate</param>
    /// <param name="parameterName">The name of the parameter (for exception messages)</param>
    /// <exception cref="ArgumentNullException">Thrown when source is null</exception>
    /// <exception cref="ArgumentException">Thrown when source is invalid</exception>
    public static void ValidateProviderSource(string source, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(source, parameterName);

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Provider source cannot be empty or whitespace.", parameterName);
        }

        // Check for dangerous characters
        if (DangerousCharactersRegex.IsMatch(source))
        {
            throw new ArgumentException(
                $"Provider source '{source}' contains invalid characters. " +
                "Provider sources cannot contain newlines, quotes, or braces.",
                parameterName);
        }

        // Provider sources should be in format: [hostname/]namespace/name
        // For simplicity, we'll just ensure it has at least namespace/name
        if (!source.Contains('/'))
        {
            throw new ArgumentException(
                $"Provider source '{source}' is not valid. " +
                "Provider sources must be in format 'namespace/name' or 'hostname/namespace/name'.",
                parameterName);
        }
    }
}
