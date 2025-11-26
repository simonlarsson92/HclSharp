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
    public static void WriteToFile(this TerraformDocumentBuilder builder, string path)
    {
        var hcl = builder.ToHcl();
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
}
