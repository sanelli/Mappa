// <copyright file="MappaDependencyInjectionFileBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Builders;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDependencyInjectionFileBuilder"/>.
/// </summary>
public sealed class MappaDependencyInjectionFileBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// When the registrar syntax has no modifiers, the builder emits a bare <c>partial</c> class.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceAddsPartialWhenModifiersAreEmpty()
    {
        var (classDeclaration, classSymbol, attributeData) = CreateRegistrarFixture();
        var withoutModifiers = classDeclaration.WithModifiers(SyntaxFactory.TokenList());

        var generated = new MappaDependencyInjectionFileBuilder(
                withoutModifiers,
                classSymbol,
                attributeData,
                ImmutableArray<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)>.Empty)
            .BuildSource();

        var classSignature = generated
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.Trim())
            .Single(line => line.Contains("class Registrar", StringComparison.Ordinal));

        classSignature.Should().Be("partial class Registrar");
    }

    /// <summary>
    /// When the registrar syntax has modifiers but omits <c>partial</c>, the builder appends <c>partial</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceAppendsPartialWhenMissingFromModifiers()
    {
        var (classDeclaration, classSymbol, attributeData) = CreateRegistrarFixture();
        var withoutPartial = classDeclaration.WithModifiers(
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

        var generated = new MappaDependencyInjectionFileBuilder(
                withoutPartial,
                classSymbol,
                attributeData,
                ImmutableArray<(INamedTypeSymbol Mapper, ImmutableArray<INamedTypeSymbol> Interfaces)>.Empty)
            .BuildSource();

        var classSignature = generated
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.Trim())
            .Single(line => line.Contains("class Registrar", StringComparison.Ordinal));

        classSignature.Should().Be("public static partial class Registrar");
    }

    private static (ClassDeclarationSyntax ClassDeclaration, INamedTypeSymbol ClassSymbol, MappaDependencyInjectionAttributeData AttributeData) CreateRegistrarFixture()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaDependencyInjection]
                              public static partial class Registrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classSymbol = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Registrar");
        if (classSymbol is null)
        {
            throw new InvalidOperationException("Registrar type was not found in the compilation.");
        }

        var classDeclaration = compilation.SyntaxTrees
            .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == "Registrar");

        var attributeData = classSymbol.GetAttributes().GetMappaDependencyInjectionAttributeData(compilation);
        if (attributeData is null)
        {
            throw new InvalidOperationException("MappaDependencyInjection attribute data was not found.");
        }

        return (classDeclaration, classSymbol, attributeData);
    }
}