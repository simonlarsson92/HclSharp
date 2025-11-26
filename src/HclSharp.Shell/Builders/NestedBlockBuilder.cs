using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Builder for nested blocks (can be recursive).
/// </summary>
public class NestedBlockBuilder
{
    private readonly string _name;
    private readonly Dictionary<string, TerraformValue> _attributes = new();
    private readonly List<NestedBlockData> _nestedBlocks = new();

    internal NestedBlockBuilder(string name)
    {
        _name = name;
    }

    /// <summary>
    /// Adds an attribute to the nested block.
    /// </summary>
    public NestedBlockBuilder AddAttribute(string key, TerraformValue value)
    {
        _attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a nested block within this block (recursive).
    /// </summary>
    public NestedBlockBuilder AddNestedBlock(string name)
    {
        var childBuilder = new NestedBlockBuilder(name);
        _nestedBlocks.Add(childBuilder.BuildData());
        return childBuilder;
    }

    /// <summary>
    /// Builds the immutable NestedBlockData.
    /// </summary>
    internal NestedBlockData BuildData()
    {
        return new NestedBlockData(
            _name,
            _attributes.ToImmutableDictionary(),
            _nestedBlocks.ToImmutableList());
    }
}
