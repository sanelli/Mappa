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
public sealed class VariableDeclarationSyntaxAssertions
: ObjectAssertions<VariableDeclarationSyntax, VariableDeclarationSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertion.</param>
    public VariableDeclarationSyntaxAssertions(VariableDeclarationSyntax value)
        : base(value)
    {
    }
}