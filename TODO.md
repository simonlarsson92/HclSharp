# HclSharp TODO List

Last Updated: November 26, 2025

## Completed ✅

### Critical Security Fixes
- ✅ **Expression Code Injection Vulnerability** - Added validation to prevent HCL injection attacks via Expression strings
- ✅ **.NET Target Framework** - Confirmed net10.0 is correct (latest LTS)
- ✅ **Input Validation** - Comprehensive validation for all user-controlled strings (identifiers, attributes, provider sources)

---

## High Priority 🟠

### 1. Create GitHub Actions CI/CD Pipeline
**Priority**: HIGH  
**Impact**: Development workflow, quality assurance

**Tasks**:
- Create `.github/workflows/ci.yml` with:
  - Build on push and PR
  - Run all tests on multiple platforms (Windows, Linux, macOS)
  - Code coverage with Coverlet
  - Static analysis integration
  - NuGet package publishing on releases
- Add status badges to README.md
- Configure branch protection rules

**Benefits**:
- Automated testing catches bugs early
- Consistent build process
- Professional project appearance

---

### 2. Add Support for Terraform `locals` Blocks
**Priority**: HIGH  
**Impact**: Core Terraform functionality

**Tasks**:
- Create `LocalsBlockData` model
  ```csharp
  public record LocalsBlockData(ImmutableDictionary<string, TerraformValue> Locals);
  ```
- Update `TerraformConfiguration` to include `LocalsBlock` property
- Add `AddLocal(string name, TerraformValue value)` method to `TerraformDocumentBuilder`
- Implement HCL generation in `HclGenerator.GenerateLocalsBlock()`
- Add comprehensive tests

**Example Output**:
```hcl
locals {
  common_tags = {
    Environment = "production"
    Project     = "myapp"
  }
  instance_count = 3
}
```

---

### 3. Add Support for Terraform `output` Blocks
**Priority**: HIGH  
**Impact**: Core Terraform functionality

**Tasks**:
- Create `OutputBlockData` model
  ```csharp
  public record OutputBlockData(
      string Name,
      TerraformValue Value,
      string? Description,
      bool Sensitive);
  ```
- Update `TerraformConfiguration` to include `Outputs` list
- Add `AddOutput()` builder method
- Implement HCL generation
- Add validation and tests

**Example Output**:
```hcl
output "instance_ip" {
  value       = aws_instance.web.public_ip
  description = "The public IP of the web server"
  sensitive   = false
}
```

---

### 4. Add Support for Terraform `variable` Blocks
**Priority**: HIGH  
**Impact**: Core Terraform functionality

**Tasks**:
- Create `VariableBlockData` model with:
  - Name
  - Type (string, number, bool, list, map, object)
  - Default value (optional)
  - Description
  - Validation rules (optional)
  - Sensitive flag
- Update `TerraformConfiguration`
- Add `AddVariable()` builder
- Implement HCL generation with proper type syntax
- Add tests for all type variations

**Example Output**:
```hcl
variable "instance_count" {
  type        = number
  description = "Number of instances to create"
  default     = 2
  validation {
    condition     = var.instance_count > 0
    error_message = "Must be greater than 0"
  }
}
```

---

### 5. Implement HCL Complex Types (Lists, Maps, Objects)
**Priority**: HIGH  
**Impact**: Essential for real-world Terraform configurations

**Tasks**:
- Create new value types:
  - `ListValue(ImmutableList<TerraformValue> Items)`
  - `MapValue(ImmutableDictionary<string, TerraformValue> Items)`
  - `ObjectValue(ImmutableDictionary<string, TerraformValue> Attributes)`
  - `SetValue(ImmutableHashSet<TerraformValue> Items)`
- Update `HclGenerator.FormatValue()` to handle these types
- Add recursive formatting for nested structures
- Add implicit conversions for convenience
- Comprehensive tests for nested and complex structures

**Example**:
```csharp
// List
new ListValue(ImmutableList.Create(
    new LiteralValue("item1"),
    new LiteralValue("item2")
));

// Map
new MapValue(ImmutableDictionary<string, TerraformValue>.Empty
    .Add("key1", new LiteralValue("value1"))
    .Add("key2", new LiteralValue("value2")));
```

**Output**:
```hcl
my_list = ["item1", "item2"]
my_map = {
  key1 = "value1"
  key2 = "value2"
}
```

---

