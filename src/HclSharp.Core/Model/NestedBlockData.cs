using System.Collections.Immutable;
using HclSharp.Core.Validation;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a nested block within a resource or data source (e.g., network_interface, disk, clone).
/// </summary>
public record NestedBlockData
{
    /// <summary>
    /// Block name (e.g., "network_interface", "disk")
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Block attributes
    /// </summary>
    public ImmutableDictionary<string, TerraformValue> Attributes { get; init; }

    /// <summary>
    /// Nested blocks (recursive)
    /// </summary>
    public ImmutableList<NestedBlockData> NestedBlocks { get; init; }

    /// <summary>
    /// Initializes a new NestedBlockData with validation.
    /// </summary>
    /// <param name="name">Block name</param>
    /// <param name="attributes">Block attributes</param>
    /// <param name="nestedBlocks">Nested blocks</param>
    public NestedBlockData(
        string name,
        ImmutableDictionary<string, TerraformValue> attributes,
        ImmutableList<NestedBlockData> nestedBlocks)
    {
        TerraformIdentifierValidator.ValidateIdentifier(name, nameof(name));
        ArgumentNullException.ThrowIfNull(attributes, nameof(attributes));
        ArgumentNullException.ThrowIfNull(nestedBlocks, nameof(nestedBlocks));

        // Validate all attribute keys
        foreach (var key in attributes.Keys)
        {
            TerraformIdentifierValidator.ValidateAttributeKey(key, nameof(attributes));
        }

        Name = name;
        Attributes = attributes;
        NestedBlocks = nestedBlocks;
    }
}
