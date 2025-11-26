namespace HclSharp.Core.Values;

/// <summary>
/// Base abstract record for all Terraform values.
/// Values can be literals, variable references, data references, resource references, or expressions.
/// </summary>
public abstract record TerraformValue
{
    /// <summary>
    /// Creates a variable reference.
    /// </summary>
    public static VariableReference Variable(string name) => new(name);
    
    /// <summary>
    /// Creates a literal value.
    /// </summary>
    public static LiteralValue Literal(object value) => new(value);
    
    /// <summary>
    /// Creates a data source reference.
    /// </summary>
    public static DataReference DataRef(string type, string name, string attributePath) => new(type, name, attributePath);
    
    /// <summary>
    /// Creates a resource reference.
    /// </summary>
    public static ResourceReference ResourceRef(string type, string name, string attributePath) => new(type, name, attributePath);
    
    /// <summary>
    /// Creates an expression.
    /// </summary>
    public static Expression Expr(string expression) => new(expression);
    
    // Implicit conversions for convenience
    public static implicit operator TerraformValue(string value) => new LiteralValue(value);
    public static implicit operator TerraformValue(int value) => new LiteralValue(value);
    public static implicit operator TerraformValue(long value) => new LiteralValue(value);
    public static implicit operator TerraformValue(bool value) => new LiteralValue(value);
    public static implicit operator TerraformValue(double value) => new LiteralValue(value);
}
