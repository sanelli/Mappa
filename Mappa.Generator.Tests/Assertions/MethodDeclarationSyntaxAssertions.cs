// <copyright file="MethodDeclarationSyntaxAssertions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
        : base(value, AwesomeAssertions.Execution.AssertionChain.GetOrCreate())
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
    /// Check that the method has a <see cref="RequiresDynamicCodeAttribute"/>.
    /// </summary>
    /// <param name="message">The expected attribute message.</param>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveRequiresDynamicCodeAttribute(string message)
    {
        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var requiresDynamicCodeAttributes = attributes.Where(attributeSyntax =>
                attributeSyntax.Name.ToString().Equals($"global::{typeof(RequiresDynamicCodeAttribute).FullName}", StringComparison.Ordinal))
            .ToArray();
        requiresDynamicCodeAttributes.Should().HaveCount(1);
        var requiresDynamicCodeAttribute = requiresDynamicCodeAttributes.Single();
        requiresDynamicCodeAttribute.ArgumentList.Should().NotBeNull();
        requiresDynamicCodeAttribute.ArgumentList!.Arguments.Should().HaveCount(1);
        requiresDynamicCodeAttribute.ArgumentList.Arguments.Single().GetText().ToString().Should().Be(message);

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
    /// Check that the method has a <see cref="System.Runtime.CompilerServices.UnsafeAccessorAttribute"/>.
    /// </summary>
    /// <param name="unsafeAccessorKind">The expected <c>UnsafeAccessorKind</c> name (<c>Method</c> or <c>Constructor</c>).</param>
    /// <param name="runtimeName">
    /// The expected <c>Name</c> attribute argument value, or <see langword="null"/> when
    /// <c>Name</c> must be omitted (constructor accessors).
    /// </param>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveUnsafeAccessorAttribute(string unsafeAccessorKind, string? runtimeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unsafeAccessorKind);

        const string attributeFullName = "global::System.Runtime.CompilerServices.UnsafeAccessor";
        var attributes = this.Subject.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        var unsafeAccessorAttributes = attributes.Where(attributeSyntax =>
                attributeSyntax.Name.ToString().Equals(attributeFullName, StringComparison.Ordinal))
            .ToArray();
        unsafeAccessorAttributes.Should().HaveCount(1);
        var unsafeAccessorAttribute = unsafeAccessorAttributes.Single();
        unsafeAccessorAttribute.ArgumentList.Should().NotBeNull();

        if (runtimeName is null)
        {
            unsafeAccessorAttribute.ArgumentList!.Arguments.Should().HaveCount(1);
            var kindOnlyArgument = unsafeAccessorAttribute.ArgumentList.Arguments[0].Expression.ToString();
            kindOnlyArgument.Should().Be($"global::System.Runtime.CompilerServices.UnsafeAccessorKind.{unsafeAccessorKind}");
            return this;
        }

        unsafeAccessorAttribute.ArgumentList!.Arguments.Should().HaveCount(2);

        var kindArgument = unsafeAccessorAttribute.ArgumentList.Arguments[0].Expression.ToString();
        kindArgument.Should().Be($"global::System.Runtime.CompilerServices.UnsafeAccessorKind.{unsafeAccessorKind}");

        var nameArgument = unsafeAccessorAttribute.ArgumentList.Arguments[1];
        nameArgument.NameEquals.Should().NotBeNull();
        nameArgument.NameEquals!.Name.Identifier.Text.Should().Be("Name");
        nameArgument.Expression.Should().BeOfType<LiteralExpressionSyntax>();
        var nameLiteral = (LiteralExpressionSyntax)nameArgument.Expression;
        nameLiteral.Token.ValueText.Should().Be(runtimeName);

        return this;
    }

    /// <summary>
    /// Assert that the method is declared without a body (for example an <c>extern</c> method).
    /// </summary>
    /// <returns>The assertions.</returns>
    internal MethodDeclarationSyntaxAssertions HaveNoBody()
    {
        this.Subject.Body.Should().BeNull();
        this.Subject.ExpressionBody.Should().BeNull();
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