using System.Collections.Immutable;
using HclSharp.Core.Validation;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a data source block.
/// </summary>
public record DataSourceBlockData
{
    /// <summary>
    /// Data source type (e.g., "vsphere_datacenter")
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Data source name (e.g., "dc")
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Data source attributes
    /// </summary>
    public ImmutableDictionary<string, TerraformValue> Attributes { get; init; }

    /// <summary>
    /// Nested blocks within the data source
    /// </summary>
    public ImmutableList<NestedBlockData> NestedBlocks { get; init; }

    /// <summary>
    /// Initializes a new DataSourceBlockData with validation.
    /// </summary>
    /// <param name="type">Data source type</param>
    /// <param name="name">Data source name</param>
    /// <param name="attributes">Data source attributes</param>
    /// <param name="nestedBlocks">Nested blocks</param>
    public DataSourceBlockData(
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
