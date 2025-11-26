using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class CompleteDocumentTests
{
    [Fact]
    public void GenerateHcl_CompleteDocument_ShouldFormatCorrectly()
    {
        // Arrange - create a complete configuration with all block types
        var requiredProvider = new RequiredProviderData(
            "test",
            "hashicorp/test",
            ">= 1.0.0");
        
        var terraformBlock = new TerraformBlockData(
            ImmutableList.Create(requiredProvider));
        
        var providerAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("region", new LiteralValue("us-west-2"));
        
        var provider = new ProviderBlockData(
            "test",
            providerAttributes);
        
        var dataAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new LiteralValue("test-data"));
        
        var dataSource = new DataSourceBlockData(
            "test_data",
            "example",
            dataAttributes,
            ImmutableList<NestedBlockData>.Empty);
        
        var resourceAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new LiteralValue("test-resource"));
        
        var resource = new ResourceBlockData(
            "test_resource",
            "example",
            resourceAttributes,
            ImmutableList<NestedBlockData>.Empty);
        
        var config = new TerraformConfiguration(
            terraformBlock,
            ImmutableList.Create(provider),
            ImmutableList.Create(dataSource),
            ImmutableList.Create(resource));
        
        // Act
        var result = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains("terraform {", result);
        Assert.Contains("required_providers {", result);
        Assert.Contains("test = {", result);
        Assert.Contains("source  = \"hashicorp/test\"", result);
        Assert.Contains("version = \">= 1.0.0\"", result);
        
        Assert.Contains("provider \"test\" {", result);
        Assert.Contains("region = \"us-west-2\"", result);
        
        Assert.Contains("data \"test_data\" \"example\" {", result);
        Assert.Contains("name = \"test-data\"", result);
        
        Assert.Contains("resource \"test_resource\" \"example\" {", result);
        Assert.Contains("name = \"test-resource\"", result);
        
        // Verify blocks are in correct order
        var terraformIndex = result.IndexOf("terraform {");
        var providerIndex = result.IndexOf("provider \"test\" {");
        var dataIndex = result.IndexOf("data \"test_data\"");
        var resourceIndex = result.IndexOf("resource \"test_resource\"");
        
        Assert.True(terraformIndex < providerIndex, "terraform block should come before provider");
        Assert.True(providerIndex < dataIndex, "provider should come before data source");
        Assert.True(dataIndex < resourceIndex, "data source should come before resource");
    }
    
    [Fact]
    public void GenerateHcl_NoTerraformBlock_ShouldNotIncludeTerraformBlock()
    {
        // Arrange
        var config = new TerraformConfiguration(
            null,
            ImmutableList<ProviderBlockData>.Empty,
            ImmutableList<DataSourceBlockData>.Empty,
            ImmutableList<ResourceBlockData>.Empty);
        
        // Act
        var result = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.DoesNotContain("terraform {", result);
        Assert.Equal("", result);
    }
    
    [Fact]
    public void GenerateHcl_WithNestedBlocks_ShouldIncludeAllContent()
    {
        // Arrange
        var diskAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("size", new LiteralValue(40));
        
        var diskBlock = new NestedBlockData(
            "disk",
            diskAttributes,
            ImmutableList<NestedBlockData>.Empty);
        
        var resourceAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new LiteralValue("vm"));
        
        var resource = new ResourceBlockData(
            "vsphere_virtual_machine",
            "test",
            resourceAttributes,
            ImmutableList.Create(diskBlock));
        
        var config = new TerraformConfiguration(
            null,
            ImmutableList<ProviderBlockData>.Empty,
            ImmutableList<DataSourceBlockData>.Empty,
            ImmutableList.Create(resource));
        
        // Act
        var result = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains("resource \"vsphere_virtual_machine\" \"test\" {", result);
        Assert.Contains("disk {", result);
        Assert.Contains("size = 40", result);
    }
}
