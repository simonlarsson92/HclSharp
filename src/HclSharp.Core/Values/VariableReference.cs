using HclSharp.Core.Validation;

namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform variable reference (e.g., var.vsphere_user).
/// </summary>
public record VariableReference : TerraformValue
{
    /// <summary>
    /// The name of the variable (without the 'var.' prefix)
    /// </summary>
    public string VariableName { get; init; }

    /// <summary>
    /// Initializes a new VariableReference with validation.
    /// </summary>
    /// <param name="variableName">The name of the variable</param>
    public VariableReference(string variableName)
    {
        TerraformIdentifierValidator.ValidateIdentifier(variableName, nameof(variableName));
        VariableName = variableName;
    }
}
