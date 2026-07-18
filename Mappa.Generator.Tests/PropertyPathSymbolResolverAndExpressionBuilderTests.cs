// <copyright file="PropertyPathSymbolResolverAndExpressionBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

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
    /// Test <see cref="PropertyPathSymbolResolver.TryGetReceiverTypeForPathPrefix"/> success and failure arms.
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
    /// Test <see cref="PropertyPathExpressionBuilder.BuildChainedAccessExpression"/> empty prefix, resolve failures, and value-type access.
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
}