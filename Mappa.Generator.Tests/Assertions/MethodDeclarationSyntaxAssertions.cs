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
internal sealed class MethodDeclarationSyntaxAssertions
    : ObjectAssertions<MethodDeclarationSyntax, MethodDeclarationSyntaxAssertions>
{
    private readonly SemanticModel semanticModel;
    private readonly Compilation compilation;

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
        : base(value, FluentAssertions.Execution.AssertionChain.GetOrCreate())
    {
        this.semanticModel = semanticModel;
        this.compilation = compilation;
    }

    /// <summary>
    /// Check that the method has a <see cref="GeneratedCodeAttribute"/>.
    /// </summary>
    /// <param name="assert">The attribute syntax assertion.</param>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveGeneratedCodeAttribute(Action<AttributeSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var generatedCodeAttributes = attributes.Where(attributeSyntax =>
                attributeSyntax.Name.ToString().Equals($"global::{typeof(GeneratedCodeAttribute).FullName}", StringComparison.Ordinal))
            .ToArray();
        generatedCodeAttributes.Should().HaveCount(1);
        var generatedCodeAttribute = generatedCodeAttributes.Single();
        assert(new AttributeSyntaxAssertions(generatedCodeAttribute));

        return this;
    }

    /// <summary>
    /// Check that the method has a <see cref="DebuggerNonUserCodeAttribute"/>.
    /// </summary>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveDebuggerNonUserCodeAttribute()
    {
        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var generatedCodeAttributes = attributes.Where(attributeSyntax =>
                attributeSyntax.Name.ToString().Equals($"global::{typeof(DebuggerNonUserCodeAttribute).FullName}", StringComparison.Ordinal))
            .ToArray();
        generatedCodeAttributes.Should().HaveCount(1);

        return this;
    }

    /// <summary>
    /// Assert that the method have all the expected modifiers.
    /// </summary>
    /// <param name="modifiers">The expected modifier.</param>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveModifiers(params SyntaxKind[] modifiers)
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
    internal MethodDeclarationSyntaxAssertions HaveBody(Action<BlockSyntaxAssertions> assert)
    {
        ArgumentNullException.ThrowIfNull(assert);

        var blockSyntaxes = this.Subject.ChildNodes().OfType<BlockSyntax>().ToArray();
        blockSyntaxes.Should().HaveCount(1);
        var blockSyntaxAssertions = new BlockSyntaxAssertions(blockSyntaxes.Single(), this.semanticModel, this.compilation);
        assert(blockSyntaxAssertions);
        return this;
    }

    /// <summary>
    /// Check that the method has a nullability annotation.
    /// </summary>
    /// <param name="nullableSetup">Define the required nullability.</param>
    /// <returns>The method syntax assertion.</returns>
    internal MethodDeclarationSyntaxAssertions HaveNullabilityAnnotation(NullableSetup nullableSetup)
    {
        var annotations = this.Subject.GetLeadingTrivia().Where(
            trivia => trivia.Kind() is SyntaxKind.NullableDirectiveTrivia
            && trivia.IsDirective
            && trivia.ToString().Equals($"#nullable {(nullableSetup is NullableSetup.Enable ? "enable" : "disable")}", StringComparison.Ordinal))
            .ToArray();

        annotations.Should().HaveCount(1);
        return this;
    }

    /// <summary>
    /// Check that the method has a nullability annotation.
    /// </summary>
    /// <param name="pragmaWarning">The type of <c>#pragma warning</c>.</param>
    /// <returns>The method syntax assertion.</returns>
    internal MethodDeclarationSyntaxAssertions HavePragmaWarningDisableAnnotation(PragmaWarning pragmaWarning)
    {
        var annotations = this.Subject.GetLeadingTrivia().Where(
                trivia => trivia.Kind() is SyntaxKind.PragmaWarningDirectiveTrivia
                          && trivia.IsDirective
                          && trivia.ToString().Equals("#pragma warning disable", StringComparison.Ordinal))
            .ToArray();

        annotations.Should().HaveCount(pragmaWarning is PragmaWarning.Disable ? 1 : 0);
        return this;
    }
}