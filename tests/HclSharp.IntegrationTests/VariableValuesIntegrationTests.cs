using System;
using System.Collections.Generic;
using System.Text;
using HclSharp.Core;
using HclSharp.Core.Values;
using HclSharp.Shell.Builders;
using HclSharp.Shell.IO;
using System.IO;

namespace HclSharp.IntegrationTests;

/// <summary>
/// Integration tests for variable values functionality, testing the complete
/// build-to-file flow, async operations, and file handling scenarios.
/// </summary>
public class VariableValuesIntegrationTests
{
    private readonly string _testDirectory;

    public VariableValuesIntegrationTests()
    {
        // Create a unique test directory for each test run
        _testDirectory = Path.Combine(Path.GetTempPath(), "HclSharp.Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
    }

    
}
