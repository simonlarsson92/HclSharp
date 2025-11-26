using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Builder for resource blocks.
/// </summary>
public class ResourceBuilder
{
    private readonly string _type;
    private readonly string _name;
    private readonly TerraformDocumentBuilder _documentBuilder;
    private readonly Dictionary<string, TerraformValue> _attributes = new();
    private readonly List<NestedBlockData> _nestedBlocks = new();

    internal ResourceBuilder(string type, string name, TerraformDocumentBuilder documentBuilder)
    {
        _type = type;
        _name = name;
        _documentBuilder = documentBuilder;
    }

    /// <summary>
    /// Adds an attribute to the resource.
    /// </summary>
    public ResourceBuilder AddAttribute(string key, TerraformValue value)
    {
        _attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a nested block to the resource.
    /// </summary>
    public NestedBlockBuilder AddNestedBlock(string name)
    {
        var builder = new NestedBlockBuilder(name);
        _nestedBlocks.Add(builder.BuildData());
        return builder;
    }

    /// <summary>
    /// Completes the resource and returns to the document builder.
    /// </summary>
    public TerraformDocumentBuilder Build()
    {
        var resource = new ResourceBlockData(
            _type,
            _name,
            _attributes.ToImmutableDictionary(),
            _nestedBlocks.ToImmutableList());
        
        _documentBuilder.RegisterResource(resource);
        return _documentBuilder;
    }
}
