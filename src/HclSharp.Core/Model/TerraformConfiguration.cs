using System.Collections.Immutable;

namespace HclSharp.Core.Model;

/// <summary>
/// Root immutable configuration representing a complete Terraform document.
/// </summary>
/// <param name="TerraformBlock">The terraform block (nullable if not present)</param>
/// <param name="Providers">List of provider blocks</param>
/// <param name="DataSources">List of data source blocks</param>
/// <param name="Resources">List of resource blocks</param>
public record TerraformConfiguration(
    TerraformBlockData? TerraformBlock,
    ImmutableList<ProviderBlockData> Providers,
    ImmutableList<DataSourceBlockData> DataSources,
    ImmutableList<ResourceBlockData> Resources)
{
    public static readonly TerraformConfiguration Empty = new(
        null,
        ImmutableList<ProviderBlockData>.Empty,
        ImmutableList<DataSourceBlockData>.Empty,
        ImmutableList<ResourceBlockData>.Empty);
}
