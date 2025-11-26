using System.Globalization;
using System.Text;
using HclSharp.Core.Model;
using HclSharp.Core.Values;

namespace HclSharp.Core;

/// <summary>
/// Pure functions for generating HCL from immutable Terraform configuration data.
/// </summary>
public static class HclGenerator
{
    /// <summary>
    /// The string used for a single level of indentation (2 spaces as per HCL convention).
    /// </summary>
    private const string IndentUnit = "  ";
    
    /// <summary>
    /// Returns indentation string for the specified level.
    /// </summary>
    /// <param name="level">Indentation level (0 = no indent, 1 = 2 spaces, 2 = 4 spaces, etc.)</param>
    private static string Indent(int level) => string.Concat(Enumerable.Repeat(IndentUnit, level));
    /// <summary>
    /// Generates complete HCL document from a TerraformConfiguration.
    /// </summary>
    public static string GenerateHcl(TerraformConfiguration config)
    {
        var sb = new StringBuilder();
        // TODO: Implement
        return sb.ToString();
    }

    /// <summary>
    /// Generates HCL for a terraform block.
    /// </summary>
    public static string GenerateTerraformBlock(TerraformBlockData block)
    {
        if (block.RequiredProviders.Count == 0)
        {
            return "terraform {\n}";
        }

        var sb = new StringBuilder();
        sb.AppendLine("terraform {");
        sb.AppendLine(string.Format("{0}required_providers {{", Indent(1)));
        
        foreach (var provider in block.RequiredProviders)
        {
            sb.AppendLine(string.Format("{0}{1} = {{", Indent(2), provider.Name));
            sb.AppendLine(string.Format("{0}source  = \"{1}\"", Indent(3), provider.Source));
            
            if (!string.IsNullOrEmpty(provider.Version))
            {
                sb.AppendLine(string.Format("{0}version = \"{1}\"", Indent(3), provider.Version));
            }
            
            sb.AppendLine(string.Format("{0}}}", Indent(2)));
        }
        
        sb.AppendLine(string.Format("{0}}}", Indent(1)));
        sb.Append("}");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates HCL for a provider block.
    /// </summary>
    public static string GenerateProviderBlock(ProviderBlockData provider)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Format("provider \"{0}\" {{", provider.Name));
        
        foreach (var attr in provider.Attributes)
        {
            sb.AppendLine(string.Format("{0}{1} = {2}", Indent(1), attr.Key, FormatValue(attr.Value)));
        }
        
        sb.Append("}");
        return sb.ToString();
    }

    /// <summary>
    /// Generates HCL for a data source block.
    /// </summary>
    public static string GenerateDataSourceBlock(DataSourceBlockData dataSource)
    {
        // TODO: Implement
        return string.Empty;
    }

    /// <summary>
    /// Generates HCL for a resource block.
    /// </summary>
    public static string GenerateResourceBlock(ResourceBlockData resource)
    {
        // TODO: Implement
        return string.Empty;
    }

    /// <summary>
    /// Generates HCL for a nested block with indentation.
    /// </summary>
    public static string GenerateNestedBlock(NestedBlockData nested, int indentLevel)
    {
        // TODO: Implement
        return string.Empty;
    }

    /// <summary>
    /// Formats a TerraformValue to its HCL representation.
    /// </summary>
    public static string FormatValue(TerraformValue value)
    {
        return value switch
        {
            LiteralValue lit => FormatLiteral(lit.Value),
            VariableReference varRef => $"var.{varRef.VariableName}",
            DataReference dataRef => $"data.{dataRef.DataSourceType}.{dataRef.DataSourceName}.{dataRef.AttributePath}",
            ResourceReference resRef => $"{resRef.ResourceType}.{resRef.ResourceName}.{resRef.AttributePath}",
            Expression expr => expr.ExpressionString,
            _ => throw new ArgumentException($"Unknown value type: {value.GetType()}")
        };
    }

    /// <summary>
    /// Formats a literal value (string, number, boolean).
    /// </summary>
    private static string FormatLiteral(object value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{EscapeString(s)}\"",
            bool b => b.ToString().ToLowerInvariant(),
            
            // Integer types
            byte or sbyte or short or ushort or int or uint or long or ulong => 
                Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture),
            
            // Floating point types - use invariant culture to ensure dots, not commas
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            decimal dec => dec.ToString("G", CultureInfo.InvariantCulture),
            
            _ => throw new ArgumentException(
                $"Unsupported literal type: {value.GetType().Name}. " +
                $"Supported types: string, bool, numeric types (int, long, double, etc.)")
        };
    }

    /// <summary>
    /// Escapes special characters in strings.
    /// </summary>
    public static string EscapeString(string str)
    {
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
