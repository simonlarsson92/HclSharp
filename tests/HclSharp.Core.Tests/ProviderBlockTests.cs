using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class ProviderBlockTests
{
    [Fact]
    public void GenerateProviderBlock_WithSingleAttribute_ShouldFormatCorrectly()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("region", new LiteralValue("us-east-1"));
        
        var provider = new ProviderBlockData("aws", attributes);
        
        // Act
        var result = HclGenerator.GenerateProviderBlock(provider);
        
        // Assert
        var expected = @"provider ""aws"" {
  region = ""us-east-1""
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateProviderBlock_WithNoAttributes_ShouldReturnEmptyBlock()
    {
        // Arrange
        var provider = new ProviderBlockData("aws", []);
        
        // Act
        var result = HclGenerator.GenerateProviderBlock(provider);
        
        // Assert
        var expected = @"provider ""aws"" {
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateProviderBlock_WithVariableReference_ShouldNotQuote()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("api_key", new VariableReference("my_api_key"));
        
        var provider = new ProviderBlockData("custom", attributes);
        
        // Act
        var result = HclGenerator.GenerateProviderBlock(provider);
        
        // Assert
        Assert.Contains("provider \"custom\" {", result);
        Assert.Contains("  api_key = var.my_api_key", result);
        Assert.EndsWith("}", result);
    }
    
    [Fact]
    public void GenerateProviderBlock_WithMultipleAttributeTypes_ShouldFormatEachCorrectly()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("string_attr", new LiteralValue("test"))
            .Add("number_attr", new LiteralValue(42))
            .Add("bool_attr", new LiteralValue(true));
        
        var provider = new ProviderBlockData("test", attributes);
        
        // Act
        var result = HclGenerator.GenerateProviderBlock(provider);
        
        // Assert - Check each attribute independently (order doesn't matter)
        Assert.Contains("provider \"test\" {", result);
        Assert.Contains("string_attr = \"test\"", result);
        Assert.Contains("number_attr = 42", result);
        Assert.Contains("bool_attr = true", result);
    }
}
