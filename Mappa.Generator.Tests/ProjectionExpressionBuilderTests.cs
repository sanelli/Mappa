// <copyright file="ProjectionExpressionBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Builders.Expressions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ProjectionExpressionBuilder"/>.
/// </summary>
public sealed class ProjectionExpressionBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test projection expression building for identity, nullable, conversions and enum strategies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionSupportsBuiltInProjectionStrategies()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DateOnly).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
            ]);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var longType = compilation.GetSpecialType(SpecialType.System_Int64);
        var doubleType = compilation.GetSpecialType(SpecialType.System_Double);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var nullableIntType = compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(intType);
        var dateTimeType = compilation.GetTypeByMetadataName(typeof(DateTime).FullName!)
                           ?? throw new InvalidOperationException("DateTime type was not found.");
        var dateTimeOffsetType = compilation.GetTypeByMetadataName(typeof(DateTimeOffset).FullName!)
                                 ?? throw new InvalidOperationException("DateTimeOffset type was not found.");
        var dateOnlyType = compilation.GetTypeByMetadataName(typeof(DateOnly).FullName!)
                           ?? throw new InvalidOperationException("DateOnly type was not found.");
        var timeOnlyType = compilation.GetTypeByMetadataName(typeof(TimeOnly).FullName!)
                           ?? throw new InvalidOperationException("TimeOnly type was not found.");
        var timeSpanType = compilation.GetTypeByMetadataName(typeof(TimeSpan).FullName!)
                           ?? throw new InvalidOperationException("TimeSpan type was not found.");
        var uriType = compilation.GetTypeByMetadataName(typeof(Uri).FullName!)
                      ?? throw new InvalidOperationException("Uri type was not found.");

        var enumSource = """
                         public enum SampleEnum
                         {
                             One = 1,
                             Two = 2,
                         }
                         """;
        var enumCompilation = CSharpCompilation.Create(
            "EnumAssembly",
            [CSharpSyntaxTree.ParseText(enumSource, cancellationToken: TestContext.Current.CancellationToken)],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var enumType = enumCompilation.GetTypeByMetadataName("SampleEnum")
                       ?? throw new InvalidOperationException("SampleEnum type was not found.");
        var enumMapConfiguration = new EnumMapConfiguration(
            [
                new EnumMapCase("SampleEnum.One", "1", "One"),
                new EnumMapCase("SampleEnum.Two", "2", "Two"),
            ],
            MappaMapEnumDefaultBehavior.Throw,
            null,
            []);

        var builderContext = new MappaBuilderContext(compilation);
        var expressionContext = new ExpressionBuildContext(
            builderContext,
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));

        AssertBuilds(new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>()), "source", "source");
        AssertBuilds(
            new NullableStrategy(
                nullableIntType,
                nullableIntType,
                new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>())),
            "source",
            "source.HasValue ? (int?)source.Value : (int?)null");
        AssertBuilds(
            new NullableStrategy(
                intType,
                nullableIntType,
                new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>())),
            "source",
            null);
        AssertBuilds(
            new NullableStrategy(
                stringType,
                stringType,
                new IdentityMapStrategy(stringType, stringType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>())),
            "source",
            null);
        AssertBuilds(new StringToUriMapStrategy(uriType, stringType), "source", "new System.Uri(source)");
        AssertBuilds(new InvokeParseMethodMapStrategy(intType, stringType), "source", $"{intType.ToDisplayString()}.Parse(source)");
        AssertBuilds(
            new StringToNumberMapStrategy(intType, stringType, CultureInfoSetting.InvariantCulture, null, null),
            "source",
            null);
        AssertBuilds(
            new InvokeToStringMapStrategy(stringType, intType, "G", CultureInfoSetting.InvariantCulture, null),
            "source",
            null);
        AssertBuilds(
            new InvokeToStringMapStrategy(stringType, intType, null, CultureInfoSetting.CurrentCulture, null),
            "source",
            null);
        AssertBuilds(
            new InvokeToStringMapStrategy(stringType, intType, "G", CultureInfoSetting.None, null),
            "source",
            null);
        AssertBuilds(
            new InvokeToStringMapStrategy(stringType, intType, null, CultureInfoSetting.Undefined, null),
            "source",
            "source.ToString()");
        AssertBuilds(
            new InvokeToStringMapStrategy(stringType, intType, null, CultureInfoSetting.UserDefined, "en-US"),
            "source",
            null);
        AssertBuilds(
            new InvokeParseStringWithFormatMapStrategy(dateTimeType, stringType, "O", CultureInfoSetting.InvariantCulture, null, null),
            "source",
            null);
        AssertBuilds(
            new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(dateOnlyType, stringType, "O", CultureInfoSetting.InvariantCulture, null, null),
            "source",
            null);
        AssertBuilds(new DateOnlyToDateTimeMapStrategy(dateTimeType, dateOnlyType), "source", null);
        AssertBuilds(new DateOnlyToLongMapStrategy(longType, dateOnlyType), "source", null);
        AssertBuilds(new DateTimeOffsetToDateOnlyMapStrategy(dateOnlyType, dateTimeOffsetType), "source", null);
        AssertBuilds(new DateTimeOffsetToDateTimeMapStrategy(dateTimeType, dateTimeOffsetType), "source", "source.DateTime");
        AssertBuilds(new DateTimeOffsetToLongMapStrategy(longType, dateTimeOffsetType), "source", "source.ToUnixTimeSeconds()");
        AssertBuilds(new DateTimeOffsetToTimeOnlyMapStrategy(timeOnlyType, dateTimeOffsetType), "source", null);
        AssertBuilds(new DateTimeToDateOnlyMapStrategy(dateOnlyType, dateTimeType), "source", null);
        AssertBuilds(new DateTimeToLongMapStrategy(longType, dateTimeType), "source", null);
        AssertBuilds(new DateTimeToTimeOnlyMapStrategy(timeOnlyType, dateTimeType), "source", null);
        AssertBuilds(new DoubleToTimeSpanMapStrategy(timeSpanType, doubleType), "source", "System.TimeSpan.FromDays(source)");
        AssertBuilds(new LongToDateTimeMapStrategy(dateTimeType, longType), "source", "System.DateTime.UnixEpoch.AddSeconds(source)");
        AssertBuilds(new LongToDateTimeOffsetMapStrategy(dateTimeOffsetType, longType), "source", "System.DateTimeOffset.FromUnixTimeSeconds(source)");
        AssertBuilds(new TimeSpanToDoubleMapStrategy(doubleType, timeSpanType), "source", "source.TotalDays");
        AssertBuilds(new EnumToIntegralMapStrategy(intType, enumType, enumMapConfiguration), "source", null);
        AssertBuilds(new IntegralToEnumMapStrategy(enumType, intType, enumMapConfiguration), "source", null);
        AssertBuilds(
            new EnumToStringMapStrategy(stringType, enumType, EnumStringMapSetting.MemberName, enumMapConfiguration),
            "source",
            null);
        AssertBuilds(
            new StringToEnumMapStrategy(enumType, stringType, BooleanSetting.Enable, EnumStringMapSetting.MemberName, enumMapConfiguration),
            "source",
            null);
        AssertBuilds(
            new StringToEnumMapStrategy(enumType, stringType, BooleanSetting.Disable, EnumStringMapSetting.MemberName, enumMapConfiguration),
            "source",
            null);

        void AssertBuilds(MapStrategy strategy, string source, string? expectedExact)
        {
            var built = ProjectionExpressionBuilder.TryBuildExpression(strategy, source, expressionContext, out var expression);
            built.Should().BeTrue();
            expression.Should().NotBeNullOrWhiteSpace();
            if (expectedExact is not null)
            {
                expression.Should().Be(expectedExact);
            }
        }
    }

    /// <summary>
    /// Test unsupported strategies are rejected by the expression builder.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionReturnsFalseForUnsupportedStrategy()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var strategy = new CollectionToCollectionMapStrategy(
            intType,
            intType,
            new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>()),
            null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));

        var built = ProjectionExpressionBuilder.TryBuildExpression(strategy, "source", expressionContext, out var expression);

        built.Should().BeFalse();
        expression.Should().BeEmpty();
    }
}