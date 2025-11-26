# HclSharp

A C# library for programmatically generating Terraform HCL (HashiCorp Configuration Language) files using a fluent, object-oriented API.

## Features

- **Type-Safe** - Strongly-typed classes representing Terraform constructs
- **No String Interpolation** - Build configurations using objects and properties
- **Functional Core, Imperative Shell (FCIS)** - Clean architecture for easy testing
- **Fluent API** - Method chaining for readable configuration
- **Provider-Agnostic** - Works with any Terraform provider without hardcoded schemas

## Installation

```bash
dotnet add package HclSharp.Shell
```

Or if you only need the core generation logic without builders:

```bash
dotnet add package HclSharp.Core
```

## Quick Start

```csharp
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;
using HclSharp.Shell.IO;

var doc = new TerraformDocumentBuilder();

// Add required provider
doc.AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0");

// Configure provider
doc.AddProvider("vsphere")
    .AddAttribute("user", new VariableReference("vsphere_user"))
    .AddAttribute("password", new VariableReference("vsphere_password"))
    .AddAttribute("vsphere_server", new VariableReference("vsphere_server"))
    .AddAttribute("allow_unverified_ssl", new LiteralValue(true))
    .Build();

// Add data source
doc.AddDataSource("vsphere_datacenter", "dc")
    .AddAttribute("name", new VariableReference("datacenter"))
    .Build();

// Add resource with nested blocks
doc.AddResource("vsphere_virtual_machine", "windows_vm")
    .AddAttribute("name", new VariableReference("vm_name"))
    .AddAttribute("num_cpus", new LiteralValue(8))
    .AddAttribute("memory", new Expression("64 * 1024"))
    .AddNestedBlock("disk")
        .AddAttribute("label", new LiteralValue("disk0"))
        .AddAttribute("size", new LiteralValue(40));

// Generate HCL
string hcl = doc.ToHcl();

// Or write directly to file
doc.WriteToFile("main.tf");
```

## Project Structure

- **HclSharp.Core** - Immutable data structures and pure HCL generation functions
- **HclSharp.Shell** - Mutable builders and file I/O operations

## Architecture

HclSharp follows the Functional Core, Imperative Shell (FCIS) pattern:

- **Functional Core (HclSharp.Core)** - Pure functions, immutable records, highly testable
- **Imperative Shell (HclSharp.Shell)** - Fluent builders, side effects (file I/O)

This makes the core logic easy to test without mocks or complex setup.

## License

MIT License - see LICENSE file for details

## Contributing

Contributions welcome! Please open an issue or PR on GitHub.
