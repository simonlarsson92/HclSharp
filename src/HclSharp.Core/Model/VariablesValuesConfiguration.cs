using HclSharp.Core.Values;
using System.Collections.Immutable;

namespace HclSharp.Core.Model;

public record VariablesValuesConfiguration(
    ImmutableDictionary<string, TerraformValue> Values)
{
    public static readonly VariablesValuesConfiguration Empty = new(
        []);
}
