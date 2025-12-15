using HclSharp.Core.Model;
using HclSharp.Core.Values;
using System.Collections.Immutable;

namespace HclSharp.Core.Tests
{
    public class VariableBlockTests
    {
        [Fact]
        public void VariableBlock_SimpleTypeOnly_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string"
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.DoesNotContain("default =", result);
            Assert.DoesNotContain("description =", result);
            Assert.DoesNotContain("sensitive =", result);
            Assert.DoesNotContain("validation {", result);
        }

        [Fact]
        public void VariableBlock_WithDefaultValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Default = new LiteralValue("t3.micro")
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.Contains("default = \"t3.micro\"", result);
        }

        [Fact]
        public void VariableBlock_WithDescription_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Description = "EC2 instance type"
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.Contains("description = \"EC2 instance type\"", result);
        }

        [Fact]
        public void VariableBlock_SensitiveTrue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "password",
                Type = "string",
                Sensitive = true
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"password\" {", result);
            Assert.Contains("type = string", result);
            Assert.Contains("sensitive = true", result);
        }

        [Fact]
        public void VariableBlock_SensitiveFalse_ShouldNotIncludeSensitive()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Sensitive = false
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.DoesNotContain("sensitive =", result);
        }

        [Fact]
        public void VariableBlock_WithSingleValidation_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var validation = new VariableValidationData
            {
                Condition = new("length(var.password) > 8"),
                ErrorMessage = "Password must be longer than 8 characters"
            };

            var variable = new VariableBlockData
            {
                Name = "password",
                Type = "string",
                Validations = ImmutableList.Create(validation)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"password\" {", result);
            Assert.Contains("type = string", result);
            Assert.Contains("validation {", result);
            Assert.Contains("condition     = length(var.password) > 8", result);
            Assert.Contains("error_message = \"Password must be longer than 8 characters\"", result);
        }

        [Fact]
        public void VariableBlock_WithMultipleValidations_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var validation1 = new VariableValidationData
            {
                Condition = new("length(var.password) > 8"),
                ErrorMessage = "Password must be longer than 8 characters"
            };

            var validation2 = new VariableValidationData
            {
                Condition = new("can(regex(\"[A-Z]\", var.password))"),
                ErrorMessage = "Password must contain at least one uppercase letter"
            };

            var variable = new VariableBlockData
            {
                Name = "password",
                Type = "string",
                Validations = ImmutableList.Create(validation1, validation2)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"password\" {", result);
            Assert.Contains("type = string", result);
            
            // Check for first validation
            Assert.Contains("validation {", result);
            Assert.Contains("condition     = length(var.password) > 8", result);
            Assert.Contains("error_message = \"Password must be longer than 8 characters\"", result);
            
            // Check for second validation
            Assert.Contains("condition     = can(regex(\"[A-Z]\", var.password))", result);
            Assert.Contains("error_message = \"Password must contain at least one uppercase letter\"", result);
            
            // Should have two validation blocks
            var validationCount = result.Split("validation {").Length - 1;
            Assert.Equal(2, validationCount);
        }

        [Fact]
        public void VariableBlock_ComplexWithAllOptions_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var validation = new VariableValidationData
            {
                Condition = new("contains([\"t3.micro\", \"t3.small\", \"t3.medium\"], var.instance_type)"),
                ErrorMessage = "Instance type must be t3.micro, t3.small, or t3.medium"
            };

            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Default = new LiteralValue("t3.micro"),
                Description = "EC2 instance type",
                Sensitive = false,
                Nullable = true,
                Validations = ImmutableList.Create(validation)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.Contains("default = \"t3.micro\"", result);
            Assert.Contains("description = \"EC2 instance type\"", result);
            Assert.DoesNotContain("sensitive = true", result); // Should not include when false
            Assert.Contains("validation {", result);
            Assert.Contains("condition     = contains([\"t3.micro\", \"t3.small\", \"t3.medium\"], var.instance_type)", result);
            Assert.Contains("error_message = \"Instance type must be t3.micro, t3.small, or t3.medium\"", result);
        }

        [Fact]
        public void VariableBlock_NumberType_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_count",
                Type = "number",
                Default = new LiteralValue(2)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_count\" {", result);
            Assert.Contains("type = number", result);
            Assert.Contains("default = 2", result);
        }

        [Fact]
        public void VariableBlock_BooleanType_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "enable_monitoring",
                Type = "bool",
                Default = new LiteralValue(true)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"enable_monitoring\" {", result);
            Assert.Contains("type = bool", result);
            Assert.Contains("default = true", result);
        }

        [Fact]
        public void VariableBlock_WithExpressionDefault_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "memory_mb",
                Type = "number",
                Default = new Expression("8 * 1024")
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"memory_mb\" {", result);
            Assert.Contains("type = number", result);
            Assert.Contains("default = 8 * 1024", result);
        }

        [Fact]
        public void VariableBlock_ValidateProperIndentation_ShouldBeFormatted()
        {
            // Arrange
            var validation = new VariableValidationData
            {
                Condition = new("var.instance_type != null"),
                ErrorMessage = "Instance type cannot be null"
            };

            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Default = new LiteralValue("t3.micro"),
                Description = "EC2 instance type",
                Sensitive = true,
                Validations = ImmutableList.Create(validation)
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert - Check proper indentation (2 spaces)
            var lines = result.Split('\n');
            
            // Main attributes should be indented with 2 spaces
            Assert.Contains(lines, line => line.StartsWith("  type = "));
            Assert.Contains(lines, line => line.StartsWith("  default = "));
            Assert.Contains(lines, line => line.StartsWith("  description = "));
            Assert.Contains(lines, line => line.StartsWith("  sensitive = "));
            Assert.Contains(lines, line => line.StartsWith("  validation {"));
            
            // Validation contents should be indented with 4 spaces
            Assert.Contains(lines, line => line.StartsWith("    condition     = "));
            Assert.Contains(lines, line => line.StartsWith("    error_message = "));
            
            // Closing braces should have proper indentation
            Assert.Contains(lines, line => line.StartsWith("  }") && line.Trim() == "}"); // validation closing
            Assert.Contains(lines, line => line == "}"); // main closing
        }

        [Fact]
        public void VariableBlock_EmptyValidations_ShouldNotIncludeValidationBlock()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Validations = ImmutableList<VariableValidationData>.Empty
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.DoesNotContain("validation {", result);
        }

        [Fact]
        public void VariableBlock_NullValidations_ShouldNotIncludeValidationBlock()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "instance_type",
                Type = "string",
                Validations = null
            };

            // Act
            var result = HclGenerator.GenerateVariablesBlock(variable);

            // Assert
            Assert.Contains("variable \"instance_type\" {", result);
            Assert.Contains("type = string", result);
            Assert.DoesNotContain("validation {", result);
        }

        [Fact]
        public void VariableBlock_DefaultValues_ShouldSetCorrectDefaults()
        {
            // Arrange
            var variable = new VariableBlockData
            {
                Name = "test_var",
                Type = "string"
                // All other properties should use their default values
            };

            // Assert - Test default values
            Assert.False(variable.Sensitive); // Default should be false
            Assert.True(variable.Nullable);   // Default should be true
            Assert.Null(variable.Default);
            Assert.Null(variable.Description);
            Assert.Null(variable.Validations);
        }
    }
}
