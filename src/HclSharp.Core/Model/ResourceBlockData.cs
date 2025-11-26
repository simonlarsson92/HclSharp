using System.Collections.Immutable;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a resource block.
/// </summary>
/// <param name="Type">Resource type (e.g., "vsphere_virtual_machine")</param>
/// <param name="Name">Resource name (e.g., "windows_vm")</param>
/// <param name="Attributes">Resource attributes</param>
/// <param name="NestedBlocks">Nested blocks within the resource</param>
public record ResourceBlockData(
    string Type,
    string Name,
    ImmutableDictionary<string, TerraformValue> Attributes,
    ImmutableList<NestedBlockData> NestedBlocks);
