using System;
using System.Collections.Generic;
using System.Text;
using HclSharp.Core;
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;
using HclSharp.Shell.IO;
using Xunit;

namespace HclSharp.IntegrationTests;

/// <summary>
/// Integration tests for variable blocks that test the complete build flow
/// and integration with other Terraform constructs.
/// </summary>
public class VariableBlockIntegrationTests
{
    [Fact]
    public void BuildVariables_FullFlow_ShouldGenerateCompleteConfiguration()
    {
        // Arrange & Act - Build a complete configuration with variables using the full builder flow
        var config = new TerraformDocumentBuilder()
            // Add variables first
            .AddVariable("environment")
                .WithType("string")
                .WithDescription("Environment name (dev, staging, prod)")
                .AddValidation(
                    new("contains([\"dev\", \"staging\", \"prod\"], var.environment)"),
                    "Environment must be dev, staging, or prod")
                .Build()
                
            .AddVariable("instance_count")
                .WithType("number")
                .WithDefault(2)
                .WithDescription("Number of instances to create")
                .AddValidation(new("var.instance_count > 0"), "Instance count must be positive")
                .AddValidation(new("var.instance_count <= 10"), "Instance count cannot exceed 10")
                .Build()
                
            .AddVariable("enable_monitoring")
                .WithType("bool")
                .WithDefault(true)
                .WithDescription("Enable CloudWatch monitoring")
                .Build()
                
            .AddVariable("db_password")
                .WithType("string")
                .WithDescription("Database password")
                .Sensitive(true)
                .AddValidation(new("length(var.db_password) >= 8"), "Password must be at least 8 characters")
                .Build()
                
            // Add other terraform constructs that use the variables
            .AddRequiredProvider("aws", "hashicorp/aws", "~> 5.0")
            
            .AddProvider("aws")
                .AddAttribute("region", TerraformValue.Variable("aws_region"))
                .Build()
                
            .AddDataSource("aws_ami", "ubuntu")
                .AddAttribute("most_recent", true)
                .AddAttribute("owners", TerraformValue.Expr("[\"099720109477\"]"))
                .Build()
                
            .AddResource("aws_instance", "web")
                .AddAttribute("ami", TerraformValue.DataRef("aws_ami", "ubuntu", "id"))
                .AddAttribute("instance_type", "t3.micro")
                .AddAttribute("count", TerraformValue.Variable("instance_count"))
                .AddNestedBlock("tags")
                    .AddAttribute("Name", TerraformValue.Expr("\"web-${var.environment}-${count.index}\""))
                    .AddAttribute("Environment", TerraformValue.Variable("environment"))
                    .AddAttribute("Monitoring", TerraformValue.Variable("enable_monitoring"))
                    .EndNestedBlock()
                .Build()
                
            .Build();

        var hcl = HclGenerator.GenerateHcl(config);

        // Assert - Verify all variables are generated correctly
        Assert.Contains("variable \"environment\" {", hcl);
        Assert.Contains("type = string", hcl);
        Assert.Contains("description = \"Environment name (dev, staging, prod)\"", hcl);
        Assert.Contains("contains([\"dev\", \"staging\", \"prod\"], var.environment)", hcl);
        
        Assert.Contains("variable \"instance_count\" {", hcl);
        Assert.Contains("type = number", hcl);
        Assert.Contains("default = 2", hcl);
        Assert.Contains("var.instance_count > 0", hcl);
        Assert.Contains("var.instance_count <= 10", hcl);
        
        Assert.Contains("variable \"enable_monitoring\" {", hcl);
        Assert.Contains("type = bool", hcl);
        Assert.Contains("default = true", hcl);
        
        Assert.Contains("variable \"db_password\" {", hcl);
        Assert.Contains("sensitive = true", hcl);
        Assert.Contains("length(var.db_password) >= 8", hcl);

        // Verify variables are used correctly in other constructs
        Assert.Contains("count = var.instance_count", hcl);
        Assert.Contains("\"web-${var.environment}-${count.index}\"", hcl);
        Assert.Contains("Environment = var.environment", hcl);
        Assert.Contains("Monitoring = var.enable_monitoring", hcl);
    }

