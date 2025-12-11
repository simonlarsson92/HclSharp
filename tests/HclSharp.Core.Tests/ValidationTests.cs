using HclSharp.Core.Model;
using HclSharp.Core.Values;
using System.Collections.Immutable;

namespace HclSharp.Core.Tests;

/// <summary>
/// Tests for input validation across all user-controlled strings.
/// </summary>
public class ValidationTests
{
    [Theory]
    [InlineData("valid_name")]
    [InlineData("ValidName")]
    [InlineData("valid-name")]
    [InlineData("_private")]
    [InlineData("name123")]
    [InlineData("my_resource_name")]
    public void ProviderBlockData_WithValidName_ShouldSucceed(string name)
    {
        // Arrange & Act
        var provider = new ProviderBlockData(name, []);
        
        // Assert
        Assert.Equal(name, provider.Name);
    }

    [Theory]
    [InlineData("invalid name")] // space
    [InlineData("invalid\nname")] // newline
    [InlineData("invalid{name")] // brace
    [InlineData("invalid}name")] // brace
    [InlineData("invalid\"name")] // quote
    [InlineData("invalid'name")] // quote
    [InlineData("123invalid")] // starts with digit
    [InlineData("-invalid")] // starts with hyphen
    public void ProviderBlockData_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new ProviderBlockData(invalidName, []));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Fact]
    public void ProviderBlockData_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new ProviderBlockData(null!, []));
    }

    [Theory]
    [InlineData("valid_key")]
    [InlineData("nested.key")]
    [InlineData("deeply.nested.key")]
    public void ProviderBlockData_WithValidAttributeKeys_ShouldSucceed(string key)
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add(key, new LiteralValue("value"));

        // Act
        var provider = new ProviderBlockData("test", attributes);
        
        // Assert
        Assert.True(provider.Attributes.ContainsKey(key));
    }

    [Theory]
    [InlineData("invalid\nkey")] // newline
    [InlineData("invalid{key")] // brace
    [InlineData("invalid}key")] // brace
    [InlineData("invalid..key")] // empty segment
    public void ProviderBlockData_WithInvalidAttributeKeys_ShouldThrowArgumentException(string invalidKey)
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add(invalidKey, new LiteralValue("value"));

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new ProviderBlockData("test", attributes));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Theory]
    [InlineData("hashicorp/aws")]
    [InlineData("terraform-providers/azurerm")]
    [InlineData("registry.terraform.io/hashicorp/google")]
    public void RequiredProviderData_WithValidSource_ShouldSucceed(string source)
    {
        // Arrange & Act
        var provider = new RequiredProviderData("test", source, null);
        
        // Assert
        Assert.Equal(source, provider.Source);
    }

    [Theory]
    [InlineData("invalid")] // no slash
    [InlineData("invalid\nsource")] // newline
    [InlineData("invalid{source")] // brace
    public void RequiredProviderData_WithInvalidSource_ShouldThrowArgumentException(string invalidSource)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new RequiredProviderData("test", invalidSource, null));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Theory]
    [InlineData("windows_vm")]
    [InlineData("test-resource")]
    [InlineData("_private_resource")]
    public void ResourceBlockData_WithValidNames_ShouldSucceed(string name)
    {
        // Arrange & Act
        var resource = new ResourceBlockData(
            "vsphere_virtual_machine",
            name,
            [],
            []);
        
        // Assert
        Assert.Equal(name, resource.Name);
    }

    [Theory]
    [InlineData("invalid\nname")]
    [InlineData("invalid name")]
    [InlineData("invalid}name")]
    public void ResourceBlockData_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new ResourceBlockData(
                "valid_type",
                invalidName,
                [],
                []));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Fact]
    public void DataSourceBlockData_WithValidParameters_ShouldSucceed()
    {
        // Arrange & Act
        var dataSource = new DataSourceBlockData(
            "vsphere_datacenter",
            "dc",
            [],
            []);
        
        // Assert
        Assert.Equal("vsphere_datacenter", dataSource.Type);
        Assert.Equal("dc", dataSource.Name);
    }

    [Theory]
    [InlineData("disk")]
    [InlineData("network_interface")]
    [InlineData("nested-block")]
    public void NestedBlockData_WithValidName_ShouldSucceed(string name)
    {
        // Arrange & Act
        var nested = new NestedBlockData(
            name,
            [],
            []);
        
        // Assert
        Assert.Equal(name, nested.Name);
    }

    [Theory]
    [InlineData("invalid\nblock")]
    [InlineData("invalid}block")]
    public void NestedBlockData_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new NestedBlockData(
                invalidName,
                [],
                []));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Theory]
    [InlineData("vsphere_user")]
    [InlineData("vm_name")]
    [InlineData("_private_var")]
    public void VariableReference_WithValidName_ShouldSucceed(string variableName)
    {
        // Arrange & Act
        var varRef = new VariableReference(variableName);
        
        // Assert
        Assert.Equal(variableName, varRef.VariableName);
    }

    [Theory]
    [InlineData("invalid\nvar")]
    [InlineData("invalid var")]
    [InlineData("invalid}var")]
    public void VariableReference_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new VariableReference(invalidName));
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    [Fact]
    public void DataReference_WithValidParameters_ShouldSucceed()
    {
        // Arrange & Act
        var dataRef = new DataReference("vsphere_datacenter", "dc", "id");
        
        // Assert
        Assert.Equal("vsphere_datacenter", dataRef.DataSourceType);
        Assert.Equal("dc", dataRef.DataSourceName);
        Assert.Equal("id", dataRef.AttributePath);
    }

    [Fact]
    public void DataReference_WithNestedAttributePath_ShouldSucceed()
    {
        // Arrange & Act
        var dataRef = new DataReference("vsphere_network", "net", "network.private_ip");
        
        // Assert
        Assert.Equal("network.private_ip", dataRef.AttributePath);
    }

    [Theory]
    [InlineData("invalid\ntype")]
    [InlineData("invalid}name")]
    [InlineData("invalid\npath")]
    public void DataReference_WithInvalidParameters_ShouldThrowArgumentException(string invalidValue)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new DataReference(invalidValue, "valid", "valid"));
        Assert.Throws<ArgumentException>(() => 
            new DataReference("valid", invalidValue, "valid"));
        Assert.Throws<ArgumentException>(() => 
            new DataReference("valid", "valid", invalidValue));
    }

    [Fact]
    public void ResourceReference_WithValidParameters_ShouldSucceed()
    {
        // Arrange & Act
        var resRef = new ResourceReference("aws_instance", "web", "id");
        
        // Assert
        Assert.Equal("aws_instance", resRef.ResourceType);
        Assert.Equal("web", resRef.ResourceName);
        Assert.Equal("id", resRef.AttributePath);
    }

    [Fact]
    public void LiteralValue_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LiteralValue(null!));
    }

    [Theory]
    [InlineData("string value")]
    [InlineData(123)]
    [InlineData(true)]
    [InlineData(45.67)]
    public void LiteralValue_WithValidValue_ShouldSucceed(object value)
    {
        // Arrange & Act
        var literal = new LiteralValue(value);
        
        // Assert
        Assert.Equal(value, literal.Value);
    }

    [Fact]
    public void CompleteConfiguration_WithInjectionAttempts_ShouldThrowArgumentException()
    {
        // This test ensures injection attempts are blocked at multiple levels
        
        // Attempt 1: Malicious provider name
        Assert.Throws<ArgumentException>(() => 
            new ProviderBlockData(
                "aws\n}\nresource \"backdoor\" \"hack\" {",
                []));

        // Attempt 2: Malicious resource name
        Assert.Throws<ArgumentException>(() => 
            new ResourceBlockData(
                "aws_instance",
                "web}\ndata \"exploit\" \"test\" {",
                [],
                []));

        // Attempt 3: Malicious attribute key
        var maliciousKey = "key}\nmalicious = true\nother_key";
        Assert.Throws<ArgumentException>(() => 
            new ProviderBlockData(
                "test",
                ImmutableDictionary<string, TerraformValue>.Empty.Add(maliciousKey, new LiteralValue("value"))));
    }
}
