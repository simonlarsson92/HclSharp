using System.Collections.Immutable;

namespace HclSharp.Core.Model;

/// <summary>
/// Represents the terraform configuration block.
/// </summary>
/// <param name="RequiredProviders">List of required providers</param>
public record TerraformBlockData(ImmutableList<RequiredProviderData> RequiredProviders)
{
    public static readonly TerraformBlockData Empty = new(ImmutableList<RequiredProviderData>.Empty);
}
