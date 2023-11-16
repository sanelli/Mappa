// <copyright file="MethodDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="MethodDeclarationSyntax"/>.
/// </summary>
[DebuggerNonUserCode]
public sealed class MethodDeclarationSyntaxAssertions
    : ObjectAssertions<MethodDeclarationSyntax, MethodDeclarationSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    internal MethodDeclarationSyntaxAssertions(
        MethodDeclarationSyntax value,
        SemanticModel semanticModel,
        Compilation compilation)
        : base(value)
    {
        this.SemanticModel = semanticModel;
        this.Compilation = compilation;
    }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    public Compilation Compilation { get; }

    /// <summary>
    /// Assert that the class have all the expected modifiers.
    /// </summary>
    /// <param name="assert">The attribute syntax assertion.</param>
    /// <returns>The assertions.</returns>
    public MethodDeclarationSyntaxAssertions HaveGeneratedCodeAttribute(Action<AttributeSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var generatedCodeAttributes = attributes.Where(attributeSyntax =>
                attributeSyntax.Name.ToString().Equals(typeof(GeneratedCodeAttribute).FullName, StringComparison.Ordinal))
            .ToArray();
        generatedCodeAttributes.Should().HaveCount(1);
        var generatedCodeAttribute = generatedCodeAttributes.Single();
        assert(new AttributeSyntaxAssertions(generatedCodeAttribute));

        return this;
    }

    /// <summary>
    /// Assert that the method have all the expected modifiers.
    /// </summary>
    /// <param name="modifiers">The expected modifier.</param>
    /// <returns>The assertions.</returns>
    public MethodDeclarationSyntaxAssertions HaveModifiers(params SyntaxKind[] modifiers)
    {
        var expectedModifiers = new HashSet<SyntaxKind>(modifiers);
        this.Subject.Modifiers.Should().HaveCount(expectedModifiers.Count);
        this.Subject.Modifiers.Should()
            .Contain(syntaxToken => expectedModifiers.Contains(syntaxToken.Kind()));

        return this;
    }

    /// <summary>
    /// Assert that the method has a body with specific characteristics.
    /// </summary>
    /// <param name="assert">The assertions on the method's body.</param>
    /// <returns>The method syntax assertion.</returns>
    public MethodDeclarationSyntaxAssertions HaveBody(Action<BlockSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var blockSyntaxes = this.Subject.ChildNodes().OfType<BlockSyntax>().ToArray();
        blockSyntaxes.Should().HaveCount(1);
        var blockSyntaxAssertions = new BlockSyntaxAssertions(blockSyntaxes.Single(), this.SemanticModel, this.Compilation);
        assert(blockSyntaxAssertions);
        return this;
    }
}