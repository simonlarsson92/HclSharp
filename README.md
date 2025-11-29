# HclSharp

A C# library for programmatically generating Terraform HCL (HashiCorp Configuration Language) files using a fluent, object-oriented API.

## Features

- **Type-Safe** - Strongly-typed classes representing Terraform constructs
- **No String Interpolation** - Build configurations using objects and properties
- **Functional Core, Imperative Shell (FCIS)** - Clean architecture for easy testing
- **Fluent API** - Method chaining for readable configuration
- **Provider-Agnostic** - Works with any Terraform provider without hardcoded schemas
- **Complete Terraform Support** - Variables, providers, data sources, resources, and nested blocks
- **File Generation** - Generate separate files for main config, variables, and tfvars

## Installation

TODO: Add NuGet package installation instructions when published.

## Quick Start

```csharp
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;
using HclSharp.Shell.IO;

var config = new TerraformDocumentBuilder()
    // Add required provider
    .AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0")
    
    // Define variables
    .AddVariable("vm_name")
        .WithType("string")
        .WithDescription("Name of the virtual machine")
        .Build()
        
    // Configure provider
    .AddProvider("vsphere")
        .AddAttribute("user", TerraformValue.Variable("vsphere_user"))
        .AddAttribute("password", TerraformValue.Variable("vsphere_password"))
        .AddAttribute("allow_unverified_ssl", true)
        .Build()
        
    // Add data source
    .AddDataSource("vsphere_datacenter", "dc")
        .AddAttribute("name", TerraformValue.Variable("datacenter"))
        .Build()
        
    // Add resource with nested blocks
    .AddResource("vsphere_virtual_machine", "vm")
        .AddAttribute("name", TerraformValue.Variable("vm_name"))
        .AddAttribute("num_cpus", 4)
        .AddAttribute("memory", TerraformValue.Expr("8 * 1024"))
        .AddNestedBlock("disk")
            .AddAttribute("label", "disk0")
            .AddAttribute("size", 40)
            .EndNestedBlock()
        .Build();

// Generate complete HCL
string hcl = config.ToHcl();

// Or write to separate files
config.WriteToFile("main.tf");                    // Main configuration
config.WriteVariablesToFile("variables.tf");      // Variable definitions only
```

## Variable Values (tfvars) Support

Generate variable value files separately:

```csharp
var variableValues = new VariableValuesBuilder()
    .AddVariable("vm_name", "my-test-vm")
    .AddVariable("datacenter", "DC1")
    .AddVariable("num_cpus", 8);

variableValues.WriteToFile("terraform.tfvars");
// Or generate HCL string
string tfvars = variableValues.ToHcl();
```

## Project Structure

```
HclSharp/
├── src/
│   ├── HclSharp.Core/           # Functional core - immutable data structures
│   │   ├── Model/               # Data records for Terraform constructs
│   │   │   ├── TerraformConfiguration.cs
│   │   │   ├── ResourceBlockData.cs
│   │   │   ├── VariableBlockData.cs
│   │   │   └── ...
│   │   ├── Values/              # Terraform value types
│   │   │   ├── TerraformValue.cs
│   │   │   ├── LiteralValue.cs
│   │   │   ├── VariableReference.cs
│   │   │   └── ...
│   │   ├── Validation/          # Input validation
│   │   └── HclGenerator.cs      # Pure HCL generation functions
│   │
│   └── HclSharp.Shell/          # Imperative shell - builders and I/O
│       ├── Builders/            # Fluent builders for configuration
│       │   ├── TerraformDocumentBuilder.cs
│       │   ├── VariableBuilder.cs
│       │   ├── ResourceBuilder.cs
│       │   └── ...
│       └── IO/                  # File operations
│           └── TerraformFileWriter.cs
│
├── tests/
│   ├── HclSharp.Core.Tests/     # Unit tests for core logic
│   ├── HclSharp.Shell.Tests/    # Unit tests for builders
│   └── HclSharp.IntegrationTests/ # End-to-end tests
│
└── samples/
    └── HclSharp.Samples/        # Example usage
```

## Architecture

HclSharp follows the **Functional Core, Imperative Shell (FCIS)** pattern:

### Functional Core (HclSharp.Core)
- **Pure functions** - No side effects, deterministic output
- **Immutable records** - Data structures that cannot be modified after creation
- **Highly testable** - Easy to unit test without mocks or complex setup
- **Domain models** - Rich types representing Terraform concepts

Key components:
- `TerraformConfiguration` - Root immutable configuration
- `HclGenerator` - Pure functions for generating HCL strings
- Value types (`TerraformValue`, `VariableReference`, `Expression`, etc.)
- Block data records (`ResourceBlockData`, `VariableBlockData`, etc.)

### Imperative Shell (HclSharp.Shell)
- **Fluent builders** - Mutable builders for constructing configurations
- **Side effects** - File I/O operations
- **User-friendly API** - Method chaining and convenience methods

