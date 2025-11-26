namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform data source reference (e.g., data.vsphere_datacenter.dc.id).
/// </summary>
/// <param name="DataSourceType">The type of the data source (e.g., "vsphere_datacenter")</param>
/// <param name="DataSourceName">The name of the data source instance (e.g., "dc")</param>
/// <param name="AttributePath">The attribute path (e.g., "id" or "network.id")</param>
public record DataReference(string DataSourceType, string DataSourceName, string AttributePath) : TerraformValue;
