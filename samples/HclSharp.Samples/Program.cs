using HclSharp.Core;
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;
using HclSharp.Shell.IO;

namespace HclSharp.Samples;

/// <summary>
/// Sample program demonstrating HCL generation for a vSphere virtual machine.
/// Generates the Terraform configuration from the original requirements.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("HclSharp Sample: Generating vSphere VM Terraform Configuration");
        Console.WriteLine("================================================================");
        Console.WriteLine();

        // Build the Terraform configuration using the fluent builder API
        var config = new TerraformDocumentBuilder()
            // Define required providers
            .AddRequiredProvider("vsphere", "hashicorp/vsphere", ">= 2.5.0")

            // Define input variables
            .AddVariable("vsphere_user")
                .WithType("string")
                .WithDescription("vSphere username")
                .Sensitive(true)
                .Build()

            .AddVariable("vsphere_password")
                .WithType("string")
                .WithDescription("vSphere password")
                .Sensitive(true)
                .Build()

            .AddVariable("vsphere_server")
                .WithType("string")
                .WithDescription("vSphere server address")
                .Sensitive(false)
                .Build()

            .AddVariable("datacenter")
                .WithType("string")
                .WithDescription("Name of the vSphere datacenter")
                .Sensitive(false)
                .Build()

            .AddVariable("vm_name")
                .WithType("string")
                .WithDescription("Name of the virtual machine to create")
                .Sensitive(false)
                .Build()

            // Configure vsphere provider
            .AddProvider("vsphere")
                .AddAttribute("user", TerraformValue.Variable("vsphere_user"))
                .AddAttribute("password", TerraformValue.Variable("vsphere_password"))
                .AddAttribute("vsphere_server", TerraformValue.Variable("vsphere_server"))
                .AddAttribute("allow_unverified_ssl", true)
                .Build()

            // Define datacenter data source
            .AddDataSource("vsphere_datacenter", "dc")
                .AddAttribute("name", TerraformValue.Variable("datacenter"))
                .Build()

            // Define virtual machine resource
            .AddResource("vsphere_virtual_machine", "windows_vm")
                .AddAttribute("name", TerraformValue.Variable("vm_name"))
                .AddAttribute("num_cpus", 8)
                .AddAttribute("memory", TerraformValue.Expr("64 * 1024"))
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk0")
                    .AddAttribute("size", 40)
                    .EndNestedBlock()
                .Build();


        var variablePath = Path.Combine(Directory.GetCurrentDirectory(), "variables.tf");
        config.WriteVariablesToFile(variablePath);
  
        // Generate HCL
        var hcl = HclGenerator.GenerateHcl(config.Build(), excludeVariables: true);
        
        // Display generated HCL
        Console.WriteLine("Generated HCL:");
        Console.WriteLine("==============");
        Console.WriteLine();
        Console.WriteLine(hcl);
        Console.WriteLine();
        
        // Write to file using extension method
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "main.tf");
        File.WriteAllText(outputPath, hcl);

        

        Console.WriteLine($"Written to: {outputPath}");
        Console.WriteLine();
        Console.WriteLine("Success! You can now use this Terraform configuration with:");
        Console.WriteLine("  terraform init");
        Console.WriteLine("  terraform plan");
        Console.WriteLine("  terraform apply");
    }
}
