using System.Collections.Immutable;
using HclSharp.Core.Validation;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a provider configuration block.
/// </summary>
public record ProviderBlockData
{
    /// <summary>
    /// Provider name (e.g., "vsphere")
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Provider configuration attributes
    /// </summary>
    public ImmutableDictionary<string, TerraformValue> Attributes { get; init; }

    /// <summary>
    /// Initializes a new ProviderBlockData with validation.
    /// </summary>
    /// <param name="name">Provider name</param>
    /// <param name="attributes">Provider configuration attributes</param>
    public ProviderBlockData(string name, ImmutableDictionary<string, TerraformValue> attributes)
    {
        TerraformIdentifierValidator.ValidateIdentifier(name, nameof(name));
        ArgumentNullException.ThrowIfNull(attributes, nameof(attributes));

        // Validate all attribute keys
        foreach (var key in attributes.Keys)
        {
            TerraformIdentifierValidator.ValidateAttributeKey(key, nameof(attributes));
        }

        Name = name;
        Attributes = attributes;
    }
}
