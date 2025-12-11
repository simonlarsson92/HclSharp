using System.Collections.Immutable;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class ResourceBlockTests
{
    [Fact]
    public void GenerateResourceBlock_WithAttributes_ShouldFormatCorrectly()
    {
        // Arrange
        var attributes = ImmutableDictionary<string, TerraformValue>.Empty
            .Add("name", new VariableReference("vm_name"))
            .Add("num_cpus", new LiteralValue(8))
            .Add("memory", new Expression("64 * 1024"));
        
        var resource = new ResourceBlockData(
            "vsphere_virtual_machine",
            "windows_vm",
            attributes,
            []);
        
        // Act
        var result = HclGenerator.GenerateResourceBlock(resource);
        
        // Assert
        Assert.Contains(@"resource ""vsphere_virtual_machine"" ""windows_vm"" {", result);
        Assert.Contains("name = var.vm_name", result);
        Assert.Contains("num_cpus = 8", result);
        Assert.Contains("memory = 64 * 1024", result);
        Assert.EndsWith("}", result);
    }
    
    [Fact]
    public void GenerateResourceBlock_WithNoAttributes_ShouldReturnEmptyBlock()
    {
        // Arrange
        var resource = new ResourceBlockData(
            "null_resource",
            "example",
            [],
            []);
        
        // Act
        var result = HclGenerator.GenerateResourceBlock(resource);
        
        // Assert
        var expected = @"resource ""null_resource"" ""example"" {
}";
        Assert.Equal(expected, result);
    }
}
