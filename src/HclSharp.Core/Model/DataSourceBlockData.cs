using System.Collections.Immutable;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a data source block.
/// </summary>
/// <param name="Type">Data source type (e.g., "vsphere_datacenter")</param>
/// <param name="Name">Data source name (e.g., "dc")</param>
/// <param name="Attributes">Data source attributes</param>
/// <param name="NestedBlocks">Nested blocks within the data source</param>
public record DataSourceBlockData(
    string Type,
    string Name,
    ImmutableDictionary<string, TerraformValue> Attributes,
    ImmutableList<NestedBlockData> NestedBlocks);
