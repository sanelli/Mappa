// <copyright file="ProjectionExpressionBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Builders.Expressions;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        AssertBuilds(
            new EnumToEnumMapStrategy(enumType, enumType, EnumToEnumMapSetting.MemberName, BooleanSetting.Disable, enumMapConfiguration),
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
    /// Test parameter and property projection expression building.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionSupportsParameterAndPropertyStrategies()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target
                              {
                                  public Target(int value) { Value = value; }
                                  public int Value { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
                         ?? throw new InvalidOperationException("Source type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var sourceProperty = sourceType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var parameter = targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1).Parameters[0];
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), compilation.SyntaxTrees[0]));

        var parameterStrategy = new ParameterMapStrategy(
            parameter,
            sourceProperty,
            new IdentityMapStrategy(parameter.Type, sourceProperty.Type, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: []));
        var propertyStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            new IdentityMapStrategy(targetProperty.Type, sourceProperty.Type, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: []),
            postConstructorInitializer: false);
        var propertyWithoutSource = new PropertyMapStrategy(
            targetProperty,
            sourceProperty: null,
            new IdentityMapStrategy(targetProperty.Type, targetProperty.Type, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: []),
            postConstructorInitializer: false);
        var constructorStrategy = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1),
            [parameterStrategy],
            [propertyStrategy],
            [],
            contextParameterName: null);

        ProjectionExpressionBuilder.TryBuildExpression(parameterStrategy, "source", expressionContext, out var parameterExpression)
            .Should()
            .BeTrue();
        parameterExpression.Should().Be("source.Value");
        ProjectionExpressionBuilder.TryBuildExpression(propertyStrategy, "source", expressionContext, out var propertyExpression)
            .Should()
            .BeTrue();
        propertyExpression.Should().Be("source.Value");
        ProjectionExpressionBuilder.TryBuildExpression(propertyWithoutSource, "source", expressionContext, out var propertyWithoutSourceExpression)
            .Should()
            .BeTrue();
        propertyWithoutSourceExpression.Should().Be("source");
        ProjectionExpressionBuilder.TryBuildExpression(constructorStrategy, "source", expressionContext, out var constructorExpression)
            .Should()
            .BeTrue();
        constructorExpression.Should().Contain("new");
        constructorExpression.Should().Contain("Value =");
    }

    /// <summary>
    /// Test nullable reference projection expression building for nullable and non-nullable targets.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionSupportsNullableReferenceStrategies()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var nullableStringType = stringType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableStringType = stringType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));

        ProjectionExpressionBuilder.TryBuildExpression(
                new NullableStrategy(
                    nullableStringType,
                    stringType,
                    new IdentityMapStrategy(stringType, stringType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: [])),
                "source",
                expressionContext,
                out var nullableTargetExpression)
            .Should()
            .BeTrue();
        nullableTargetExpression.Should().Contain("== null");
        nullableTargetExpression.Should().Contain("null :");
        ProjectionExpressionBuilder.TryBuildExpression(
                new NullableStrategy(
                    nonNullableStringType,
                    stringType,
                    new IdentityMapStrategy(stringType, stringType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: [])),
                "source",
                expressionContext,
                out var nonNullableTargetExpression)
            .Should()
            .BeTrue();
        nonNullableTargetExpression.Should().Contain("NullReferenceException");
    }

    /// <summary>
    /// Test chained source property paths are expressed using the root map method parameter when applicable.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionSupportsChainedSourcePropertyPaths()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Address
                              {
                                  public string City { get; set; }
                              }

                              public class Source
                              {
                                  public Address Address { get; set; }
                              }

                              public class Target
                              {
                                  public string City { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
                         ?? throw new InvalidOperationException("Source type was not found.");
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")
                          ?? throw new InvalidOperationException("Address type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var cityProperty = addressType.GetMembers("City").OfType<IPropertySymbol>().Single();
        var targetCity = targetType.GetMembers("City").OfType<IPropertySymbol>().Single();
        var mapMethod = CreateMapMethod(compilation, "Map");
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), compilation.SyntaxTrees[0]));
        var identity = new IdentityMapStrategy(
            targetCity.Type,
            cityProperty.Type,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);

        var matchingPrefix = new PropertyMapStrategy(
            targetCity,
            cityProperty,
            identity,
            postConstructorInitializer: false,
            new ChainedSourcePropertyPathInfo("Address.City", ["City"], addressType, "input.Address"));
        var nestedPrefix = new PropertyMapStrategy(
            targetCity,
            cityProperty,
            identity,
            postConstructorInitializer: false,
            new ChainedSourcePropertyPathInfo("Address.City", ["City"], addressType, "input.Address.Something"));
        var unrelatedPrefix = new PropertyMapStrategy(
            targetCity,
            cityProperty,
            identity,
            postConstructorInitializer: false,
            new ChainedSourcePropertyPathInfo("Address.City", ["City"], addressType, "other.Address"));
        var emptyPrefix = new PropertyMapStrategy(
            targetCity,
            cityProperty,
            identity,
            postConstructorInitializer: false,
            new ChainedSourcePropertyPathInfo("City", ["City"], sourceType, string.Empty));

        using (expressionContext.BuilderContext.PushMapMethod(mapMethod))
        {
            ProjectionExpressionBuilder.TryBuildExpression(matchingPrefix, "ignored", expressionContext, out var matchingExpression)
                .Should()
                .BeTrue();
            matchingExpression.Should().Contain("input");
            ProjectionExpressionBuilder.TryBuildExpression(nestedPrefix, "ignored", expressionContext, out var nestedExpression)
                .Should()
                .BeTrue();
            nestedExpression.Should().Contain("input");
            ProjectionExpressionBuilder.TryBuildExpression(unrelatedPrefix, "ignored", expressionContext, out var unrelatedExpression)
                .Should()
                .BeTrue();
            unrelatedExpression.Should().NotBeNullOrWhiteSpace();
            ProjectionExpressionBuilder.TryBuildExpression(emptyPrefix, "source", expressionContext, out var emptyPrefixExpression)
                .Should()
                .BeTrue();
            emptyPrefixExpression.Should().NotBeNullOrWhiteSpace();

            var chainedWithoutSourceProperty = new PropertyMapStrategy(
                targetCity,
                sourceProperty: null,
                identity,
                postConstructorInitializer: false,
                new ChainedSourcePropertyPathInfo("Address.City", ["City"], addressType, "input.Address"));
            ProjectionExpressionBuilder.TryBuildExpression(chainedWithoutSourceProperty, "ignored", expressionContext, out var chainedWithoutSourceExpression)
                .Should()
                .BeTrue();
            chainedWithoutSourceExpression.Should().Contain("input");
        }
    }

    /// <summary>
    /// Test unsupported culture settings throw when building ToString projection expressions.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionThrowsForUnsupportedCultureInfoSetting()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));
        var strategy = new InvokeToStringMapStrategy(stringType, intType, null, (CultureInfoSetting)42, null);

        var act = () => ProjectionExpressionBuilder.TryBuildExpression(strategy, "source", expressionContext, out _);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Test user-defined culture with a null culture name falls back to an empty culture name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionSupportsUserDefinedCultureWithNullCultureName()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));
        var strategy = new InvokeToStringMapStrategy(stringType, intType, null, CultureInfoSetting.UserDefined, null);

        ProjectionExpressionBuilder.TryBuildExpression(strategy, "source", expressionContext, out var expression)
            .Should()
            .BeTrue();
        expression.Should().Contain("GetCultureInfo");
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

    /// <summary>
    /// Test integral-to-enum projection throws when the target type has no enum underlying type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionThrowsWhenIntegralToEnumTargetHasNoUnderlyingType()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken)));
        var strategy = new IntegralToEnumMapStrategy(
            intType,
            intType,
            new EnumMapConfiguration([], MappaMapEnumDefaultBehavior.Throw, null, []));

        var act = () => ProjectionExpressionBuilder.TryBuildExpression(strategy, "source", expressionContext, out _);

        act.Should().Throw<MappaGeneratorException>()
            .WithMessage("*does not have an underlying type*");
    }

    /// <summary>
    /// Test nested builders throw when their element strategies cannot be projected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NestedProjectionBuildersThrowWhenElementStrategyIsUnsupported()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target
                              {
                                  public Target(int value) { Value = value; }
                                  public int Value { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
                         ?? throw new InvalidOperationException("Source type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var sourceProperty = sourceType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var parameter = targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1).Parameters[0];
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var unsupported = new CollectionToCollectionMapStrategy(
            intType,
            intType,
            new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: []),
            null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), compilation.SyntaxTrees[0]));

        var nullableAct = () => ProjectionExpressionBuilder.BuildNullableExpression(
            new NullableStrategy(intType, intType, unsupported),
            "source",
            expressionContext);
        var parameterAct = () => ProjectionExpressionBuilder.BuildParameterExpression(
            new ParameterMapStrategy(parameter, sourceProperty, unsupported),
            "source",
            expressionContext);
        var propertyAct = () => ProjectionExpressionBuilder.BuildPropertyExpression(
            new PropertyMapStrategy(targetProperty, sourceProperty, unsupported, postConstructorInitializer: false),
            "source",
            expressionContext);

        nullableAct.Should().Throw<MappaGeneratorException>()
            .WithMessage("Nullable projection element strategy is not supported.");
        parameterAct.Should().Throw<MappaGeneratorException>()
            .WithMessage("Parameter projection strategy is not supported.");
        propertyAct.Should().Throw<MappaGeneratorException>()
            .WithMessage("Property projection strategy is not supported.");
    }

    /// <summary>
    /// Test whitespace-only chained receiver prefixes do not rewrite the chain source.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryBuildExpressionIgnoresWhitespaceOnlyReceiverPathPrefix()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Address
                              {
                                  public string City { get; set; }
                              }

                              public class Source
                              {
                                  public Address Address { get; set; }
                              }

                              public class Target
                              {
                                  public string City { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")
                          ?? throw new InvalidOperationException("Address type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var cityProperty = addressType.GetMembers("City").OfType<IPropertySymbol>().Single();
        var targetCity = targetType.GetMembers("City").OfType<IPropertySymbol>().Single();
        var mapMethod = CreateMapMethod(compilation, "Map");
        var expressionContext = new ExpressionBuildContext(
            new MappaBuilderContext(compilation),
            new MappaGlobalOptions(TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"), compilation.SyntaxTrees[0]));
        var identity = new IdentityMapStrategy(
            targetCity.Type,
            cityProperty.Type,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);
        var whitespacePrefix = new PropertyMapStrategy(
            targetCity,
            cityProperty,
            identity,
            postConstructorInitializer: false,
            new ChainedSourcePropertyPathInfo("Address.City", ["City"], addressType, "   "));

        using (expressionContext.BuilderContext.PushMapMethod(mapMethod))
        {
            ProjectionExpressionBuilder.TryBuildExpression(whitespacePrefix, "lambdaSource", expressionContext, out var expression)
                .Should()
                .BeTrue();
            expression.Should().NotBeNullOrWhiteSpace();
        }
    }

    private static MapMethod CreateMapMethod(CSharpCompilation compilation, string methodName)
    {
        var syntaxTree = compilation.SyntaxTrees.Single(tree =>
            tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Any(methodSyntax => methodSyntax.Identifier.Text == methodName));
        var methodDeclarationSyntax = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == methodName);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        return new MapMethod(
            methodDeclarationSyntax,
            semanticModel,
            nullableEnabled: true,
            CancellationToken.None);
    }
}