## Medium Priority 🟡

### 6. Add Path Traversal Protection
**Priority**: MEDIUM  
**Impact**: Security

**Tasks**:
- Locate file writing code (likely `TerraformFileWriter`)
- Add path validation before file operations:
  ```csharp
  var fullPath = Path.GetFullPath(outputPath);
  var allowedDirectory = Path.GetFullPath(baseDirectory);
  if (!fullPath.StartsWith(allowedDirectory))
      throw new SecurityException("Path traversal attempt detected");
  ```
- Add tests for path traversal attempts
- Document safe usage patterns

---

### 7. Enhance String Escaping
**Priority**: MEDIUM  
**Impact**: Correctness

**Tasks**:
- Update `HclGenerator.EscapeString()` to handle:
  - Unicode escapes: `\uXXXX`
  - Backspace: `\b`
  - Form feed: `\f`
  - Dollar signs for interpolation: `${...}` → `$${...}`
- Add comprehensive escape tests
- Handle edge cases (already escaped strings)

---

### 8. Replace string.Format with String Interpolation
**Priority**: MEDIUM  
**Impact**: Code quality, readability, performance

**Tasks**:
- Refactor all `string.Format()` calls in `HclGenerator.cs` to use `$"..."` syntax
- Example: `string.Format("{0}{1} = {2}", Indent(1), key, value)` → `$"{Indent(1)}{key} = {value}"`
- Run benchmarks to verify performance improvement
- Update code style guidelines

**Impact**: ~15 occurrences in HclGenerator.cs

---

### 9. Optimize Indent Method with Caching
**Priority**: MEDIUM  
**Impact**: Performance

**Tasks**:
- Create cached indentation strings:
  ```csharp
  private static readonly string[] IndentCache = 
      Enumerable.Range(0, 10).Select(i => new string(' ', i * 2)).ToArray();
  
  private static string Indent(int level) =>
      level < IndentCache.Length ? IndentCache[level] : new string(' ', level * 2);
  ```
- Add benchmarks to measure improvement
- Document performance characteristics

**Expected Impact**: Reduced string allocations during HCL generation

---

### 10. Add Support for `count` and `for_each` Meta-Arguments
**Priority**: MEDIUM  
**Impact**: Advanced Terraform features

**Tasks**:
- Add meta-argument properties to `ResourceBlockData` and `DataSourceBlockData`:
  ```csharp
  public TerraformValue? Count { get; init; }
  public TerraformValue? ForEach { get; init; }
  ```
- Update builders to support meta-arguments
- Update HCL generation to output meta-arguments before regular attributes
- Add tests for count/for_each scenarios

**Example**:
```hcl
resource "aws_instance" "server" {
  count         = 3
  ami           = "ami-123456"
  instance_type = "t2.micro"
}
```

---

### 11. Add Support for `depends_on` Meta-Argument
**Priority**: MEDIUM  
**Impact**: Resource dependencies

**Tasks**:
- Add `DependsOn` property (list of resource references)
- Update builders
- Generate proper syntax: `depends_on = [resource.type.name, ...]`
- Add validation for valid resource references
- Add tests

---

### 12. Add Support for `lifecycle` Blocks
**Priority**: MEDIUM  
**Impact**: Advanced resource management

**Tasks**:
- Create `LifecycleBlockData` model:
  ```csharp
  public record LifecycleBlockData(
      bool? CreateBeforeDestroy,
      bool? PreventDestroy,
      ImmutableList<string>? IgnoreChanges);
  ```
- Add to resources
- Generate HCL syntax
- Add tests for all lifecycle options

**Example**:
```hcl
resource "aws_instance" "web" {
  lifecycle {
    create_before_destroy = true
    prevent_destroy       = false
    ignore_changes        = [tags]
  }
}
```

---

### 13. Add Sensitive Attribute Detection
**Priority**: MEDIUM  
**Impact**: Security, developer experience

**Tasks**:
- Create list of sensitive attribute patterns:
  ```csharp
  private static readonly string[] SensitivePatterns = {
      "password", "secret", "token", "api_key", "private_key",
      "access_key", "credential", "auth"
  };
  ```
- Detect sensitive attributes during building
- Log warnings when sensitive values aren't using variables
- Mask sensitive values in error messages
- Add configuration option to customize patterns

---

### 14. Add Support for Provider Alias
**Priority**: MEDIUM  
**Impact**: Multi-region/account configurations

