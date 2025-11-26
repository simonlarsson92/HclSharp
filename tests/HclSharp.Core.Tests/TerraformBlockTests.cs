using System.Collections.Immutable;
using HclSharp.Core.Model;

namespace HclSharp.Core.Tests;

public class TerraformBlockTests
{
    [Fact]
    public void GenerateTerraformBlock_WithSingleProvider_ShouldFormatCorrectly()
    {
        // Arrange
        var provider = new RequiredProviderData("vsphere", "hashicorp/vsphere", ">= 2.5.0");
        var block = new TerraformBlockData(ImmutableList.Create(provider));
        
        // Act
        var result = HclGenerator.GenerateTerraformBlock(block);
        
        // Assert
        var expected = @"terraform {
  required_providers {
    vsphere = {
      source  = ""hashicorp/vsphere""
      version = "">= 2.5.0""
    }
  }
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateTerraformBlock_WithMultipleProviders_ShouldFormatCorrectly()
    {
        // Arrange
        var providers = ImmutableList.Create(
            new RequiredProviderData("aws", "hashicorp/aws", "~> 5.0"),
            new RequiredProviderData("random", "hashicorp/random", null)
        );
        var block = new TerraformBlockData(providers);
        
        // Act
        var result = HclGenerator.GenerateTerraformBlock(block);
        
        // Assert
        var expected = @"terraform {
  required_providers {
    aws = {
      source  = ""hashicorp/aws""
      version = ""~> 5.0""
    }
    random = {
      source  = ""hashicorp/random""
    }
  }
}";
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GenerateTerraformBlock_WithNoProviders_ShouldReturnEmptyBlock()
    {
        // Arrange
        var block = new TerraformBlockData(ImmutableList<RequiredProviderData>.Empty);
        
        // Act
        var result = HclGenerator.GenerateTerraformBlock(block);
        
        // Assert
        Assert.Equal("terraform {\n}", result);
    }
    
    [Fact]
    public void GenerateTerraformBlock_WithProviderWithoutVersion_ShouldOmitVersionLine()
    {
        // Arrange
        var provider = new RequiredProviderData("null", "hashicorp/null", null);
        var block = new TerraformBlockData(ImmutableList.Create(provider));
        
        // Act
        var result = HclGenerator.GenerateTerraformBlock(block);
        
        // Assert
        var expected = @"terraform {
  required_providers {
    null = {
      source  = ""hashicorp/null""
    }
  }
}";
        Assert.Equal(expected, result);
    }
}
