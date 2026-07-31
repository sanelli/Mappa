// <copyright file="NamespaceDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="NamespaceDeclarationSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
internal sealed class NamespaceDeclarationSyntaxAssertions
    : BaseNamespaceDeclarationSyntaxAssertions<NamespaceDeclarationSyntax, NamespaceDeclarationSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamespaceDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    internal NamespaceDeclarationSyntaxAssertions(
        NamespaceDeclarationSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
            : base(value, semanticModel, compilation)
    {
    }
}