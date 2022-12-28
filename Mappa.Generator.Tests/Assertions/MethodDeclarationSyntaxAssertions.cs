// <copyright file="MethodDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions for <see cref="MethodDeclarationSyntax"/>.
/// </summary>
public sealed class MethodDeclarationSyntaxAssertions
    : ObjectAssertions<MethodDeclarationSyntax, MethodDeclarationSyntaxAssertions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodDeclarationSyntaxAssertions"/> class.
    /// </summary>
    /// <param name="value">The target of the assertions.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation unit.</param>
    public MethodDeclarationSyntaxAssertions(
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
    private SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Assert that the class have all the expected modifiers.
    /// </summary>
    /// <returns>The assertions.</returns>
    public MethodDeclarationSyntaxAssertions HaveGeneratedCodeAttribute()
    {
        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var generatedCodeAttributes = attributes.Where(x =>
                x.Name.ToString().Equals(typeof(GeneratedCodeAttribute).FullName, StringComparison.Ordinal))
            .ToArray();
        generatedCodeAttributes.Should().HaveCount(1);
        var generatedCodeAttribute = generatedCodeAttributes.Single();
        generatedCodeAttribute.Should().BeGeneratedCodeAttribute();

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
    /// Assert that the method has a body.
    /// </summary>
    /// <returns>The block syntax assertion.</returns>
    public BlockSyntaxAssertions HaveBody()
    {
        var blockSyntaxes = this.Subject.ChildNodes().OfType<BlockSyntax>().ToArray();
        blockSyntaxes.Should().HaveCount(1);
        return new BlockSyntaxAssertions(blockSyntaxes.Single(), this.SemanticModel, this.Compilation);
    }
}