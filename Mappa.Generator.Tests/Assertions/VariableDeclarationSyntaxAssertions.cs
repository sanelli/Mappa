// <copyright file="VariableDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions on <see cref="VariableDeclarationSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
internal sealed class VariableDeclarationSyntaxAssertions
: ObjectAssertions<VariableDeclarationSyntax, VariableDeclarationSyntaxAssertions>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertion.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    public VariableDeclarationSyntaxAssertions(VariableDeclarationSyntax value, SemanticModel semanticModel, Compilation compilation)
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Assert the declaration syntax is for a single variable initialized by a constant.
    /// </summary>
    /// <param name="type">The type of the variable.</param>
    /// <param name="identifier">The variable identifier.</param>
    /// <param name="value">The constant value.</param>
    /// <returns>The assertions.</returns>
    public VariableDeclarationSyntaxAssertions BeAssignmentFromConstant(string type, string identifier, object value)
    {
        var expectedType = this.compilation.GetTypeSymbol(type);
        var actualSymbol = this.compilation.GetTypeSymbol(this.Subject.Type.ToString());

        SymbolEqualityComparer
            .Default
            .Equals(actualSymbol, expectedType)
            .Should().BeTrue();

        this.Subject.Variables.Should().HaveCount(1);
        this.Subject.Variables[0].Identifier.Text.Should().Be(identifier);
        this.Subject.Variables[0].Initializer.Should().NotBeNull();

        var valueExpression = this.Subject.Variables[0].Initializer!.Value;
        new ExpressionSyntaxAssertions(valueExpression, this.semanticModel, this.compilation).BeLiteralExpressionSyntax(value);
        return this;
    }
}