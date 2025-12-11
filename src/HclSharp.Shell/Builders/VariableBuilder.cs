using HclSharp.Core.Model;
using HclSharp.Core.Values;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HclSharp.Shell.Builders;

public class VariableBuilder
{
    private readonly string _name;
    private string _type = string.Empty;
    private readonly TerraformDocumentBuilder _documentBuilder;
    private TerraformValue? _default;
    private string? _description;
    private bool _sensitive;
    private bool _nullable = true;
    private readonly List<VariableValidationData> _validations = new();

    internal VariableBuilder(string name, TerraformDocumentBuilder documentBuilder)
    {
        _name = name;
        _documentBuilder = documentBuilder;
    }

    /// <summary>
    /// Sets the variable type.
    /// </summary>
    public VariableBuilder WithType(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the description for the variable (optional).
    /// </summary>
    public VariableBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the default value for the variable (optional).
    /// </summary>
    public VariableBuilder WithDefault(TerraformValue defaultValue)
    {
        _default = defaultValue;
        return this;
    }

    /// <summary>
    /// Marks the variable as sensitive (optional).
    /// </summary>
    public VariableBuilder Sensitive(bool sensitive = true)
    {
        _sensitive = sensitive;
        return this;
    }

    /// <summary>
    /// Sets the nullable flag for the variable (optional).
    /// </summary>
    public VariableBuilder Nullable(bool nullable = true)
    {
        _nullable = nullable;
        return this;
    }

    /// <summary>
    /// Adds a validation block to the variable (optional, multiple allowed).
    /// </summary>
    public VariableBuilder AddValidation(string condition, string errorMessage)
    {
        _validations.Add(new VariableValidationData
        {
            Condition = condition,
            ErrorMessage = errorMessage, 
        });
        return this;
    }

    /// <summary>
    /// Builds the variable and registers it with the parent document builder.
    /// </summary>
    public TerraformDocumentBuilder Build()
    {
        if (string.IsNullOrEmpty(_type))
        {
            throw new InvalidOperationException($"Variable '{_name}' requires a type. Call WithType() before Build().");
        }

        var variableData = new VariableBlockData
        {
            Name = _name,
            Type = _type,
            Default = _default,
            Description = _description,
            Sensitive = _sensitive,
            Nullable = _nullable,
            Validations = _validations.Count > 0 ? _validations.ToImmutableList() : null,
        };

        _documentBuilder.RegisterVariable(variableData);
        return _documentBuilder;
    }
}