**Tasks**:
- Add `Alias` property to `ProviderBlockData`
- Update builder: `AddProvider("aws").WithAlias("west")`
- Update HCL generation to include alias
- Update resources to reference aliased providers
- Add tests

**Example**:
```hcl
provider "aws" {
  alias  = "west"
  region = "us-west-2"
}

resource "aws_instance" "web" {
  provider = aws.west
}
```

---

### 15. Add Support for `module` Blocks
**Priority**: MEDIUM  
**Impact**: Code reuse, modularity

**Tasks**:
- Create `ModuleBlockData` model:
  ```csharp
  public record ModuleBlockData(
      string Name,
      string Source,
      string? Version,
      ImmutableDictionary<string, TerraformValue> Arguments);
  ```
- Add builder methods
- Generate HCL with all module arguments
- Add tests for local and remote modules

**Example**:
```hcl
module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "3.0.0"
  
  name = "my-vpc"
  cidr = "10.0.0.0/16"
}
```

---

### 16. Create .editorconfig File
**Priority**: MEDIUM  
**Impact**: Code consistency

**Tasks**:
- Create `.editorconfig` with C# formatting rules:
  - Indent size: 4 spaces
  - Line endings: CRLF (Windows) / LF (Unix)
  - Charset: UTF-8
  - Trim trailing whitespace
  - Insert final newline
- Configure C# specific rules (brace style, naming conventions)
- Document in CONTRIBUTING.md

---

### 17. Create Directory.Build.props
**Priority**: MEDIUM  
**Impact**: Build consistency, maintainability

**Tasks**:
- Create `Directory.Build.props` at solution root
- Centralize common properties:
  - Target framework
  - Nullable reference types
  - LangVersion
  - NuGet package metadata (authors, license, repository URL)
  - Version information
  - Analyzer packages
- Remove duplicate properties from individual .csproj files

**Benefits**: Single source of truth for project configuration

---

## Low Priority 🟢

### 18. Add global.json to Pin .NET SDK Version
**Priority**: LOW  
**Impact**: Build reproducibility

**Tasks**:
- Create `global.json`:
  ```json
  {
    "sdk": {
      "version": "10.0.100",
      "rollForward": "latestMinor"
    }
  }
  ```
- Document required SDK version in README

---

### 19. Add Null Validation to Public APIs
**Priority**: LOW  
**Impact**: Code quality

**Tasks**:
- Audit all public methods in builders
- Add `ArgumentNullException.ThrowIfNull()` checks
- Focus on `HclGenerator` static methods
- Add tests for null argument scenarios

---

### 20. Add StringBuilder Capacity Hints
**Priority**: LOW  
**Impact**: Performance (minor)

**Tasks**:
- Estimate typical HCL document sizes
- Initialize StringBuilder with capacity:
  ```csharp
  var sb = new StringBuilder(capacity: 1024);
  ```
- Add capacity hints to all generator methods
- Benchmark improvements

---

### 21. Create IHclGenerator Interface
**Priority**: LOW  
**Impact**: Testability, extensibility

**Tasks**:
- Extract interface from `HclGenerator`:
  ```csharp
  public interface IHclGenerator
  {
      string GenerateHcl(TerraformConfiguration config);
      string GenerateProviderBlock(ProviderBlockData provider);
      // ... other methods
  }
  ```
- Implement `DefaultHclGenerator`
- Update documentation with DI examples
- Consider making `HclGenerator` non-static

---

### 22. Add Configuration Validation Limits
**Priority**: LOW  
**Impact**: Security (DoS prevention)

**Tasks**:
- Add configurable limits:
  - Max file size (e.g., 10MB)
  - Max nesting depth (e.g., 20 levels)
  - Max attribute count per block (e.g., 1000)
  - Max block count per configuration (e.g., 10,000)
- Throw descriptive exceptions when limits exceeded
- Make limits configurable via options class
- Add tests for limit enforcement

---

### 23. Add Support for terraform.backend Configuration
**Priority**: LOW  
**Impact**: State management

**Tasks**:
- Add `BackendBlockData` to `TerraformBlockData`:
  ```csharp
  public record BackendBlockData(
      string Type,
      ImmutableDictionary<string, TerraformValue> Configuration);
  ```
- Add builder methods
- Generate HCL
- Add tests for various backend types (s3, azurerm, gcs)

