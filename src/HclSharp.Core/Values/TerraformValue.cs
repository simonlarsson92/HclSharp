namespace HclSharp.Core.Values;

/// <summary>
/// Base abstract record for all Terraform values.
/// Values can be literals, variable references, data references, resource references, or expressions.
/// </summary>
public abstract record TerraformValue
{
    // Marker base class for all value types
    // Derived classes will define specific value behaviors
}
