using HclSharp.Core.Validation;

namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform resource reference (e.g., aws_instance.web.id).
/// </summary>
public record ResourceReference : TerraformValue
{
    /// <summary>
    /// The type of the resource (e.g., "aws_instance")
    /// </summary>
    public string ResourceType { get; init; }

    /// <summary>
    /// The name of the resource instance (e.g., "web")
    /// </summary>
    public string ResourceName { get; init; }

    /// <summary>
    /// The attribute path (e.g., "id" or "network.private_ip")
    /// </summary>
    public string AttributePath { get; init; }

    /// <summary>
    /// Initializes a new ResourceReference with validation.
    /// </summary>
    /// <param name="resourceType">The type of the resource</param>
    /// <param name="resourceName">The name of the resource instance</param>
    /// <param name="attributePath">The attribute path</param>
    public ResourceReference(string resourceType, string resourceName, string attributePath)
    {
        TerraformIdentifierValidator.ValidateIdentifier(resourceType, nameof(resourceType));
        TerraformIdentifierValidator.ValidateIdentifier(resourceName, nameof(resourceName));
        TerraformIdentifierValidator.ValidateAttributeKey(attributePath, nameof(attributePath));

        ResourceType = resourceType;
        ResourceName = resourceName;
        AttributePath = attributePath;
    }
}
