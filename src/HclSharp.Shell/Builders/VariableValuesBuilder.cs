using System.Collections.Generic;
using System.Collections.Immutable;
using HclSharp.Core;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Shell.Builders;

/// <summary>
/// Mutable builder for constructing variable assignments (like .tfvars files).
/// </summary>
public class VariableValuesBuilder
{
    private readonly Dictionary<string, TerraformValue> _values;

    /// <summary>
    /// Initializes a new instance of the VariableValuesBuilder.
    /// </summary>
    public VariableValuesBuilder()
    {
        _values = new Dictionary<string, TerraformValue>();
    }

    /// <summary>
    /// Adds or updates a variable value.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    /// <returns>This builder for method chaining.</returns>
    public VariableValuesBuilder AddVariable(string name, TerraformValue value)
    {
        _values[name] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates a variable value. (Alternative method name for compatibility)
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    /// <returns>This builder for method chaining.</returns>
    public VariableValuesBuilder AddVariableValue(string name, TerraformValue value)
    {
        return AddVariable(name, value);
    }

    /// <summary>
    /// Builds the immutable VariableValuesConfiguration.
    /// </summary>
    /// <returns>An immutable configuration containing all variable values.</returns>
    public VariablesValuesConfiguration Build()
    {
        return new VariablesValuesConfiguration(_values.ToImmutableDictionary());
    }

    /// <summary>
    /// Convenience method: builds and generates HCL for variable values.
    /// </summary>
    /// <returns>HCL string representation of the variable values.</returns>
    public string ToHcl()
    {
        return HclGenerator.GenerateVariableValuesHcl(Build());
    }
}
