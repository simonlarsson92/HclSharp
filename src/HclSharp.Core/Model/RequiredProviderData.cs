using HclSharp.Core.Validation;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents a required provider in the terraform block.
/// </summary>
public record RequiredProviderData
{
    /// <summary>
    /// Provider name (e.g., "vsphere")
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Provider source (e.g., "hashicorp/vsphere")
    /// </summary>
    public string Source { get; init; }

    /// <summary>
    /// Version constraint (e.g., ">= 2.5.0")
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// Initializes a new RequiredProviderData with validation.
    /// </summary>
    /// <param name="name">Provider name</param>
    /// <param name="source">Provider source</param>
    /// <param name="version">Version constraint (optional)</param>
    public RequiredProviderData(string name, string source, string? version)
    {
        TerraformIdentifierValidator.ValidateIdentifier(name, nameof(name));
        TerraformIdentifierValidator.ValidateProviderSource(source, nameof(source));

        Name = name;
        Source = source;
        Version = version;
    }
}
