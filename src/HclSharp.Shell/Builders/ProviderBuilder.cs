using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Builder for provider blocks.
/// </summary>
public class ProviderBuilder
{
    private readonly string _name;
    private readonly TerraformDocumentBuilder _documentBuilder;
    private readonly Dictionary<string, TerraformValue> _attributes = new();

    internal ProviderBuilder(string name, TerraformDocumentBuilder documentBuilder)
    {
        _name = name;
        _documentBuilder = documentBuilder;
    }

    /// <summary>
    /// Adds an attribute to the provider.
    /// </summary>
    public ProviderBuilder AddAttribute(string key, TerraformValue value)
    {
        _attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Completes the provider configuration and returns to the document builder.
    /// </summary>
    public TerraformDocumentBuilder Build()
    {
        var providerData = new ProviderBlockData(_name, _attributes.ToImmutableDictionary());
        _documentBuilder.RegisterProvider(providerData);
        return _documentBuilder;
    }
}
