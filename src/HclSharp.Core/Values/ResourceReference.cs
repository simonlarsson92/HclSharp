namespace HclSharp.Core.Values;

/// <summary>
/// Represents a Terraform resource reference (e.g., aws_instance.web.id).
/// </summary>
/// <param name="ResourceType">The type of the resource (e.g., "aws_instance")</param>
/// <param name="ResourceName">The name of the resource instance (e.g., "web")</param>
/// <param name="AttributePath">The attribute path (e.g., "id" or "network.private_ip")</param>
public record ResourceReference(string ResourceType, string ResourceName, string AttributePath) : TerraformValue;
