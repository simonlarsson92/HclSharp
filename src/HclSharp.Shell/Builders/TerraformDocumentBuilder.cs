using System.Collections.Immutable;
using HclSharp.Core;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Mutable builder for constructing Terraform configurations.
/// </summary>
public class TerraformDocumentBuilder
{
    private readonly List<RequiredProviderData> _requiredProviders = new();
    private readonly List<ProviderBlockData> _providers = new();
    private readonly List<DataSourceBlockData> _dataSources = new();
    private readonly List<ResourceBlockData> _resources = new();

    /// <summary>
    /// Adds a required provider to the terraform block.
    /// </summary>
    public TerraformDocumentBuilder AddRequiredProvider(string name, string source, string? version = null)
    {
        _requiredProviders.Add(new RequiredProviderData(name, source, version));
        return this;
    }

    /// <summary>
    /// Adds a provider block and returns a builder for configuring it.
    /// </summary>
    public ProviderBuilder AddProvider(string name)
    {
        return new ProviderBuilder(name, this);
    }

    /// <summary>
    /// Adds a data source block and returns a builder for configuring it.
    /// </summary>
    public DataSourceBuilder AddDataSource(string type, string name)
    {
        return new DataSourceBuilder(type, name, this);
    }

    /// <summary>
    /// Adds a resource block and returns a builder for configuring it.
    /// </summary>
    public ResourceBuilder AddResource(string type, string name)
    {
        return new ResourceBuilder(type, name, this);
    }

    /// <summary>
    /// Builds the immutable TerraformConfiguration.
    /// </summary>
    public TerraformConfiguration Build()
    {
        var terraformBlock = _requiredProviders.Count > 0
            ? new TerraformBlockData(_requiredProviders.ToImmutableList())
            : null;

        return new TerraformConfiguration(
            terraformBlock,
            _providers.ToImmutableList(),
            _dataSources.ToImmutableList(),
            _resources.ToImmutableList());
    }

    /// <summary>
    /// Convenience method: builds and generates HCL.
    /// </summary>
    public string ToHcl()
    {
        return HclGenerator.GenerateHcl(Build());
    }

    // Internal methods for builders to register blocks
    internal void RegisterProvider(ProviderBlockData provider) => _providers.Add(provider);
    internal void RegisterDataSource(DataSourceBlockData dataSource) => _dataSources.Add(dataSource);
    internal void RegisterResource(ResourceBlockData resource) => _resources.Add(resource);
}