    [Fact]
    public void BuildVariables_BlockOrderValidation_VariablesShouldAppearFirst()
    {
        // Arrange & Act - Build configuration to test block ordering
        var config = new TerraformDocumentBuilder()
            .AddRequiredProvider("aws", "hashicorp/aws", "~> 5.0")
            
            // Add variables
            .AddVariable("region")
                .WithType("string")
                .WithDefault("us-west-2")
                .Build()
                
            .AddVariable("instance_type")
                .WithType("string")
                .WithDefault("t3.micro")
                .Build()
                
            // Add provider
            .AddProvider("aws")
                .AddAttribute("region", TerraformValue.Variable("region"))
                .Build()
                
            // Add data source
            .AddDataSource("aws_availability_zones", "available")
                .AddAttribute("state", "available")
                .Build()
                
            // Add resource
            .AddResource("aws_instance", "example")
                .AddAttribute("instance_type", TerraformValue.Variable("instance_type"))
                .Build()
                
            .Build();

        var hcl = HclGenerator.GenerateHcl(config);

        // Assert - Verify correct block ordering
        var terraformBlockIndex = hcl.IndexOf("terraform {");
        var firstVariableIndex = hcl.IndexOf("variable \"region\"");
        var secondVariableIndex = hcl.IndexOf("variable \"instance_type\"");
        var providerIndex = hcl.IndexOf("provider \"aws\"");
        var dataIndex = hcl.IndexOf("data \"aws_availability_zones\"");
        var resourceIndex = hcl.IndexOf("resource \"aws_instance\"");

        // Variables should appear after terraform block but before everything else
        Assert.True(terraformBlockIndex < firstVariableIndex, "Variables should come after terraform block");
        Assert.True(firstVariableIndex < secondVariableIndex, "Variables should be in order");
        Assert.True(secondVariableIndex < providerIndex, "Variables should come before provider");
        Assert.True(providerIndex < dataIndex, "Provider should come before data sources");
        Assert.True(dataIndex < resourceIndex, "Data sources should come before resources");
    }

    [Fact]
    public void BuildVariables_CombinedWithResourcesAndProviders_ShouldWorkTogether()
    {
        // Arrange & Act - Test complex integration between variables and other constructs
        var config = new TerraformDocumentBuilder()
            // Define comprehensive variables
            .AddVariable("project_name")
                .WithType("string")
                .WithDescription("Project name for resource naming")
                .Build()
                
            .AddVariable("allowed_cidr_blocks")
                .WithType("list(string)")
                .WithDefault(TerraformValue.Expr("[\"10.0.0.0/8\", \"172.16.0.0/12\"]"))
                .WithDescription("CIDR blocks allowed for access")
                .Build()
                
            .AddVariable("instance_config")
                .WithType("object({\n    type = string\n    size = number\n  })")
                .WithDefault(TerraformValue.Expr("{\n    type = \"t3.micro\"\n    size = 20\n  }"))
                .WithDescription("Instance configuration object")
                .Build()

            // Add required provider
            .AddRequiredProvider("aws", "hashicorp/aws", "~> 5.0")
            
            // Configure provider using variables
            .AddProvider("aws")
                .AddAttribute("region", TerraformValue.Variable("aws_region"))
                .AddAttribute("default_tags", TerraformValue.Expr("{\n    Project = var.project_name\n  }"))
                .Build()
                
            // Data sources that reference variables
            .AddDataSource("aws_vpc", "main")
                .AddAttribute("default", true)
                .Build()
                
            // Resource with complex variable usage
            .AddResource("aws_security_group", "web")
                .AddAttribute("name", TerraformValue.Expr("\"${var.project_name}-web-sg\""))
                .AddAttribute("vpc_id", TerraformValue.DataRef("aws_vpc", "main", "id"))
                .AddNestedBlock("ingress")
                    .AddAttribute("from_port", 80)
                    .AddAttribute("to_port", 80)
                    .AddAttribute("protocol", "tcp")
                    .AddAttribute("cidr_blocks", TerraformValue.Variable("allowed_cidr_blocks"))
                    .EndNestedBlock()
                .AddNestedBlock("tags")
                    .AddAttribute("Name", TerraformValue.Expr("\"${var.project_name}-web-security-group\""))
                    .EndNestedBlock()
                .Build()
                
            .AddResource("aws_instance", "web")
                .AddAttribute("instance_type", TerraformValue.Expr("var.instance_config.type"))
                .AddAttribute("vpc_security_group_ids", TerraformValue.Expr("[aws_security_group.web.id]"))
                .AddNestedBlock("root_block_device")
                    .AddAttribute("volume_size", TerraformValue.Expr("var.instance_config.size"))
                    .EndNestedBlock()
                .Build()
                
            .Build();

        var hcl = HclGenerator.GenerateHcl(config);

        // Assert - Verify complex variable integration
        
        // Variable definitions
        Assert.Contains("variable \"project_name\" {", hcl);
        Assert.Contains("variable \"allowed_cidr_blocks\" {", hcl);
        Assert.Contains("variable \"instance_config\" {", hcl);
        Assert.Contains("type = list(string)", hcl);
        Assert.Contains("type = object({", hcl);
        
        // Variable usage in provider
        Assert.Contains("Project = var.project_name", hcl);
        
        // Variable usage in resources
        Assert.Contains("\"${var.project_name}-web-sg\"", hcl);
        Assert.Contains("cidr_blocks = var.allowed_cidr_blocks", hcl);
        Assert.Contains("\"${var.project_name}-web-security-group\"", hcl);
        Assert.Contains("var.instance_config.type", hcl);
        Assert.Contains("var.instance_config.size", hcl);
        
        // Verify complex default values work (using Expression, not Literal)
        Assert.Contains("[\"10.0.0.0/8\", \"172.16.0.0/12\"]", hcl);
    }