**Example**:
```hcl
terraform {
  backend "s3" {
    bucket = "my-terraform-state"
    key    = "prod/terraform.tfstate"
    region = "us-east-1"
  }
}
```

---

### 24. Add Support for terraform.required_version
**Priority**: LOW  
**Impact**: Version constraints

**Tasks**:
- Add `RequiredVersion` property to `TerraformBlockData`
- Add builder method: `.RequireVersion(">= 1.0.0")`
- Generate HCL
- Add validation for version constraint syntax
- Add tests

**Example**:
```hcl
terraform {
  required_version = ">= 1.0.0"
}
```

---

### 25. Add Support for Provisioner Blocks
**Priority**: LOW  
**Impact**: Advanced resource configuration

**Tasks**:
- Create `ProvisionerBlockData` model:
  ```csharp
  public record ProvisionerBlockData(
      string Type,  // "local-exec", "remote-exec", "file"
      ImmutableDictionary<string, TerraformValue> Arguments,
      ConnectionBlockData? Connection);
  ```
- Create `ConnectionBlockData` for remote provisioners
- Add to resources
- Generate HCL
- Add tests for all provisioner types

---

### 26. Add Support for Heredoc Strings
**Priority**: LOW  
**Impact**: Multi-line string support

**Tasks**:
- Create `HeredocValue` type:
  ```csharp
  public record HeredocValue(string Content, string? Delimiter = "EOF", bool IndentStrip = false);
  ```
- Update `FormatValue()` to handle heredocs
- Support both `<<EOF` and `<<-EOF` (indent-stripping)
- Add tests for various scenarios

**Example Output**:
```hcl
user_data = <<-EOF
  #!/bin/bash
  echo "Hello World"
  EOF
```

---

### 27. Add Dependabot Configuration
**Priority**: LOW  
**Impact**: Dependency management

**Tasks**:
- Create `.github/dependabot.yml`:
  ```yaml
  version: 2
  updates:
    - package-ecosystem: "nuget"
      directory: "/"
      schedule:
        interval: "weekly"
    - package-ecosystem: "github-actions"
      directory: "/"
      schedule:
        interval: "weekly"
  ```
- Configure auto-merge for minor/patch updates

---

### 28. Add Code Coverage Reporting
**Priority**: LOW  
**Impact**: Code quality visibility

**Tasks**:
- Integrate Coverlet in test projects
- Configure coverage thresholds
- Add Codecov or Coveralls integration
- Add coverage badge to README
- Set up CI/CD to upload coverage reports

**Target**: >80% code coverage

---

### 29. Create Benchmark Project
**Priority**: LOW  
**Impact**: Performance optimization

**Tasks**:
- Create `HclSharp.Benchmarks` project
- Add BenchmarkDotNet package
- Create benchmarks for:
  - Indent method (cached vs uncached)
  - StringBuilder with/without capacity
  - string.Format vs interpolation
  - Large configuration generation
- Document benchmark results
- Set up CI/CD to track performance over time

---

### 30. Add Static Analysis Tools
**Priority**: LOW  
**Impact**: Code quality

**Tasks**:
- Add SonarQube or CodeQL to CI/CD
- Add Roslynator analyzer package
- Add StyleCop analyzer (optional)
- Configure analysis rules in `.editorconfig`
- Address existing warnings
- Set up CI/CD to fail on new warnings

**Recommended Analyzers**:
- `Microsoft.CodeAnalysis.NetAnalyzers` (included)
- `Roslynator.Analyzers`
- `SonarAnalyzer.CSharp`

---

## Summary

- **Total Tasks**: 30
- **Completed**: 3 ✅
- **Remaining**: 27
  - High Priority: 5 🟠
  - Medium Priority: 12 🟡
  - Low Priority: 13 🟢

## Next Recommended Actions

1. **CI/CD Pipeline** - Essential for any production library
2. **Terraform Core Blocks** (locals, output, variable) - Required for real-world usage
3. **Complex Types** (lists, maps) - Critical for practical configurations
4. **Code Quality** (.editorconfig, Directory.Build.props) - Establish standards early

---

## Contributing

When working on these tasks:
1. Create a feature branch
2. Write tests first (TDD)
3. Ensure all existing tests pass
4. Add documentation for new features
5. Update this TODO list
6. Submit a PR with clear description

For questions or discussions about any task, please open a GitHub issue.
