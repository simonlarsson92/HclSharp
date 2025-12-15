using HclSharp.Core;
using HclSharp.Core.Model;
using HclSharp.Core.Values;
using System.Collections.Immutable;
using Xunit;

namespace HclSharp.Core.Tests
{
    /// <summary>
    /// Unit tests for variable values functionality (tfvars generation).
    /// Tests only core components - no shell dependencies.
    /// </summary>
    public class VariableValuesTests
    {
        [Fact]
        public void VariableValues_EmptyConfiguration_ShouldGenerateEmptyHcl()
        {
            // Arrange
            var config = VariablesValuesConfiguration.Empty;

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void VariableValues_SingleStringValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("vm_name", new LiteralValue("my-test-vm"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("vm_name = \"my-test-vm\"", result);
        }

        [Fact]
        public void VariableValues_SingleNumberValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("instance_count", new LiteralValue(5));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("instance_count = 5", result);
        }

        [Fact]
        public void VariableValues_SingleBooleanValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("enable_monitoring", new LiteralValue(true));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("enable_monitoring = true", result);
        }

        [Fact]
        public void VariableValues_MultipleValuesDifferentTypes_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("vm_name", new LiteralValue("production-vm"))
                .Add("num_cpus", new LiteralValue(8))
                .Add("memory_mb", new LiteralValue(16384))
                .Add("enable_backup", new LiteralValue(true))
                .Add("region", new LiteralValue("us-west-2"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("vm_name = \"production-vm\"", result);
            Assert.Contains("num_cpus = 8", result);
            Assert.Contains("memory_mb = 16384", result);
            Assert.Contains("enable_backup = true", result);
            Assert.Contains("region = \"us-west-2\"", result);
        }

        [Fact]
        public void VariableValues_BooleanFalse_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("enable_monitoring", new LiteralValue(false));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("enable_monitoring = false", result);
        }

        [Fact]
        public void VariableValues_DoubleValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("threshold", new LiteralValue(0.75));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("threshold = 0.75", result);
        }

