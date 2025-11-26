using System.Collections.Immutable;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a nested block within a resource or data source (e.g., network_interface, disk, clone).
/// </summary>
/// <param name="Name">Block name (e.g., "network_interface", "disk")</param>
/// <param name="Attributes">Block attributes</param>
/// <param name="NestedBlocks">Nested blocks (recursive)</param>
public record NestedBlockData(
    string Name,
    ImmutableDictionary<string, TerraformValue> Attributes,
    ImmutableList<NestedBlockData> NestedBlocks);
