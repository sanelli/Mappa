// <copyright file="PropertyPathSymbolResolverAndExpressionBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyPathSymbolResolver"/> and <see cref="PropertyPathExpressionBuilder"/>.
/// </summary>
public sealed class PropertyPathSymbolResolverAndExpressionBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    private const string ModelsSource = """
                                        namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                        public class Address
                                        {
                                            public string City { get; set; }
                                            public string ZipCode { get; set; }
                                        }

                                        public class Location
                                        {
                                            public Address Address { get; set; }
                                        }

                                        public class Source
                                        {
                                            public Location Location { get; set; }
                                        }

                                        public class Target
                                        {
                                            public Address Address { get; set; }
                                            public string City { get; set; }
                                        }

                                        public partial class Mapper
                                        {
                                            public partial Target Map(Source input);
                                        }
                                        """;

    /// <summary>
    /// Test <see cref="PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(ITypeSymbol, string, string, out ITypeSymbol)"/> success and failure arms.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetReceiverTypeForPathPrefixHandlesPrefixVariants()
    {
        var compilation = BuildCompilation(ModelsSource);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var locationType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Location")!;

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(sourceType, "input", "input", out var sameAsRoot)
            .Should().BeTrue();
        sameAsRoot.Should().Be(sourceType);

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(sourceType, "input", string.Empty, out var emptyPrefix)
            .Should().BeTrue();
        emptyPrefix.Should().Be(sourceType);

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(sourceType, "input", "input.Location", out var locationReceiver)
            .Should().BeTrue();
        locationReceiver.ToDisplayString().Should().Be(locationType.ToDisplayString());

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(sourceType, "input", "other.Location", out _)
            .Should().BeFalse();

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(sourceType, "input", "input.Missing", out _)
            .Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="PropertyPathSymbolResolver.TryResolveTargetMemberPath"/> empty, single-segment, nested, and missing paths.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryResolveTargetMemberPathHandlesEmptySingleNestedAndMissingSegments()
    {
        var compilation = BuildCompilation(ModelsSource);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;

        PropertyPathSymbolResolver.TryResolveTargetMemberPath(targetType, PropertyPath.Parse(string.Empty), out var emptyExpr, out var emptyMissing)
            .Should().BeFalse();
        emptyExpr.Should().BeEmpty();
        emptyMissing.Should().BeEmpty();

        PropertyPathSymbolResolver.TryResolveTargetMemberPath(targetType, PropertyPath.Parse("City"), out var singleExpr, out var singleMissing)
            .Should().BeTrue();
        singleExpr.Should().Be("City");
        singleMissing.Should().BeNull();

        PropertyPathSymbolResolver.TryResolveTargetMemberPath(targetType, PropertyPath.Parse("Address.City"), out var nestedExpr, out var nestedMissing)
            .Should().BeTrue();
        nestedExpr.Should().Be("Address.City");
        nestedMissing.Should().BeNull();

        PropertyPathSymbolResolver.TryResolveTargetMemberPath(targetType, PropertyPath.Parse("Address.Missing"), out var missingExpr, out var missingSegment)
            .Should().BeFalse();
        missingExpr.Should().BeEmpty();
        missingSegment.Should().Be("Missing");
    }

    /// <summary>
    /// Test <see cref="PropertyPathExpressionBuilder.BuildChainedAccessExpression(string, string, string[], ITypeSymbol, bool, ITypeSymbol, out IPropertySymbol[], string?)"/> empty prefix, resolve failures, and value-type access.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildChainedAccessExpressionHandlesEmptyPrefixResolveFailuresAndValueTypes()
    {
        var compilation = BuildCompilation("""
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public class Outer
                                           {
                                               public int Code { get; set; }
                                               public string Name { get; set; }
                                           }

                                           public class Source
                                           {
                                               public Outer Outer { get; set; }
                                           }
                                           """);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var emptyPrefix = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            string.Empty,
            ["Outer", "Name"],
            sourceType,
            nullableEnabled: true,
            stringType,
            out var resolvedWithEmptyPrefix);
        resolvedWithEmptyPrefix.Should().HaveCount(2);
        emptyPrefix.Should().Contain("input.Outer");

        var badPrefix = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "other.Outer",
            ["Name"],
            sourceType,
            nullableEnabled: true,
            stringType,
            out var resolvedBadPrefix);
        resolvedBadPrefix.Should().BeEmpty();
        badPrefix.Should().Be("input");

        var missingSegment = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Outer", "Missing"],
            sourceType,
            nullableEnabled: true,
            stringType,
            out var resolvedMissing);
        resolvedMissing.Should().BeEmpty();
        missingSegment.Should().Be("input");

        var valueTypeAccess = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Outer", "Code"],
            sourceType,
            nullableEnabled: true,
            intType,
            out var resolvedValueType);
        resolvedValueType.Should().HaveCount(2);
        valueTypeAccess.Should().Contain(".Code");
        valueTypeAccess.Should().NotContain("?.Code");
        valueTypeAccess.Should().NotContain("?? throw");
    }

    /// <summary>
    /// Test <see cref="PropertyPathExpressionBuilder.BuildChainedAccessExpression(string, string, string[], ITypeSymbol, bool, ITypeSymbol, out IPropertySymbol[], string?)"/> appends
    /// <c>?? throw</c> only when conditional access makes the expression nullable-capable.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildChainedAccessExpressionAppendsNullForgivingThrowOnlyWhenExpressionCanBeNull()
    {
        var compilation = BuildCompilation("""
                                           #nullable enable
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public class Address
                                           {
                                               public string City { get; set; } = null!;
                                           }

                                           public class Location
                                           {
                                               public Address? Address { get; set; }
                                           }

                                           public class Source
                                           {
                                               public Location Location { get; set; } = null!;
                                           }
                                           """);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")!;
        var cityProperty = addressType.GetMembers("City").OfType<IPropertySymbol>().Single();
        var locationType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Location")!;

        var nullableIntermediate = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Location", "Address", "City"],
            sourceType,
            nullableEnabled: true,
            cityProperty.Type,
            out _);
        nullableIntermediate.Should().Contain("Location.Address?.City");
        nullableIntermediate.Should().Contain("?? throw");

        var nonNullableChain = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Location"],
            sourceType,
            nullableEnabled: true,
            locationType,
            out _);
        nonNullableChain.Should().Be("input.Location");
        nonNullableChain.Should().NotContain("?? throw");
    }

    /// <summary>
    /// Test <see cref="PropertyPathExpressionBuilder.BuildChainedAccessExpression(string, string, string[], ITypeSymbol, bool, ITypeSymbol, out IPropertySymbol[], string?)"/> treats
    /// <see cref="Nullable{T}"/> leaves as nullable-capable without prior conditional access.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildChainedAccessExpressionTreatsNullableValueTypeLeafAsNullableCapable()
    {
        var compilation = BuildCompilation("""
                                           #nullable enable
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public class Outer
                                           {
                                               public int? Code { get; set; }
                                           }

                                           public class Source
                                           {
                                               public Outer Outer { get; set; } = null!;
                                           }
                                           """);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var nullableValueLeaf = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Outer", "Code"],
            sourceType,
            nullableEnabled: true,
            intType,
            out var resolved);
        resolved.Should().HaveCount(2);
        nullableValueLeaf.Should().Contain("input.Outer.Code");
        nullableValueLeaf.Should().NotContain("?.Code");
        nullableValueLeaf.Should().Contain("?? throw");
    }

    /// <summary>
    /// Test <see cref="PropertyPathExpressionBuilder.BuildTargetMemberAccessExpression"/> for flat and nested paths.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildTargetMemberAccessExpressionHandlesFlatAndNestedPaths()
    {
        PropertyPathExpressionBuilder.BuildTargetMemberAccessExpression("result", "City")
            .Should().Be("result.City");
        PropertyPathExpressionBuilder.BuildTargetMemberAccessExpression("result", "Address.City")
            .Should().Be("result.Address.City");
    }

    /// <summary>
    /// Test <see cref="PropertyPathSymbolResolver.TryResolvePropertyPath"/> success and missing-segment failure.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryResolvePropertyPathHandlesSuccessAndMissingSegment()
    {
        var compilation = BuildCompilation(ModelsSource);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        sourceType.Should().NotBeNull();

        PropertyPathSymbolResolver.TryResolvePropertyPath(
                sourceType,
                PropertyPath.Parse("Location.Address.City"),
                out var resolved,
                out var missing)
            .Should().BeTrue();
        resolved.Should().HaveCount(3);
        missing.Should().BeNull();

        PropertyPathSymbolResolver.TryResolvePropertyPath(
                sourceType,
                PropertyPath.Parse("Location.Missing"),
                out var missingResolved,
                out var missingSegment)
            .Should().BeFalse();
        missingResolved.Should().BeEmpty();
        missingSegment.Should().Be("Missing");
    }

    /// <summary>
    /// Test <see cref="PropertyPathSymbolResolver.TryResolveMemberPath"/> and prefix resolution when fields are included.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryResolveMemberPathAndPrefixHandleFieldsWhenEnabled()
    {
        var compilation = BuildCompilation("""
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public class Address
                                           {
                                               public string City;
                                           }

                                           public class Source
                                           {
                                               public Address Location;
                                           }
                                           """);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address");
        sourceType.Should().NotBeNull();
        addressType.Should().NotBeNull();

        PropertyPathSymbolResolver.TryResolveMemberPath(
                sourceType,
                PropertyPath.Parse("Location.City"),
                includeFields: false,
                out var withoutFields,
                out var missingWithoutFields)
            .Should().BeFalse();
        withoutFields.Should().BeEmpty();
        missingWithoutFields.Should().Be("Location");

        PropertyPathSymbolResolver.TryResolveMemberPath(
                sourceType,
                PropertyPath.Parse("Location.City"),
                includeFields: true,
                out var withFields,
                out var missingWithFields)
            .Should().BeTrue();
        withFields.Should().HaveCount(2);
        withFields[0].IsField.Should().BeTrue();
        withFields[1].IsField.Should().BeTrue();
        missingWithFields.Should().BeNull();

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                sourceType,
                "input",
                "input.Location",
                includeFields: true,
                out var locationReceiver)
            .Should().BeTrue();
        locationReceiver.ToDisplayString().Should().Be(addressType.ToDisplayString());

        PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix(
                sourceType,
                "input",
                "input.Location",
                includeFields: false,
                out _)
            .Should().BeFalse();

        var chained = PropertyPathExpressionBuilder.BuildChainedAccessExpression(
            "input",
            "input",
            ["Location", "City"],
            sourceType,
            nullableEnabled: true,
            compilation.GetSpecialType(SpecialType.System_String),
            includeFields: true,
            out var resolvedMembers);
        resolvedMembers.Should().HaveCount(2);
        chained.Should().Contain("input.Location");
        chained.Should().Contain(".City");
    }
}