using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class ConvenienceMethodTests
{
    [Fact]
    public void Variable_ShouldCreateVariableReference()
    {
        // Act
        var value = TerraformValue.Variable("test_var");
        
        // Assert
        Assert.IsType<VariableReference>(value);
        Assert.Equal("test_var", ((VariableReference)value).VariableName);
        Assert.Equal("var.test_var", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void Literal_ShouldCreateLiteralValue()
    {
        // Act
        var value = TerraformValue.Literal("test");
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal("test", ((LiteralValue)value).Value);
        Assert.Equal("\"test\"", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void DataRef_ShouldCreateDataReference()
    {
        // Act
        var value = TerraformValue.DataRef("vsphere_datacenter", "dc", "id");
        
        // Assert
        Assert.IsType<DataReference>(value);
        var dataRef = (DataReference)value;
        Assert.Equal("vsphere_datacenter", dataRef.DataSourceType);
        Assert.Equal("dc", dataRef.DataSourceName);
        Assert.Equal("id", dataRef.AttributePath);
        Assert.Equal("data.vsphere_datacenter.dc.id", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ResourceRef_ShouldCreateResourceReference()
    {
        // Act
        var value = TerraformValue.ResourceRef("vsphere_virtual_machine", "vm", "id");
        
        // Assert
        Assert.IsType<ResourceReference>(value);
        var resRef = (ResourceReference)value;
        Assert.Equal("vsphere_virtual_machine", resRef.ResourceType);
        Assert.Equal("vm", resRef.ResourceName);
        Assert.Equal("id", resRef.AttributePath);
        Assert.Equal("vsphere_virtual_machine.vm.id", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void Expr_ShouldCreateExpression()
    {
        // Act
        var value = TerraformValue.Expr("64 * 1024");
        
        // Assert
        Assert.IsType<Expression>(value);
        Assert.Equal("64 * 1024", ((Expression)value).ExpressionString);
        Assert.Equal("64 * 1024", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ImplicitConversion_String_ShouldCreateLiteralValue()
    {
        // Act
        TerraformValue value = "test string";
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal("test string", ((LiteralValue)value).Value);
        Assert.Equal("\"test string\"", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ImplicitConversion_Int_ShouldCreateLiteralValue()
    {
        // Act
        TerraformValue value = 42;
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal(42, ((LiteralValue)value).Value);
        Assert.Equal("42", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ImplicitConversion_Long_ShouldCreateLiteralValue()
    {
        // Act
        TerraformValue value = 9876543210L;
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal(9876543210L, ((LiteralValue)value).Value);
        Assert.Equal("9876543210", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ImplicitConversion_Bool_ShouldCreateLiteralValue()
    {
        // Act
        TerraformValue value = true;
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal(true, ((LiteralValue)value).Value);
        Assert.Equal("true", HclGenerator.FormatValue(value));
    }
    
    [Fact]
    public void ImplicitConversion_Double_ShouldCreateLiteralValue()
    {
        // Act
        TerraformValue value = 3.14;
        
        // Assert
        Assert.IsType<LiteralValue>(value);
        Assert.Equal(3.14, ((LiteralValue)value).Value);
        Assert.Equal("3.14", HclGenerator.FormatValue(value));
    }
}
