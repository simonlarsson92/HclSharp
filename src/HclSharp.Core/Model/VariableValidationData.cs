using System.ComponentModel.DataAnnotations;

namespace HclSharp.Core.Model;

/// <summary>
/// Variable validation data.
/// </summary>
/// <param name="Condition">The validation condition expression.</param>
/// <param name="ErrorMessage">The error message to display if validation fails.</param>"
public record VariableValidationData
{
    public required string Condition { get; init; }

    public required string ErrorMessage { get; init; }
}
