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

    /// <summary>
    /// Test <see cref="MappaDiagnostics.AmbiguousInvokeMethodResolution"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AmbiguousInvokeMethodResolutionAcceptsNullLocation()
    {
        var diagnostic = MappaDiagnostics.AmbiguousInvokeMethodResolution(
            null,
            "multiple methods named 'InvokeMe' in 'Mapper' match.");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.GetMessage(CultureInfo.CurrentCulture)
            .Should()
            .Be("Invoke method resolution is ambiguous: multiple methods named 'InvokeMe' in 'Mapper' match..");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.ProjectionMappingNotSupported"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProjectionMappingNotSupportedAcceptsNullLocation()
    {
        var diagnostic = MappaDiagnostics.ProjectionMappingNotSupported(null, "ProjectToDto", "ContainerMapStrategy");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.Descriptor.Should().Be(MappaDiagnosticDescriptors.ProjectionMappingNotSupported);
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.ProjectionInvokeMethodNotInlinable"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProjectionInvokeMethodNotInlinableAcceptsNullLocation()
    {
        var diagnostic = MappaDiagnostics.ProjectionInvokeMethodNotInlinable(null, "ProjectToDto", "Transform");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.Descriptor.Should().Be(MappaDiagnosticDescriptors.ProjectionInvokeMethodNotInlinable);
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.ProjectionNestedQueryableNotSupported"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProjectionNestedQueryableNotSupportedAcceptsNullLocation()
    {
        var diagnostic = MappaDiagnostics.ProjectionNestedQueryableNotSupported(null, "ProjectToDto", "Items");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.Descriptor.Should().Be(MappaDiagnosticDescriptors.ProjectionNestedQueryableNotSupported);
    }

    /// <summary>
    /// Test <see cref="MappaDiagnostics.ProjectionEnumStrategyNotSupported"/> accepts a null location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProjectionEnumStrategyNotSupportedAcceptsNullLocation()
    {
        var diagnostic = MappaDiagnostics.ProjectionEnumStrategyNotSupported(null, "ProjectToDto");

        diagnostic.Location.Should().Be(Location.None);
        diagnostic.Descriptor.Should().Be(MappaDiagnosticDescriptors.ProjectionEnumStrategyNotSupported);
    }

    /// <summary>
    /// Test remaining nullable-location factory methods accept null and use <see cref="Location.None"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RemainingNullableLocationFactoriesAcceptNullAndUseLocationNone()
    {
        var (typeSymbol, propertySymbol) = GetSampleTypeAndProperty();

        AssertLocationNone(MappaDiagnostics.PropertySetterIsNotAccessible(null, typeSymbol, propertySymbol));
        AssertLocationNone(MappaDiagnostics.CannotMapNonRequiredProperty(null, typeSymbol, propertySymbol));
        AssertLocationNone(MappaDiagnostics.InvalidMappaSettingsStyleValue(null, "DateTimeStyle", 999, "DateTimeStyles"));
        AssertLocationNone(MappaDiagnostics.EnumMapAttributeEnumTypeMismatch(
            null,
            "Map",
            "MappaMapEnumMember",
            "WrongEnum",
            "SourceEnum",
            "TargetEnum"));
        AssertLocationNone(MappaDiagnostics.EnumMapMemberMappingClash(null, "Map", "SourceEnum", "clash details"));
        AssertLocationNone(MappaDiagnostics.EnumMapIgnoreConflictsWithMemberMapping(null, "Map", "SourceEnum", "One"));
        AssertLocationNone(MappaDiagnostics.EnumMapDefaultBehaviorRequiresDefaultValue(null, "Map", "SourceEnum"));
        AssertLocationNone(MappaDiagnostics.EnumMapDefaultValueConstructorMismatch(null, "Map", "SourceEnum", "string"));
        AssertLocationNone(MappaDiagnostics.EnumMapDefaultAttributeUnusedDefaultValue(null, "Map", "SourceEnum"));
        AssertLocationNone(MappaDiagnostics.TooManyEnumMapDefaultAttributesOnDirectEnumMap(null, "Map", 2));
        AssertLocationNone(MappaDiagnostics.DuplicateEnumMapDefaultAttribute(null, "Map", "SourceEnum"));
        AssertLocationNone(MappaDiagnostics.ProjectionMethodHasBeforeOrAfterMapHooks(null, "ProjectToDto"));
        AssertLocationNone(MappaDiagnostics.ProjectionMethodHasMappaContextParameter(null, "ProjectToDto"));
        AssertLocationNone(MappaDiagnostics.ProjectionMethodHasObjectFactory(null, "ProjectToDto"));
        AssertLocationNone(MappaDiagnostics.ProjectionMethodHasAllowInaccessibleMembers(null, "ProjectToDto"));
        AssertLocationNone(MappaDiagnostics.MustMapTargetPropertyWasNotMapped(null, typeSymbol, propertySymbol));
        AssertLocationNone(MappaDiagnostics.UnsafeAccessorNotSupported(null, "Map"));
        AssertLocationNone(MappaDiagnostics.AllowInaccessibleTargetMembersDisabledAll(null, "Map"));
        AssertLocationNone(MappaDiagnostics.MappaDependencyInjectionClassIsNotPartial(null, "Registrar"));
        AssertLocationNone(MappaDiagnostics.MappaAndMappaDependencyInjectionBothApplied(null, "Mapper"));
        AssertLocationNone(MappaDiagnostics.MappaDependencyInjectionMapperHasNoEligibleInterfaces(null, "Mapper"));
        AssertLocationNone(MappaDiagnostics.MappaDependencyInjectionStaticMapperSkipped(null, "StaticMapper"));
        AssertLocationNone(MappaDiagnostics.MethodToInvokeUndefined(null));
        AssertLocationNone(MappaDiagnostics.ReferenceHandlingNestedMapWithoutMappaContext(null, "MapNested"));
        AssertLocationNone(MappaDiagnostics.MaxCompileTimeDepthReached(null, typeSymbol, typeSymbol, 3));
        AssertLocationNone(MappaDiagnostics.MappingCycleDetected(null, typeSymbol, typeSymbol));
        AssertLocationNone(MappaDiagnostics.MappingCycleAutoBroken(null, typeSymbol, typeSymbol, "Map__Source__To__Target"));
        AssertLocationNone(MappaDiagnostics.ObjectFactoryMethodNotFound(null, "Map", "Target", "CreateTarget"));
        AssertLocationNone(MappaDiagnostics.DuplicateObjectFactoryForTargetType(null, "Map", "Target"));
        AssertLocationNone(MappaDiagnostics.IQueryableMappedAsCollection(null, "Map"));
    }

    private static void AssertLocationNone(Diagnostic diagnostic)
    {
        diagnostic.Location.Should().Be(Location.None);
    }

    private static (INamedTypeSymbol TypeSymbol, IPropertySymbol PropertySymbol) GetSampleTypeAndProperty()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class SampleType
                              {
                                  public int Value { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var typeSymbol = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SampleType");
        if (typeSymbol is null)
        {
            throw new InvalidOperationException("Expected SampleType to be present in the compilation.");
        }

        var propertySymbol = typeSymbol.GetMembers("Value").OfType<IPropertySymbol>().Single();
        return (typeSymbol, propertySymbol);
    }
}