using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Builder for nested blocks with support for chaining back to parent.
/// </summary>
/// <typeparam name="TParent">Type of parent builder</typeparam>
public class NestedBlockBuilder<TParent> where TParent : class
{
    private readonly string _name;
    private readonly TParent _parent;
    private readonly Action<NestedBlockData> _registerWithParent;
    private readonly Dictionary<string, TerraformValue> _attributes = new();
    private readonly List<NestedBlockData> _nestedBlocks = new();

    internal NestedBlockBuilder(string name, TParent parent, Action<NestedBlockData> registerWithParent)
    {
        _name = name;
        _parent = parent;
        _registerWithParent = registerWithParent;
    }

    /// <summary>
    /// Adds an attribute to the nested block.
    /// </summary>
    public NestedBlockBuilder<TParent> AddAttribute(string key, TerraformValue value)
    {
        _attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Starts building a nested block within this block.
    /// Call EndNestedBlock() on the returned builder to return to this builder.
    /// </summary>
    public NestedBlockBuilder<NestedBlockBuilder<TParent>> AddNestedBlock(string name)
    {
        return new NestedBlockBuilder<NestedBlockBuilder<TParent>>(
            name,
            this,
            data => _nestedBlocks.Add(data));
    }

    /// <summary>
    /// Completes this nested block and returns to the parent builder.
    /// </summary>
    public TParent EndNestedBlock()
    {
        _registerWithParent(BuildData());
        return _parent;
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