Key components:
- `TerraformDocumentBuilder` - Main builder for complete configurations
- Specialized builders (`VariableBuilder`, `ResourceBuilder`, etc.)
- File writers for generating `.tf` and `.tfvars` files

## Core Concepts

### Terraform Values

HclSharp provides strongly-typed representations of Terraform values:

```csharp
// Literal values
TerraformValue.Literal("hello")        // "hello"
TerraformValue.Literal(42)             // 42
TerraformValue.Literal(true)           // true

// Variable references
TerraformValue.Variable("vm_name")     // var.vm_name

// Data source references
TerraformValue.DataRef("vsphere_datacenter", "dc", "id")  // data.vsphere_datacenter.dc.id

// Resource references
TerraformValue.ResourceRef("vsphere_vm", "main", "id")    // vsphere_vm.main.id

// Expressions
TerraformValue.Expr("64 * 1024")       // 64 * 1024

// Implicit conversions
TerraformValue name = "my-vm";         // Automatically becomes LiteralValue
TerraformValue count = 5;              // Automatically becomes LiteralValue
```

### Variables with Validation

```csharp
.AddVariable("instance_type")
    .WithType("string")
    .WithDefault("t3.micro")
    .WithDescription("EC2 instance type")
    .AddValidation(
        "contains([\"t3.micro\", \"t3.small\", \"t3.medium\"], var.instance_type)",
        "Instance type must be t3.micro, t3.small, or t3.medium")
    .Sensitive(false)
    .Nullable(false)
    .Build()
```

### Nested Blocks

```csharp
.AddResource("aws_instance", "web")
    .AddNestedBlock("ebs_block_device")
        .AddAttribute("device_name", "/dev/sda1")
        .AddAttribute("volume_type", "gp3")
        .AddAttribute("volume_size", 20)
        .EndNestedBlock()
    .AddNestedBlock("network_interface")
        .AddAttribute("network_interface_id", TerraformValue.Variable("network_id"))
        .AddAttribute("device_index", 0)
        .EndNestedBlock()
    .Build()
```

### File Generation Options

```csharp
var config = new TerraformDocumentBuilder()
    // ... build configuration

// Option 1: Everything in one file
config.WriteToFile("main.tf");

// Option 2: Separate variables from main config
config.WriteToFile("main.tf", excludeVariables: true);  // Main config without variables
config.WriteVariablesToFile("variables.tf");            // Variables only

// Option 3: Include variable values
var values = new VariableValuesBuilder()
    .AddVariable("region", "us-west-2")
    .AddVariable("instance_count", 3);
    
values.WriteToFile("terraform.tfvars");
```

## Advanced Usage

### Complex Resource Configuration

```csharp
.AddResource("aws_launch_template", "web")
    .AddAttribute("name_prefix", "web-")
    .AddAttribute("image_id", TerraformValue.Variable("ami_id"))
    .AddAttribute("instance_type", TerraformValue.Variable("instance_type"))
    
    .AddNestedBlock("vpc_security_group_ids")
        .AddAttribute("security_groups", TerraformValue.Variable("security_group_ids"))
        .EndNestedBlock()
        
    .AddNestedBlock("block_device_mappings")
        .AddAttribute("device_name", "/dev/sda1")
        .AddNestedBlock("ebs")
            .AddAttribute("volume_size", 20)
            .AddAttribute("volume_type", "gp3")
            .AddAttribute("encrypted", true)
            .EndNestedBlock()
        .EndNestedBlock()
    .Build()
```

### Provider Configuration with Multiple Providers

```csharp
var config = new TerraformDocumentBuilder()
    .AddRequiredProvider("aws", "hashicorp/aws", "~> 5.0")
    .AddRequiredProvider("random", "hashicorp/random", "~> 3.0")
    
    .AddProvider("aws")
        .AddAttribute("region", TerraformValue.Variable("aws_region"))
        .AddAttribute("profile", TerraformValue.Variable("aws_profile"))
        .Build()
        
    .AddProvider("aws")  // Second provider with alias
        .AddAttribute("alias", "backup")
        .AddAttribute("region", "us-east-1")
        .Build();
```

## Testing

The FCIS architecture makes testing straightforward:

```csharp
// Test the functional core without any setup
var config = new TerraformConfiguration(/* ... */);
var hcl = HclGenerator.GenerateHcl(config);
Assert.Contains("resource \"aws_instance\"", hcl);

// Test builders with simple assertions
var builder = new ResourceBuilder("aws_instance", "test", documentBuilder)
    .AddAttribute("instance_type", "t3.micro");
    
var resource = builder.Build();
Assert.Equal("t3.micro", resource.Attributes["instance_type"]);
```

## Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

### Development Setup

```bash
git clone https://github.com/your-username/HclSharp.git
cd HclSharp
dotnet restore
dotnet test
dotnet run --project samples/HclSharp.Samples
```

## License

MIT License - see LICENSE file for details
