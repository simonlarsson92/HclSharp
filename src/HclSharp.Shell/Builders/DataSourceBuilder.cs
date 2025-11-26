using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Builder for data source blocks.
/// </summary>
public class DataSourceBuilder
{
    private readonly string _type;
    private readonly string _name;
    private readonly TerraformDocumentBuilder _documentBuilder;
    private readonly Dictionary<string, TerraformValue> _attributes = new();
    private readonly List<NestedBlockData> _nestedBlocks = new();

    internal DataSourceBuilder(string type, string name, TerraformDocumentBuilder documentBuilder)
    {
        _type = type;
        _name = name;
        _documentBuilder = documentBuilder;
    }

    /// <summary>
    /// Adds an attribute to the data source.
    /// </summary>
    public DataSourceBuilder AddAttribute(string key, TerraformValue value)
    {
        _attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a nested block to the data source.
    /// Call EndNestedBlock() on the returned builder to return to this DataSourceBuilder.
    /// </summary>
    public NestedBlockBuilder<DataSourceBuilder> AddNestedBlock(string name)
    {
        return new NestedBlockBuilder<DataSourceBuilder>(
            name,
            this,
            data => _nestedBlocks.Add(data));
    }

    /// <summary>
    /// Completes the data source and returns to the document builder.
    /// </summary>
    public TerraformDocumentBuilder Build()
    {
        var dataSource = new DataSourceBlockData(
            _type,
            _name,
            _attributes.ToImmutableDictionary(),
            _nestedBlocks.ToImmutableList());
        
        _documentBuilder.RegisterDataSource(dataSource);
        return _documentBuilder;
    }
}
