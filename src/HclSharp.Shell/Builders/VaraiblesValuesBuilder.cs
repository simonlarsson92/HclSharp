using HclSharp.Core;
using HclSharp.Core.Model;
using HclSharp.Core.Values;
using System.Collections.Immutable;

namespace HclSharp.Shell.Builders;

public class VariablesValuesBuilder
{
    private readonly Dictionary<string, TerraformValue> _values;

    internal VariablesValuesBuilder()
    {
        _values = [];
    }

    /// <summary>
    /// Adds a variable value.
    /// </summary>
    public VariablesValuesBuilder AddVariableValue(string name, TerraformValue value)
    {
        _values[name] = value;
        return this;
    }

    public VariablesValuesConfiguration Build()
    {
        return new VariablesValuesConfiguration(_values.ToImmutableDictionary());
    }

    public string ToHcl()
    {
        return HclGenerator.GenerateVariableValuesHcl(Build());
    }
}