    [Fact]
    public void BuildVariables_WithFileGeneration_ShouldSeparateVariablesCorrectly()
    {
        // Arrange - Build configuration with variables
        var builder = new TerraformDocumentBuilder()
            .AddVariable("environment")
                .WithType("string")
                .WithDescription("Environment name")
                .Build()
                
            .AddVariable("instance_count")
                .WithType("number")
                .WithDefault(1)
                .Build()
                
            .AddProvider("aws")
                .AddAttribute("region", "us-west-2")
                .Build()
                
            .AddResource("aws_instance", "web")
                .AddAttribute("instance_type", "t3.micro")
                .AddAttribute("count", TerraformValue.Variable("instance_count"))
                .Build();

        // Act & Assert - Test different file generation options
        
        // Test main configuration with variables
        var mainWithVariables = builder.ToHcl();
        Assert.Contains("variable \"environment\"", mainWithVariables);
        Assert.Contains("variable \"instance_count\"", mainWithVariables);
        Assert.Contains("provider \"aws\"", mainWithVariables);
        Assert.Contains("resource \"aws_instance\"", mainWithVariables);
        
        // Test main configuration without variables
        var mainWithoutVariables = builder.ToHcl(excludeVariables: true);
        Assert.DoesNotContain("variable \"environment\"", mainWithoutVariables);
        Assert.DoesNotContain("variable \"instance_count\"", mainWithoutVariables);
        Assert.Contains("provider \"aws\"", mainWithoutVariables);
        Assert.Contains("resource \"aws_instance\"", mainWithoutVariables);
        
        // Test variables-only output
        var variablesOnly = builder.ToVariablesHcl();
        Assert.Contains("variable \"environment\"", variablesOnly);
        Assert.Contains("variable \"instance_count\"", variablesOnly);
        Assert.DoesNotContain("provider \"aws\"", variablesOnly);
        Assert.DoesNotContain("resource \"aws_instance\"", variablesOnly);
    }

    [Fact]
    public void BuildVariables_ComplexValidationScenarios_ShouldGenerateCorrectly()
    {
        // Arrange & Act - Test complex validation scenarios
        var config = new TerraformDocumentBuilder()
            .AddVariable("instance_type")
                .WithType("string")
                .WithDefault("t3.micro")
                .WithDescription("EC2 instance type")
                .AddValidation(
                    new("contains([\"t3.micro\", \"t3.small\", \"t3.medium\", \"t3.large\"], var.instance_type)"),
                    "Instance type must be a valid t3 instance type")
                .AddValidation(
                    new("can(regex(\"^t3\\\\.\", var.instance_type))"),
                    "Instance type must be from the t3 family")
                .Build()
                
            .AddVariable("subnet_cidrs")
                .WithType("list(string)")
                .WithDescription("Subnet CIDR blocks")
                .AddValidation(
                    new("alltrue([for cidr in var.subnet_cidrs : can(cidrhost(cidr, 0))])"),
                    "All subnet CIDRs must be valid")
                .AddValidation(
                    new("length(var.subnet_cidrs) >= 2"),
                    "At least 2 subnet CIDRs must be provided")
                .Build()
                
            .AddResource("aws_instance", "validated")
                .AddAttribute("instance_type", TerraformValue.Variable("instance_type"))
                .Build()
                
            .Build();

        var hcl = HclGenerator.GenerateHcl(config);

        // Assert - Verify complex validations are generated correctly
        Assert.Contains("variable \"instance_type\" {", hcl);
        Assert.Contains("contains([\"t3.micro\", \"t3.small\", \"t3.medium\", \"t3.large\"], var.instance_type)", hcl);
        Assert.Contains("can(regex(\"^t3\\\\.\", var.instance_type))", hcl);
        Assert.Contains("Instance type must be a valid t3 instance type", hcl);
        Assert.Contains("Instance type must be from the t3 family", hcl);
        
        Assert.Contains("variable \"subnet_cidrs\" {", hcl);
        Assert.Contains("alltrue([for cidr in var.subnet_cidrs : can(cidrhost(cidr, 0))])", hcl);
        Assert.Contains("length(var.subnet_cidrs) >= 2", hcl);
        Assert.Contains("All subnet CIDRs must be valid", hcl);
        Assert.Contains("At least 2 subnet CIDRs must be provided", hcl);

        // Extract just the instance_type variable block
        var instanceTypeStart = hcl.IndexOf("variable \"instance_type\"");
        var instanceTypeEnd = hcl.IndexOf("variable \"subnet_cidrs\"");
        var instanceTypeBlock = hcl.Substring(instanceTypeStart, instanceTypeEnd - instanceTypeStart);

        // Count "validation {" in that specific block
        var instanceTypeValidationCount = instanceTypeBlock.Split("validation {", StringSplitOptions.None).Length - 1;
        
        Assert.Equal(2, instanceTypeValidationCount);
    }
}
