namespace HclSharp.Core.Model;

/// <summary>
/// Represents a required provider in the terraform block.
/// </summary>
/// <param name="Name">Provider name (e.g., "vsphere")</param>
/// <param name="Source">Provider source (e.g., "hashicorp/vsphere")</param>
/// <param name="Version">Version constraint (e.g., ">= 2.5.0")</param>
public record RequiredProviderData(string Name, string Source, string? Version);
