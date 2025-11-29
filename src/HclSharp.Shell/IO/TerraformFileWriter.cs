using System.Text;
using HclSharp.Shell.Builders;

namespace HclSharp.Shell.IO;

/// <summary>
/// Extension methods for writing Terraform configurations to files.
/// </summary>
public static class TerraformFileWriter
{
    /// <summary>
    /// Writes the Terraform configuration to a file.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    public static void WriteToFile(
        this TerraformDocumentBuilder builder,
        string path)
    {
        var hcl = builder.ToHcl();
        File.WriteAllText(path, hcl, Encoding.UTF8);
    }

    /// <summary>
    /// Writes the Variables values to a file.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    public static void WriteToFile(
        this VariablesValuesBuilder builder,
        string path)
    {
        var hcl = builder.ToHcl();
        File.WriteAllText(path, hcl, Encoding.UTF8);
    }

    /// <summary>
    /// Writes the Variables configuratuib to a file.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static void WriteVariablesToFile(
        this TerraformDocumentBuilder builder,
        string path)
    {
        var hcl = builder.ToVariablesHcl();
        File.WriteAllText(path, hcl, Encoding.UTF8);
    }

    /// <summary>
    /// Writes the Terraform configuration to a file asynchronously.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task WriteToFileAsync(
        this TerraformDocumentBuilder builder,
        string path,
        CancellationToken cancellationToken = default)
    {
        var hcl = builder.ToHcl();
        await File.WriteAllTextAsync(path, hcl, Encoding.UTF8, cancellationToken);
    }

    /// <summary>
    /// Writes the Variables values to a file asynchronously.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task WriteToFileAsync(
        this VariablesValuesBuilder builder,
        string path,
        CancellationToken cancellationToken = default)
    {
        var hcl = builder.ToHcl();
        await File.WriteAllTextAsync(path, hcl, Encoding.UTF8, cancellationToken);
    }

    /// <summary>
    /// Writes the Variables configuratuib to a file asynchronously.
    /// </summary>
    /// <param name="builder">The document builder</param>
    /// <param name="path">File path to write to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task WriteVariablesToFileAsync(
        this TerraformDocumentBuilder builder,
        string path,
        CancellationToken cancellationToken = default)
    { 
        var hcl = builder.ToVariablesHcl();
        await File.WriteAllTextAsync(path, hcl, Encoding.UTF8, cancellationToken);
    }
}
