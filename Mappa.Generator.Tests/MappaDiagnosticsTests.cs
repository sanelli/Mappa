// <copyright file="MappaDiagnosticsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDiagnostics"/> factory methods.
/// </summary>
public sealed class MappaDiagnosticsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaDiagnostics.DuplicatedMapping"/> uses <c>unknown</c> when the first parameter has no type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DuplicatedMappingUsesUnknownWhenParameterTypeIsMissing()
    {
        var methodDeclaration = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.Identifier("DuplicateMap"))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("input")))))
            .WithReturnType(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));

        var diagnostic = MappaDiagnostics.DuplicatedMapping(methodDeclaration);

        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Method 'DuplicateMap' cannot be generated because mapping from 'unknown' to 'int' already exists in the current class.");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.ParseExactDoesNotAcceptOnlyFormat"/> accepts a null method declaration.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParseExactDoesNotAcceptOnlyFormatAcceptsNullMethodDeclaration()
    {
        var diagnostic = MappaDiagnostics.ParseExactDoesNotAcceptOnlyFormat(null, "System.DateTime");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Format will be ignored because method System.DateTime.ParseExact(string, string) does not exist; consider defining a culture via MappaSettings.");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod"/> accepts a null syntax node.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DependencyDoesNotProvideAnyViableMethodAcceptsNullSyntaxNode()
    {
        var diagnostic = MappaDiagnostics.DependencyDoesNotProvideAnyViableMethod(null, "MyDependency");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Dependency 'MyDependency' does not provide any method that could be used for mapping.");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.CannotIdentifyStrategy"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CannotIdentifyStrategyAcceptsNullLocation()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class SourceType { }

                              public class TargetType { }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceType");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetType");
        if (sourceType is null || targetType is null)
        {
            throw new InvalidOperationException("Expected source and target types to be present in the compilation.");
        }

        var diagnostic = MappaDiagnostics.CannotIdentifyStrategy(targetType, sourceType, null);

        diagnostic.Location.Should().Be(Location.None);
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty"/> accepts a null method declaration.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TooManyUsePropertyAttributesForTheSameTargetPropertyAcceptsNullMethodDeclaration()
    {
        var diagnostic = MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(null, "Map", "Property");

        diagnostic.Location.Should().Be(Location.None);
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.NotAllSourceEnumMembersCanBeMapped"/> accepts a null method declaration.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NotAllSourceEnumMembersCanBeMappedAcceptsNullMethodDeclaration()
    {
        var diagnostic = MappaDiagnostics.NotAllSourceEnumMembersCanBeMapped(
            null,
            "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
            "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
            "'One'");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Not all members of source enum 'Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum' can be mapped to target enum 'Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum' by name: 'One'. Unmapped source values throw ArgumentOutOfRangeException at runtime.");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.EnumMemberMissingDescription"/> accepts a null method declaration.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumMemberMissingDescriptionAcceptsNullMethodDeclaration()
    {
        var diagnostic = MappaDiagnostics.EnumMemberMissingDescription(
            null,
            "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
            "'One'");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Enum 'Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum' has members without a non-empty Description attribute required for Description mapping: 'One'.");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.AmbiguousEnumMap"/> accepts a null method declaration.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AmbiguousEnumMapAcceptsNullMethodDeclaration()
    {
        var diagnostic = MappaDiagnostics.AmbiguousEnumMap(
            null,
            "Target enum member 'one' is matched by multiple source members: 'ONe', 'One'.");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Enum mapping is ambiguous: Target enum member 'one' is matched by multiple source members: 'ONe', 'One'..");
    }
}