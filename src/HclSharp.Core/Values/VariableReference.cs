namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform variable reference (e.g., var.vsphere_user).
/// </summary>
/// <param name="VariableName">The name of the variable (without the 'var.' prefix)</param>
public record VariableReference(string VariableName) : TerraformValue;
