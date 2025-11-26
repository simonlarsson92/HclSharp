using System.Collections.Immutable;
using HclSharp.Core.Validation;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a resource block.
/// </summary>
public record ResourceBlockData
{
    /// <summary>
    /// Resource type (e.g., "vsphere_virtual_machine")
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Resource name (e.g., "windows_vm")
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Resource attributes
    /// </summary>
    public ImmutableDictionary<string, TerraformValue> Attributes { get; init; }

    /// <summary>
    /// Nested blocks within the resource
    /// </summary>
    public ImmutableList<NestedBlockData> NestedBlocks { get; init; }

    /// <summary>
    /// Initializes a new ResourceBlockData with validation.
    /// </summary>
    /// <param name="type">Resource type</param>
    /// <param name="name">Resource name</param>
    /// <param name="attributes">Resource attributes</param>
    /// <param name="nestedBlocks">Nested blocks</param>
    public ResourceBlockData(
        string type,
        string name,
        ImmutableDictionary<string, TerraformValue> attributes,
        ImmutableList<NestedBlockData> nestedBlocks)
    {
        TerraformIdentifierValidator.ValidateIdentifier(type, nameof(type));
        TerraformIdentifierValidator.ValidateIdentifier(name, nameof(name));
        ArgumentNullException.ThrowIfNull(attributes, nameof(attributes));
        ArgumentNullException.ThrowIfNull(nestedBlocks, nameof(nestedBlocks));

        // Validate all attribute keys
        foreach (var key in attributes.Keys)
        {
            TerraformIdentifierValidator.ValidateAttributeKey(key, nameof(attributes));
        }

        Type = type;
        Name = name;
        Attributes = attributes;
        NestedBlocks = nestedBlocks;
    }
}
