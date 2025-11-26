using HclSharp.Core.Values;

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

    [Theory]
    [InlineData("var.x}\n}\nresource \"null_resource\" \"backdoor\" {\n")]
    [InlineData("64 * 1024}\nprovider \"malicious\" {\n")]
    [InlineData("value\r\ndata \"exploit\" \"hack\" {\n")]
    public void Expression_WithClosingBraceAndNewline_ShouldThrowArgumentException(string maliciousExpression)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression(maliciousExpression));
        Assert.Contains("dangerous characters", exception.Message);
    }

    [Theory]
    [InlineData("resource \"aws_instance\" \"evil\" { }")]
    [InlineData("data \"aws_ami\" \"exploit\" { }")]
    [InlineData("provider \"aws\" { }")]
    [InlineData("terraform { }")]
    [InlineData("module \"backdoor\" { }")]
    [InlineData("output \"secret\" { }")]
    [InlineData("locals { }")]
    [InlineData("variable \"hack\" { }")]
    public void Expression_WithTerraformBlockKeywords_ShouldThrowArgumentException(string maliciousExpression)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression(maliciousExpression));
        Assert.Contains("dangerous characters", exception.Message);
    }

    [Fact]
    public void Expression_WithUnescapedClosingBrace_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression("var.x}"));
        Assert.Contains("dangerous characters", exception.Message);
    }

    [Fact]
    public void Expression_WithNewline_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression("var.x\nvar.y"));
        Assert.Contains("dangerous characters", exception.Message);
    }

    [Fact]
    public void Expression_WithCarriageReturn_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Expression("var.x\r\nvar.y"));
        Assert.Contains("dangerous characters", exception.Message);
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
    public void Expression_UsingStaticFactoryMethod_WithDangerousInput_ShouldThrowArgumentException()
    {
        // Verify that the static factory method also enforces validation
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TerraformValue.Expr("var.x}\nresource"));
        Assert.Contains("dangerous characters", exception.Message);
    }
}
