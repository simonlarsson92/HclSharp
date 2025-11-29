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

            // vSphere Credentials
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
                .WithDescription("Hostname or IP of the vSphere server")
                .Build()

            // Required Environment Variables
            .AddVariable("datacenter")
                .WithType("string")
                .WithDescription("Name of the vSphere datacenter")
                .Build()

            .AddVariable("cluster")
                .WithType("string")
                .WithDescription("Compute cluster name")
                .Build()

            .AddVariable("datastore")
                .WithType("string")
                .WithDescription("Datastore name to store the VM")
                .Build()

            .AddVariable("network")
                .WithType("string")
                .WithDescription("Port group / network name")
                .Build()

            .AddVariable("template")
                .WithType("string")
                .WithDescription("Name of the template to clone from")
                .Build()

            .AddVariable("vm_name")
                .WithType("string")
                .WithDescription("Name of the virtual machine")
                .Build()

            // Optional parameters with defaults
            .AddVariable("num_cpus")
                .WithType("number")
                .WithDefault(4)
                .WithDescription("How many vCPUs the VM should have")
                .Build()

            .AddVariable("memory_mb")
                .WithType("number")
                .WithDefault(8192)
                .WithDescription("Memory size in MB (e.g., 8192 = 8GB)")
                .Build()

            .AddVariable("disk_gb")
                .WithType("number")
                .WithDefault(40)
                .WithDescription("Primary disk size in GB")
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

            // Define datastore data source
            .AddDataSource("vsphere_datastore", "datastore")
                .AddAttribute("name", TerraformValue.Variable("datastore"))
                .AddAttribute("datacenter_id", TerraformValue.Expr("data.vsphere_datacenter.dc.id"))
                .Build()

            // Define compute cluster data source
            .AddDataSource("vsphere_compute_cluster", "cluster")
                .AddAttribute("name", TerraformValue.Variable("cluster"))
                .AddAttribute("datacenter_id", TerraformValue.Expr("data.vsphere_datacenter.dc.id"))
                .Build()

            // Define network data source
            .AddDataSource("vsphere_network", "network")
                .AddAttribute("name", TerraformValue.Variable("network"))
                .AddAttribute("datacenter_id", TerraformValue.Expr("data.vsphere_datacenter.dc.id"))
                .Build()

            // Define virtual machine template data source
            .AddDataSource("vsphere_virtual_machine", "template")
                .AddAttribute("name", TerraformValue.Variable("template"))
                .AddAttribute("datacenter_id", TerraformValue.Expr("data.vsphere_datacenter.dc.id"))
                .Build()

            // Define virtual machine resource
            .AddResource("vsphere_virtual_machine", "windows_vm")
                .AddAttribute("name", TerraformValue.Variable("vm_name"))
                .AddAttribute("resource_pool_id", TerraformValue.Expr("data.vsphere_compute_cluster.cluster.resource_pool_id"))
                .AddAttribute("datastore_id", TerraformValue.Expr("data.vsphere_datastore.datastore.id"))
                .AddAttribute("num_cpus", TerraformValue.Variable("num_cpus"))
                .AddAttribute("memory", TerraformValue.Variable("memory_mb"))
                .AddAttribute("guest_id", TerraformValue.Expr("data.vsphere_virtual_machine.template.guest_id"))
                .AddAttribute("scsi_type", TerraformValue.Expr("data.vsphere_virtual_machine.template.scsi_type"))
                .AddNestedBlock("network_interface")
                    .AddAttribute("network_id", TerraformValue.Expr("data.vsphere_network.network.id"))
                    .AddAttribute("adapter_type", "vmxnet3")
                    .EndNestedBlock()
                .AddNestedBlock("disk")
                    .AddAttribute("label", "disk0")
                    .AddAttribute("size", TerraformValue.Variable("disk_gb"))
                    .AddAttribute("thin_provisioned", true)
                    .EndNestedBlock()
                .AddNestedBlock("clone")
                    .AddAttribute("template_uuid", TerraformValue.Expr("data.vsphere_virtual_machine.template.id"))
                    .EndNestedBlock()
                .Build();

        var variableValues = new VariablesValuesBuilder()
            .AddVariableValue("vsphere_user", "administrator@vsphere.local")
            .AddVariableValue("vsphere_password", "MySecretPassword!")
            .AddVariableValue("vsphere_server", "vcenter.lab.local")
            .AddVariableValue("datacenter", "Lab_DC")
            .AddVariableValue("cluster", "Cluster1")
            .AddVariableValue("datastore", "NVMe-DS1")
            .AddVariableValue("network", "VM Network")
            .AddVariableValue("template", "Windows2022_Template")
            .AddVariableValue("vm_name", "win2022-testvm")
            .AddVariableValue("num_cpus", 4)
            .AddVariableValue("memory_mb", 16384)
            .AddVariableValue("disk_gb", 60);

        var variableValuesPath = Path.Combine(Directory.GetCurrentDirectory(), "terraform.tfvars");
        variableValues.WriteToFile(variableValuesPath);

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

        Console.WriteLine($"Main configuration written to: {outputPath}");
        Console.WriteLine($"Variables written to: {variablePath}");
        Console.WriteLine($"Variable values written to: {variableValuesPath}");
        Console.WriteLine();
        Console.WriteLine("Success! You can now use this Terraform configuration with:");
        Console.WriteLine("  terraform init");
        Console.WriteLine("  terraform plan");
        Console.WriteLine("  terraform apply");
    }
}