        [Fact]
        public void VariableValues_LongValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("max_size", new LiteralValue(9223372036854775807L));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("max_size = 9223372036854775807", result);
        }

        [Fact]
        public void VariableValues_FloatValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("ratio", new LiteralValue(3.14f));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("ratio = 3.14", result);
        }

        [Fact]
        public void VariableValues_DecimalValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("price", new LiteralValue(99.99m));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("price = 99.99", result);
        }

        [Fact]
        public void VariableValues_StringWithSpecialCharacters_ShouldEscapeCorrectly()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("description", new LiteralValue("Line 1\nLine 2\tTabbed"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("description = \"Line 1\\nLine 2\\tTabbed\"", result);
        }

        [Fact]
        public void VariableValues_StringWithQuotes_ShouldEscapeCorrectly()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("message", new LiteralValue("He said \"hello\""));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("message = \"He said \\\"hello\\\"\"", result);
        }

        [Fact]
        public void VariableValues_StringWithBackslashes_ShouldEscapeCorrectly()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("path", new LiteralValue("C:\\Users\\Admin"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("path = \"C:\\\\Users\\\\Admin\"", result);
        }

        [Fact]
        public void VariableValues_VariableReference_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("backup_region", new VariableReference("primary_region"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("backup_region = var.primary_region", result);
        }

        [Fact]
        public void VariableValues_Expression_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("total_memory", new Expression("8 * 1024"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("total_memory = 8 * 1024", result);
        }

        [Fact]
        public void VariableValues_DataReference_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("datacenter_id", new DataReference("vsphere_datacenter", "dc", "id"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("datacenter_id = data.vsphere_datacenter.dc.id", result);
        }

        [Fact]
        public void VariableValues_ResourceReference_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("vm_id", new ResourceReference("vsphere_virtual_machine", "vm", "id"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("vm_id = vsphere_virtual_machine.vm.id", result);
        }

        [Fact]
        public void VariableValues_ZeroValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("min_instances", new LiteralValue(0));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("min_instances = 0", result);
        }

        [Fact]
        public void VariableValues_NegativeNumber_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("offset", new LiteralValue(-5));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("offset = -5", result);
        }

        [Fact]
        public void VariableValues_EmptyString_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("empty_value", new LiteralValue(""));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("empty_value = \"\"", result);
        }

        [Fact]
        public void VariableValues_NullValue_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("nullable_field", new LiteralValue(null));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("nullable_field = null", result);
        }

        [Fact]
        public void VariableValues_MultipleLines_ShouldHaveProperFormatting()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("first", new LiteralValue("value1"))
                .Add("second", new LiteralValue("value2"))
                .Add("third", new LiteralValue("value3"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);
            var lines = result.Split('\n');

            // Assert
            Assert.Equal(3, lines.Length);
            Assert.All(lines, line => Assert.Contains(" = ", line));
        }

        [Fact]
        public void VariableValues_ImplicitConversionFromString_ShouldWork()
        {
            // Arrange
            TerraformValue value = "implicit-string";
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("name", value);
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("name = \"implicit-string\"", result);
        }

        [Fact]
        public void VariableValues_ImplicitConversionFromInt_ShouldWork()
        {
            // Arrange
            TerraformValue value = 42;
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("count", value);
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("count = 42", result);
        }

        [Fact]
        public void VariableValues_ImplicitConversionFromBool_ShouldWork()
        {
            // Arrange
            TerraformValue value = true;
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("enabled", value);
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("enabled = true", result);
        }

        [Fact]
        public void VariableValues_ImplicitConversionFromDouble_ShouldWork()
        {
            // Arrange
            TerraformValue value = 1.5;
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("ratio", value);
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Equal("ratio = 1.5", result);
        }

        [Fact]
        public void VariableValues_ComplexConfiguration_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("vsphere_user", new LiteralValue("administrator@vsphere.local"))
                .Add("vsphere_password", new LiteralValue("MySecretPassword!"))
                .Add("vsphere_server", new LiteralValue("vcenter.lab.local"))
                .Add("datacenter", new LiteralValue("Lab_DC"))
                .Add("cluster", new LiteralValue("Cluster1"))
                .Add("datastore", new LiteralValue("NVMe-DS1"))
                .Add("network", new LiteralValue("VM Network"))
                .Add("template", new LiteralValue("Windows2022_Template"))
                .Add("vm_name", new LiteralValue("win2022-testvm"))
                .Add("num_cpus", new LiteralValue(4))
                .Add("memory_mb", new LiteralValue(16384))
                .Add("disk_gb", new LiteralValue(60));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("vsphere_user = \"administrator@vsphere.local\"", result);
            Assert.Contains("vsphere_password = \"MySecretPassword!\"", result);
            Assert.Contains("vsphere_server = \"vcenter.lab.local\"", result);
            Assert.Contains("datacenter = \"Lab_DC\"", result);
            Assert.Contains("num_cpus = 4", result);
            Assert.Contains("memory_mb = 16384", result);
            Assert.Contains("disk_gb = 60", result);
        }

        [Fact]
        public void VariableValuesConfiguration_EmptyConstant_ShouldHaveEmptyValues()
        {
            // Arrange & Act
            var config = VariablesValuesConfiguration.Empty;

            // Assert
            Assert.NotNull(config.Values);
            Assert.Empty(config.Values);
        }

        [Fact]
        public void VariableValuesConfiguration_Constructor_ShouldStoreValues()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("test", new LiteralValue("value"));

            // Act
            var config = new VariablesValuesConfiguration(values);

            // Assert
            Assert.Same(values, config.Values);
        }

        [Fact]
        public void VariableValues_VariableNamesWithUnderscores_ShouldGenerateCorrectHcl()
        {
            // Arrange
            var values = ImmutableDictionary<string, TerraformValue>.Empty
                .Add("simple_name", new LiteralValue("value1"))
                .Add("name_with_numbers123", new LiteralValue("value2"))
                .Add("name_", new LiteralValue("value3"));
            var config = new VariablesValuesConfiguration(values);

            // Act
            var result = HclGenerator.GenerateVariableValuesHcl(config);

            // Assert
            Assert.Contains("simple_name = \"value1\"", result);
            Assert.Contains("name_with_numbers123 = \"value2\"", result);
            Assert.Contains("name_ = \"value3\"", result);
        }
    }
}
