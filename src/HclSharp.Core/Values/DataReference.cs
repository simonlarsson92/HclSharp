using HclSharp.Core.Validation;

namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform data source reference (e.g., data.vsphere_datacenter.dc.id).
/// </summary>
public record DataReference : TerraformValue
{
    /// <summary>
    /// The type of the data source (e.g., "vsphere_datacenter")
    /// </summary>
    public string DataSourceType { get; init; }

    /// <summary>
    /// The name of the data source instance (e.g., "dc")
    /// </summary>
    public string DataSourceName { get; init; }

    /// <summary>
    /// The attribute path (e.g., "id" or "network.id")
    /// </summary>
    public string AttributePath { get; init; }

    /// <summary>
    /// Initializes a new DataReference with validation.
    /// </summary>
    /// <param name="dataSourceType">The type of the data source</param>
    /// <param name="dataSourceName">The name of the data source instance</param>
    /// <param name="attributePath">The attribute path</param>
    public DataReference(string dataSourceType, string dataSourceName, string attributePath)
    {
        TerraformIdentifierValidator.ValidateIdentifier(dataSourceType, nameof(dataSourceType));
        TerraformIdentifierValidator.ValidateIdentifier(dataSourceName, nameof(dataSourceName));
        TerraformIdentifierValidator.ValidateAttributeKey(attributePath, nameof(attributePath));

        DataSourceType = dataSourceType;
        DataSourceName = dataSourceName;
        AttributePath = attributePath;
    }
}
