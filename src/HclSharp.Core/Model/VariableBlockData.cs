using HclSharp.Core.Values;
using System.Collections.Immutable;

namespace HclSharp.Core.Model;

/// <summary>
/// Variable block data representation.
/// </summary>
/// <param name="Name">The name of the variable.</param>
/// <param name="Type">The type of the variable.</param>
/// <param name="Default">The default value of the variable.</param>
/// <param name="Description">The description of the variable.</param>
/// <param name="Sensitive">Indicates if the variable is sensitive.</param>
/// <param name="Validations">List of validations for the variable.</param>
/// <param name="Nullable">Indicates if the variable can be null.</param>
public record VariableBlockData
{
    public string Name { get; init; }
    
    public string Type { get; init; }

    public TerraformValue? Default { get; init; }

    public string? Description { get; init; }

    public bool Sensitive { get; init; } = false;

    public ImmutableList<VariableValidationData>? Validations { get; init; }

    public bool Nullable { get; init; } = true;
}
