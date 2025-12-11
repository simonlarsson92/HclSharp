using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class DataSourceBlockTests
{
    [Fact]
    public void GenerateDataSourceBlock_WithSingleAttribute_ShouldFormatCorrectly()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new VariableReference("datacenter"));
        
        var dataSource = new DataSourceBlockData(
            "vsphere_datacenter",
            "dc",
            attributes,
            []);
        
        // Act
        var result = HclGenerator.GenerateDataSourceBlock(dataSource);
        
        // Assert
        var expected = @"data ""vsphere_datacenter"" ""dc"" {
  name = var.datacenter
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateDataSourceBlock_WithNoAttributes_ShouldReturnEmptyBlock()
    {
        // Arrange
        var dataSource = new DataSourceBlockData(
            "aws_ami",
            "ubuntu",
            [],
            []);
        
        // Act
        var result = HclGenerator.GenerateDataSourceBlock(dataSource);
        
        // Assert
        var expected = @"data ""aws_ami"" ""ubuntu"" {
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateDataSourceBlock_WithMultipleAttributes_ShouldFormatEach()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("most_recent", new LiteralValue(true))
            .Add("owner", new LiteralValue("099720109477"));
        
        var dataSource = new DataSourceBlockData(
            "aws_ami",
            "ubuntu",
            attributes,
            []);
        
        // Act
        var result = HclGenerator.GenerateDataSourceBlock(dataSource);
        
        // Assert - Check each attribute independently (order doesn't matter)
        Assert.Contains(@"data ""aws_ami"" ""ubuntu"" {", result);
        Assert.Contains("most_recent = true", result);
        Assert.Contains(@"owner = ""099720109477""", result);
        Assert.EndsWith("}", result);
    }
}
