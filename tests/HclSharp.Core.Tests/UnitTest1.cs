using HclSharp.Core.Values;

namespace HclSharp.Core.Tests;

public class ValueFormattingTests
{
    [Fact]
    public void FormatValue_StringLiteral_ShouldQuoteAndEscape()
    {
        // Arrange
        var value = new LiteralValue("hello world");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("\"hello world\"", result);
    }
    
    [Fact]
    public void FormatValue_StringWithEscapes_ShouldEscapeSpecialCharacters()
    {
        // Arrange
        var value = new LiteralValue("line1\nline2\ttab\"quote\"");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("\"line1\\nline2\\ttab\\\"quote\\\"\"", result);
    }
    
    [Fact]
    public void FormatValue_Integer_ShouldReturnNumberWithoutQuotes()
    {
        // Arrange
        var value = new LiteralValue(42);
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("42", result);
    }
    
    [Fact]
    public void FormatValue_Double_ShouldUseInvariantCulture()
    {
        // Arrange
        var value = new LiteralValue(3.14159);
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert - Should use dot, not comma regardless of system culture
        Assert.Contains(".", result);
        Assert.DoesNotContain(",", result);
        // Verify the actual numeric value is preserved
        Assert.True(double.TryParse(result, System.Globalization.NumberStyles.Float, 
            System.Globalization.CultureInfo.InvariantCulture, out var parsed));
        Assert.Equal(3.14159, parsed, precision: 5);
    }
    
    [Fact]
    public void FormatValue_Boolean_ShouldBeLowercase()
    {
        // Arrange
        var trueValue = new LiteralValue(true);
        var falseValue = new LiteralValue(false);
        
        // Act
        var trueResult = HclGenerator.FormatValue(trueValue);
        var falseResult = HclGenerator.FormatValue(falseValue);
        
        // Assert
        Assert.Equal("true", trueResult);
        Assert.Equal("false", falseResult);
    }
    
    [Fact]
    public void FormatValue_VariableReference_ShouldNotQuote()
    {
        // Arrange
        var value = new VariableReference("vm_name");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("var.vm_name", result);
    }
    
    [Fact]
    public void FormatValue_DataReference_ShouldFormatCorrectly()
    {
        // Arrange
        var value = new DataReference("vsphere_datacenter", "dc", "id");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("data.vsphere_datacenter.dc.id", result);
    }
    
    [Fact]
    public void FormatValue_ResourceReference_ShouldFormatCorrectly()
    {
        // Arrange
        var value = new ResourceReference("aws_instance", "web", "id");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("aws_instance.web.id", result);
    }
    
    [Fact]
    public void FormatValue_Expression_ShouldReturnAsIs()
    {
        // Arrange
        var value = new Expression("64 * 1024");
        
        // Act
        var result = HclGenerator.FormatValue(value);
        
        // Assert
        Assert.Equal("64 * 1024", result);
    }
    
    [Fact]
    public void EscapeString_BackslashAndQuote_ShouldEscape()
    {
        // Arrange
        var input = "path\\to\\file\"quoted\"";
        
        // Act
        var result = HclGenerator.EscapeString(input);
        
        // Assert
        Assert.Equal("path\\\\to\\\\file\\\"quoted\\\"", result);
    }
}
