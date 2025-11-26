using HclSharp.Core;
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;

namespace HclSharp.IntegrationTests;

/// <summary>
/// Integration tests that verify complete Terraform configurations can be generated
/// using the builder API and match expected HCL output.
/// </summary>
public class VsphereExampleTests
{
    [Fact]
    public void BuildVsphereExample_ShouldGenerateCompleteHcl()
    {
        // Arrange & Act - Build the complete vsphere example from original requirements
        var config = new TerraformDocumentBuilder()
            .AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0")
            .AddProvider("vsphere")
                .AddAttribute("user", TerraformValue.Variable("vsphere_user"))
                .AddAttribute("password", TerraformValue.Variable("vsphere_password"))
                .AddAttribute("vsphere_server", TerraformValue.Variable("vsphere_server"))
                .AddAttribute("allow_unverified_ssl", true)
                .Build()
            .AddDataSource("vsphere_datacenter", "dc")
                .AddAttribute("name", TerraformValue.Variable("datacenter"))
                .Build()
            .AddResource("vsphere_virtual_machine", "windows_vm")
                .AddAttribute("name", TerraformValue.Variable("vm_name"))
                .AddAttribute("num_cpus", 8)
                .AddAttribute("memory", TerraformValue.Expr("64 * 1024"))
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk0")
                    .AddAttribute("size", 40)
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert - Verify all required elements are present
        Assert.Contains("terraform {", hcl);
        Assert.Contains("required_providers {", hcl);
        Assert.Contains("vsphere = {", hcl);
        Assert.Contains(@"source  = ""hashicorp/vsphere""", hcl);
        Assert.Contains(@"version = "">= 2.5.0""", hcl);
        
        Assert.Contains(@"provider ""vsphere"" {", hcl);
        Assert.Contains("user = var.vsphere_user", hcl);
        Assert.Contains("password = var.vsphere_password", hcl);
        Assert.Contains("vsphere_server = var.vsphere_server", hcl);
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
    
    [Fact]
    public void BuildVsphereExample_WithImplicitConversions_ShouldWork()
    {
        // Arrange & Act - Use implicit conversions for values
        var config = new TerraformDocumentBuilder()
            .AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0")
            .AddProvider("vsphere")
                .AddAttribute("user", TerraformValue.Variable("vsphere_user"))
                .AddAttribute("allow_unverified_ssl", true)  // implicit bool conversion
                .Build()
            .AddDataSource("vsphere_datacenter", "dc")
                .AddAttribute("name", TerraformValue.Variable("datacenter"))
                .Build()
            .AddResource("vsphere_virtual_machine", "windows_vm")
                .AddAttribute("name", "my-vm")  // implicit string conversion
                .AddAttribute("num_cpus", 8)    // implicit int conversion
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk0")  // implicit string conversion
                    .AddAttribute("size", 40)        // implicit int conversion
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert - Verify implicit conversions work correctly
        Assert.Contains("allow_unverified_ssl = true", hcl);
        Assert.Contains("name = \"my-vm\"", hcl);
        Assert.Contains("num_cpus = 8", hcl);
        Assert.Contains("label = \"disk0\"", hcl);
        Assert.Contains("size = 40", hcl);
    }
    
    [Fact]
    public void BuildComplexResource_WithMultipleNestedBlocks_ShouldGenerateCorrectly()
    {
        // Arrange & Act - Build a more complex resource with multiple nested blocks
        var config = new TerraformDocumentBuilder()
            .AddResource("vsphere_virtual_machine", "complex_vm")
                .AddAttribute("name", "complex-vm")
                .AddAttribute("num_cpus", 16)
                .AddAttribute("memory", 32768)
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk0")
                    .AddAttribute("size", 100)
                    .AddAttribute("thin_provisioned", true)
                    .EndNestedBlock()
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk1")
                    .AddAttribute("size", 500)
                    .AddAttribute("thin_provisioned", false)
                    .EndNestedBlock()
                .AddNestedBlock("network_interface")
                    .AddAttribute("network_id", TerraformValue.DataRef("vsphere_network", "net", "id"))
                    .EndNestedBlock()
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert
        Assert.Contains(@"resource ""vsphere_virtual_machine"" ""complex_vm"" {", hcl);
        Assert.Contains("name = \"complex-vm\"", hcl);
        Assert.Contains("num_cpus = 16", hcl);
        Assert.Contains("memory = 32768", hcl);
        
        // Verify both disk blocks
        Assert.Contains("label = \"disk0\"", hcl);
        Assert.Contains("size = 100", hcl);
        Assert.Contains("thin_provisioned = true", hcl);
        
        Assert.Contains("label = \"disk1\"", hcl);
        Assert.Contains("size = 500", hcl);
        Assert.Contains("thin_provisioned = false", hcl);
        
        // Verify network interface block
        Assert.Contains("network_interface {", hcl);
        Assert.Contains("network_id = data.vsphere_network.net.id", hcl);
    }
    
    [Fact]
    public void BuildConfiguration_WithAllBlockTypes_ShouldHaveCorrectOrder()
    {
        // Arrange & Act
        var config = new TerraformDocumentBuilder()
            .AddRequiredProvider("test", "hashicorp/test", "1.0.0")
            .AddProvider("test")
                .AddAttribute("region", "us-west-2")
                .Build()
            .AddDataSource("test_data", "example")
                .AddAttribute("filter", "value")
                .Build()
            .AddResource("test_resource", "example")
                .AddAttribute("name", "test")
                .Build()
            .Build();
        
        var hcl = HclGenerator.GenerateHcl(config);
        
        // Assert - Verify blocks appear in correct order
        var terraformIndex = hcl.IndexOf("terraform {");
        var providerIndex = hcl.IndexOf("provider \"test\"");
        var dataIndex = hcl.IndexOf("data \"test_data\"");
        var resourceIndex = hcl.IndexOf("resource \"test_resource\"");
        
        Assert.True(terraformIndex >= 0, "terraform block should exist");
        Assert.True(providerIndex > terraformIndex, "provider should come after terraform block");
        Assert.True(dataIndex > providerIndex, "data source should come after provider");
        Assert.True(resourceIndex > dataIndex, "resource should come after data source");
    }
}
