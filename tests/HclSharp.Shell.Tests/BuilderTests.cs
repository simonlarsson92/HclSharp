using HclSharp.Core;
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;

namespace HclSharp.Shell.Tests;

public class BuilderTests
{
    [Fact]
    public void ResourceBuilder_WithNestedBlock_ShouldBuildCorrectly()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddResource("vsphere_virtual_machine", "test")
                .AddAttribute("name", new LiteralValue("vm"))
                .AddAttribute("memory", new LiteralValue(8192))
                .AddNestedBlock("disk")
                    .AddAttribute("label", new LiteralValue("disk0"))
                    .AddAttribute("size", new LiteralValue(40))
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains(@"resource ""vsphere_virtual_machine"" ""test"" {", hcl);
        Assert.Contains("name = \"vm\"", hcl);
        Assert.Contains("memory = 8192", hcl);
        Assert.Contains("disk {", hcl);
        Assert.Contains("label = \"disk0\"", hcl);
        Assert.Contains("size = 40", hcl);
    }
    
    [Fact]
    public void ResourceBuilder_WithMultipleNestedBlocks_ShouldBuildCorrectly()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddResource("vsphere_virtual_machine", "test")
                .AddAttribute("name", new LiteralValue("vm"))
                .AddNestedBlock("disk")
                    .AddAttribute("label", new LiteralValue("disk0"))
                    .AddAttribute("size", new LiteralValue(40))
                    .EndNestedBlock()
                .AddNestedBlock("disk")
                    .AddAttribute("label", new LiteralValue("disk1"))
                    .AddAttribute("size", new LiteralValue(100))
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains("label = \"disk0\"", hcl);
        Assert.Contains("size = 40", hcl);
        Assert.Contains("label = \"disk1\"", hcl);
        Assert.Contains("size = 100", hcl);
    }
    
    [Fact]
    public void ResourceBuilder_WithRecursiveNestedBlocks_ShouldBuildCorrectly()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddResource("test_resource", "example")
                .AddNestedBlock("outer")
                    .AddAttribute("outer_attr", new LiteralValue("outer_value"))
                    .AddNestedBlock("inner")
                        .AddAttribute("inner_attr", new LiteralValue("inner_value"))
                        .EndNestedBlock()
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains("outer {", hcl);
        Assert.Contains("outer_attr = \"outer_value\"", hcl);
        Assert.Contains("inner {", hcl);
        Assert.Contains("inner_attr = \"inner_value\"", hcl);
    }
    
    [Fact]
    public void DataSourceBuilder_WithNestedBlock_ShouldBuildCorrectly()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddDataSource("vsphere_datacenter", "dc")
                .AddAttribute("name", new LiteralValue("DC1"))
                .AddNestedBlock("filter")
                    .AddAttribute("key", new LiteralValue("region"))
                    .AddAttribute("value", new LiteralValue("us-west"))
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains(@"data ""vsphere_datacenter"" ""dc"" {", hcl);
        Assert.Contains("name = \"DC1\"", hcl);
        Assert.Contains("filter {", hcl);
        Assert.Contains("key = \"region\"", hcl);
        Assert.Contains("value = \"us-west\"", hcl);
    }
    
    [Fact]
    public void TerraformDocumentBuilder_CompleteVsphereExample_ShouldGenerate()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0")
            .AddProvider("vsphere")
                .AddAttribute("user", new VariableReference("vsphere_user"))
                .AddAttribute("password", new VariableReference("vsphere_password"))
                .AddAttribute("vsphere_server", new VariableReference("vsphere_server"))
                .AddAttribute("allow_unverified_ssl", new LiteralValue(true))
                .Build()
            .AddDataSource("vsphere_datacenter", "dc")
                .AddAttribute("name", new VariableReference("datacenter"))
                .Build()
            .AddResource("vsphere_virtual_machine", "windows_vm")
                .AddAttribute("name", new VariableReference("vm_name"))
                .AddAttribute("num_cpus", new LiteralValue(8))
                .AddAttribute("memory", new Expression("64 * 1024"))
                .AddNestedBlock("disk")
                    .AddAttribute("label", new LiteralValue("disk0"))
                    .AddAttribute("size", new LiteralValue(40))
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains("terraform {", hcl);
        Assert.Contains("vsphere = {", hcl);
        Assert.Contains(@"source  = ""hashicorp/vsphere""", hcl);
        Assert.Contains(@"version = "">= 2.5.0""", hcl);
        
        Assert.Contains(@"provider ""vsphere"" {", hcl);
        Assert.Contains("user = var.vsphere_user", hcl);
        Assert.Contains("allow_unverified_ssl = true", hcl);
        
        Assert.Contains(@"data ""vsphere_datacenter"" ""dc"" {", hcl);
        Assert.Contains("name = var.datacenter", hcl);
        
        Assert.Contains(@"resource ""vsphere_virtual_machine"" ""windows_vm"" {", hcl);
        Assert.Contains("name = var.vm_name", hcl);
        Assert.Contains("num_cpus = 8", hcl);
        Assert.Contains("memory = 64 * 1024", hcl);
        Assert.Contains("disk {", hcl);
        Assert.Contains("label = \"disk0\"", hcl);
        Assert.Contains("size = 40", hcl);
    }
}
