using System.Collections.Immutable;
using HclSharp.Core.Values;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a provider configuration block.
/// </summary>
/// <param name="Name">Provider name (e.g., "vsphere")</param>
/// <param name="Attributes">Provider configuration attributes</param>
public record ProviderBlockData(string Name, ImmutableDictionary<string, TerraformValue> Attributes);
