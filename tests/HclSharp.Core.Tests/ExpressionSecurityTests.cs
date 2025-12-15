using HclSharp.Core.Values;
using Xunit;

namespace HclSharp.Core.Tests;

/// <summary>
/// Security-focused tests for Expression validation to prevent code injection attacks.
/// </summary>
public class ExpressionSecurityTests
{
    [Fact]
    public void Expression_WithValidExpression_ShouldSucceed()
    {
        // Arrange & Act
        var expr = new Expression("64 * 1024");
        
        // Assert
        Assert.Equal("64 * 1024", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithTerraformFunction_ShouldSucceed()
    {
        // Arrange & Act
        var expr = new Expression("length(var.list)");
        
        // Assert
        Assert.Equal("length(var.list)", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithComplexExpression_ShouldSucceed()
    {
        // Arrange & Act
        var expr = new Expression("var.enabled ? var.value : null");
        
        // Assert
        Assert.Equal("var.enabled ? var.value : null", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithMapAccess_ShouldSucceed()
    {
        // Arrange & Act
        var expr = new Expression("var.config[\"key\"]");
        
        // Assert
        Assert.Equal("var.config[\"key\"]", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithObjectLiteral_ShouldSucceed()
    {
        // Object literals are valid HCL expressions
        // Arrange & Act
        var expr = new Expression("{\n    type = \"t3.micro\"\n    size = 20\n  }");
        
        // Assert
        Assert.Equal("{\n    type = \"t3.micro\"\n    size = 20\n  }", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithClosingBraceAndTextWithoutBlockDeclaration_ShouldSucceed()
    {
        // Closing brace followed by text that's not a block declaration should be allowed
        // Arrange & Act
        var expr = new Expression("var.x}\nresource");
        
        // Assert
        Assert.Equal("var.x}\nresource", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithMultilineSimpleExpression_ShouldSucceed()
    {
        // Multi-line expressions without block keywords should be allowed
        // Arrange & Act
        var expr = new Expression("var.x + var.y");
        
        // Assert
        Assert.Equal("var.x + var.y", expr.ExpressionString);
    }

    [Theory]
    [InlineData("var.x}\nresource \"null_resource\" \"backdoor\" {")]
    [InlineData("64 * 1024}\nprovider \"malicious\" {")]
    [InlineData("value\r\ndata \"exploit\" \"hack\" {")]
    [InlineData("var.x}\n\nresource \"evil\" \"thing\" {")]
    [InlineData("test}\nmodule \"backdoor\" {")]
    [InlineData("}\noutput \"secret\" {")]
    [InlineData("}\nlocals {")]
    [InlineData("}\nvariable \"hack\" {")]
    [InlineData("\nresource \"evil\" \"thing\" {")]
    [InlineData("something\r\nprovider \"bad\" {")]
    public void Expression_WithHclInjectionPattern_ShouldThrowArgumentException(string maliciousExpression)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression(maliciousExpression));
        Assert.Contains("dangerous", exception.Message);
    }

    [Fact]
    public void Expression_WithTerraformBlockAtStart_ShouldThrowArgumentException()
    {
        // Block declarations at the start should be rejected
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression("resource \"aws_instance\" \"evil\" { }"));
        Assert.Contains("dangerous", exception.Message);
    }

    [Fact]
    public void Expression_WithNullString_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Expression(null!));
    }

    [Fact]
    public void Expression_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression(""));
        Assert.Contains("empty or whitespace", exception.Message);
    }

    [Fact]
    public void Expression_WithWhitespaceOnly_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression("   "));
        Assert.Contains("empty or whitespace", exception.Message);
    }

    [Fact]
    public void Expression_WithClosingBracesInString_ShouldSucceed()
    {
        // Closing braces inside quoted strings should be fine
        // Arrange & Act
        var expr = new Expression("format(\"value: %s\", var.x)");
        
        // Assert
        Assert.Equal("format(\"value: %s\", var.x)", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithEscapedClosingBrace_ShouldSucceed()
    {
        // Escaped closing braces should be allowed
        // Arrange & Act
        var expr = new Expression("replace(var.x, \"\\\\}\", \"-\")");
        
        // Assert
        Assert.Equal("replace(var.x, \"\\\\}\", \"-\")", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithNestedFunctions_ShouldSucceed()
    {
        // Arrange & Act
        var expr = new Expression("upper(replace(var.name, \"_\", \"-\"))");
        
        // Assert
        Assert.Equal("upper(replace(var.name, \"_\", \"-\"))", expr.ExpressionString);
    }

    [Fact]
    public void Expression_WithClosingBraceOnly_ShouldSucceed()
    {
        // Just a closing brace without injection pattern should be allowed
        // Arrange & Act
        var expr = new Expression("var.x}");
        
        // Assert
        Assert.Equal("var.x}", expr.ExpressionString);
    }

    [Fact]
    public void Expression_UsingStaticFactoryMethod_WithDangerousInput_ShouldThrowArgumentException()
    {
        // Verify that the static factory method also enforces validation
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TerraformValue.Expr("var.x}\nresource \"evil\" {"));
        Assert.Contains("dangerous", exception.Message);
    }

    [Fact]
    public void Expression_UsingStaticFactoryMethod_WithValidInput_ShouldSucceed()
    {
        // Verify that the static factory method works with valid input
        // Arrange & Act
        var expr = TerraformValue.Expr("var.x}\nresource");
        
        // Assert
        Assert.Equal("var.x}\nresource", ((Expression)expr).ExpressionString);
    }
}
