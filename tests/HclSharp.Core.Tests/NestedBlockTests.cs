using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class NestedBlockTests
{
    [Fact]
    public void GenerateNestedBlock_Simple_ShouldFormatWithCorrectIndent()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("label", new LiteralValue("disk0"))
            .Add("size", new LiteralValue(40));
        
        var nested = new NestedBlockData(
            "disk",
            attributes,
            ImmutableList<NestedBlockData>.Empty);
        
        // Act
        var result = HclGenerator.GenerateNestedBlock(nested, 1);
        
        // Assert
        Assert.Contains("  disk {", result);
        Assert.Contains("label = \"disk0\"", result);
        Assert.Contains("size = 40", result);
        Assert.Contains("  }", result);
        Assert.True(result.EndsWith("}\n") || result.EndsWith("}\r\n"));
    }
    
    [Fact]
    public void GenerateNestedBlock_Recursive_ShouldIndentProperly()
    {
        // Arrange - nested block within nested block
        var innerAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("setting", new LiteralValue("value"));
        
        var innerBlock = new NestedBlockData(
            "inner",
            innerAttributes,
            ImmutableList<NestedBlockData>.Empty);
        
        var outerAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new LiteralValue("outer"));
        
        var outerBlock = new NestedBlockData(
            "outer",
            outerAttributes,
            ImmutableList.Create(innerBlock));
        
        // Act
        var result = HclGenerator.GenerateNestedBlock(outerBlock, 1);
        
        // Assert
        Assert.Contains("  outer {", result);
        Assert.Contains("    name = \"outer\"", result);
        Assert.Contains("    inner {", result);
        Assert.Contains("      setting = \"value\"", result);
        Assert.Contains("    }", result);
        Assert.Contains("  }", result);
    }
    
    [Fact]
    public void GenerateNestedBlock_InResource_ShouldIntegrate()
    {
        // Arrange
        var diskAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("label", new LiteralValue("disk0"))
            .Add("size", new LiteralValue(40));
        
        var diskBlock = new NestedBlockData(
            "disk",
            diskAttributes,
            ImmutableList<NestedBlockData>.Empty);
        
        var resourceAttributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new LiteralValue("test"));
        
        var resource = new ResourceBlockData(
            "example",
            "test",
            resourceAttributes,
            ImmutableList.Create(diskBlock));
        
        // Act
        var result = HclGenerator.GenerateResourceBlock(resource);
        
        // Assert
        Assert.Contains(@"resource ""example"" ""test"" {", result);
        Assert.Contains("  name = \"test\"", result);
        Assert.Contains("  disk {", result);
        Assert.Contains("    label = \"disk0\"", result);
        Assert.Contains("    size = 40", result);
        Assert.Contains("  }", result);
        Assert.EndsWith("}", result);
    }
}
