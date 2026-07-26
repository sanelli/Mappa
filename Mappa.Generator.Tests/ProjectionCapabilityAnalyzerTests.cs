// <copyright file="ProjectionCapabilityAnalyzerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Algorithm;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Diagnostics.Debug;
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
/// Unit tests for <see cref="ProjectionCapabilityAnalyzer"/>.
/// </summary>
public sealed class ProjectionCapabilityAnalyzerTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test shallow-copy identity strategies are supported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsTrueForShallowCopyIdentity()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var strategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeTrue();
    }

    /// <summary>
    /// Test deep-copy identity strategies are not supported for projections.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsFalseForDeepCopyIdentity()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var strategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.DeepCopy);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeFalse();
    }

    /// <summary>
    /// Test collection-to-collection strategies are not supported for projections.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsFalseForCollectionToCollectionMapStrategy()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var elementStrategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);
        var strategy = new CollectionToCollectionMapStrategy(
            intType,
            intType,
            elementStrategy,
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeFalse();
    }

    /// <summary>
    /// Test built-in conversion strategies are supported for projections.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsTrueForBuiltInTranslatableStrategies()
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DateOnly).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
            ]);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var longType = compilation.GetSpecialType(SpecialType.System_Int64);
        var doubleType = compilation.GetSpecialType(SpecialType.System_Double);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var dateTimeType = compilation.GetTypeByMetadataName(typeof(DateTime).FullName!)!;
        var dateTimeOffsetType = compilation.GetTypeByMetadataName(typeof(DateTimeOffset).FullName!)!;
        var dateOnlyType = compilation.GetTypeByMetadataName(typeof(DateOnly).FullName!)!;
        var timeOnlyType = compilation.GetTypeByMetadataName(typeof(TimeOnly).FullName!)!;
        var timeSpanType = compilation.GetTypeByMetadataName(typeof(TimeSpan).FullName!)!;
        var uriType = compilation.GetTypeByMetadataName(typeof(Uri).FullName!)!;
        var enumMapConfiguration = new EnumMapConfiguration([], MappaMapEnumDefaultBehavior.Throw, null, []);

        ProjectionCapabilityAnalyzer.IsSupported(new EnumToIntegralMapStrategy(intType, intType, enumMapConfiguration)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new IntegralToEnumMapStrategy(intType, intType, enumMapConfiguration)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new InvokeParseMethodMapStrategy(intType, stringType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new InvokeToStringMapStrategy(stringType, intType, null, CultureInfoSetting.None, null)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new InvokeParseStringWithFormatMapStrategy(dateTimeType, stringType, null, CultureInfoSetting.None, null, null)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(dateOnlyType, stringType, null, CultureInfoSetting.None, null, null)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new StringToNumberMapStrategy(intType, stringType, CultureInfoSetting.None, null, null)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new StringToUriMapStrategy(uriType, stringType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateOnlyToDateTimeMapStrategy(dateTimeType, dateOnlyType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateOnlyToLongMapStrategy(longType, dateOnlyType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeOffsetToDateOnlyMapStrategy(dateOnlyType, dateTimeOffsetType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeOffsetToDateTimeMapStrategy(dateTimeType, dateTimeOffsetType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeOffsetToLongMapStrategy(longType, dateTimeOffsetType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeOffsetToTimeOnlyMapStrategy(timeOnlyType, dateTimeOffsetType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeToDateOnlyMapStrategy(dateOnlyType, dateTimeType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeToTimeOnlyMapStrategy(timeOnlyType, dateTimeType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DateTimeToLongMapStrategy(longType, dateTimeType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new DoubleToTimeSpanMapStrategy(timeSpanType, doubleType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new LongToDateTimeMapStrategy(dateTimeType, longType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new LongToDateTimeOffsetMapStrategy(dateTimeOffsetType, longType)).Should().BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(new TimeSpanToDoubleMapStrategy(doubleType, timeSpanType)).Should().BeTrue();
    }

    /// <summary>
    /// Test enum strategies invoke warning helpers during analysis.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedInvokesEnumWarningHelpers()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var enumMapConfiguration = new EnumMapConfiguration([], MappaMapEnumDefaultBehavior.Throw, null, []);

        ProjectionCapabilityAnalyzer.IsSupported(
                new EnumToEnumMapStrategy(intType, intType, EnumToEnumMapSetting.MemberName, BooleanSetting.Enable, enumMapConfiguration))
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(
                new EnumToStringMapStrategy(stringType, intType, EnumStringMapSetting.MemberName, enumMapConfiguration))
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.IsSupported(
                new StringToEnumMapStrategy(intType, stringType, BooleanSetting.Enable, EnumStringMapSetting.MemberName, enumMapConfiguration))
            .Should()
            .BeTrue();
    }

    /// <summary>
    /// Test method-map and invoke-method strategies are rejected when they cannot be inlined.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsFalseForNonInlinableMethodStrategies()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target { public int Value { get; set; } }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                                  public Target MapWithContext(Source input, MappaContext context);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapperType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")
                         ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapMethodSymbol = mapperType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapWithContextSymbol = mapperType.GetMembers("MapWithContext").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(mapMethodSymbol, "this", nullableEnabled: true, canBeUsedByStaticMethod: false, attributes: []);
        var mapWithContextMethod = new MapMethod(mapWithContextSymbol, "this", nullableEnabled: true, canBeUsedByStaticMethod: false, attributes: []);
        var methodMapStrategy = new MethodMapStrategy(mapMethod, contextParameterName: null);
        var methodMapWithContextStrategy = new MethodMapStrategy(mapWithContextMethod, "context");
        var invokeStrategy = new MappaInvokeMethodAttributeStrategy(
            mapWithContextSymbol.ReturnType,
            mapWithContextSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "MapWithContext"),
            fieldOrProperty: null,
            mapWithContextSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: "context");

        ProjectionCapabilityAnalyzer.IsSupported(methodMapStrategy).Should().BeFalse();
        ProjectionCapabilityAnalyzer.IsSupported(methodMapWithContextStrategy).Should().BeFalse();
        ProjectionCapabilityAnalyzer.IsSupported(invokeStrategy).Should().BeFalse();
        ProjectionCapabilityAnalyzer.IsSupported(new MappaInvokeMethodAttributeStrategy(
            mapMethodSymbol.ReturnType,
            mapMethodSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "Map"),
            fieldOrProperty: null,
            mapMethodSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: null)).Should().BeFalse();
    }

    /// <summary>
    /// Test nested queryable properties are rejected when analyzed with a projection context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeReportsNestedQueryableNotSupported()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Child { public int Id { get; set; } }
                              public class ChildDto { public int Id { get; set; } }
                              public class Order { public IQueryable<Child> Items { get; set; } = null!; }
                              public class OrderDto { public IQueryable<ChildDto> Items { get; set; } = null!; }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source);
        var orderDtoType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.OrderDto")
                           ?? throw new InvalidOperationException("OrderDto type was not found.");
        var orderType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Order")
                        ?? throw new InvalidOperationException("Order type was not found.");
        var targetProperty = orderDtoType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var sourceProperty = orderType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var propertyStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            CreateShallowIdentity(targetProperty.Type, sourceProperty.Type),
            postConstructorInitializer: false);

        var supported = ProjectionCapabilityAnalyzer.TryAnalyze(propertyStrategy, analysisContext, out _);

        supported.Should().BeFalse();
        classContext.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.ProjectionNestedQueryableNotSupported));
    }

    /// <summary>
    /// Test deep-copy identity fails analysis without producing a failure kind.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeReturnsFalseWithoutDiagnosticWhenFailureKindIsNull()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order { public int Id { get; set; } }
                              public class OrderDto { public int Id { get; set; } }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var strategy = new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.DeepCopy);

        var supported = ProjectionCapabilityAnalyzer.TryAnalyze(strategy, analysisContext, out _);

        supported.Should().BeFalse();
        classContext.Diagnostics.Should().BeEmpty();
    }

    /// <summary>
    /// Test unsupported constructs report <see cref="MappaDiagnosticDescriptors.ProjectionMappingNotSupported"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeReportsProjectionMappingNotSupportedForUnsupportedConstructs()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target
                              {
                                  public Target(int value) { Value = value; }
                                  public int Value { get; set; }
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                              }
                              """;

        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
                         ?? throw new InvalidOperationException("Source type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var sourceProperty = sourceType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var constructor = targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1);
        var parameter = constructor.Parameters[0];
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var collectionStrategy = new CollectionToCollectionMapStrategy(
            intType,
            intType,
            CreateShallowIdentity(intType, intType),
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);
        var assignToContextStrategy = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            constructor,
            [],
            [],
            [new MappaAssignToContextEntry("key", "Value")],
            contextParameterName: null);
        var contextParameterStrategy = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            constructor,
            [],
            [],
            [],
            contextParameterName: "context");
        var postConstructorProperty = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            CreateShallowIdentity(targetProperty.Type, sourceProperty.Type),
            postConstructorInitializer: true);
        var postConstructorViaInitializer = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            constructor,
            [],
            [postConstructorProperty],
            [],
            contextParameterName: null);
        var nullableUnsupported = new NullableStrategy(
            intType,
            intType,
            new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.DeepCopy));
        var unsupportedParameter = new ParameterMapStrategy(
            parameter,
            sourceProperty,
            new IdentityMapStrategy(intType, intType, IdentityMapDeepCopySetting.DeepCopy));
        var unsupportedConstructorParameter = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            constructor,
            [unsupportedParameter],
            [],
            [],
            contextParameterName: null);
        var unsupportedPropertyElement = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            collectionStrategy,
            postConstructorInitializer: false);
        var unsupportedConstructorInitializer = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            constructor,
            [],
            [unsupportedPropertyElement],
            [],
            contextParameterName: null);

        ProjectionCapabilityAnalyzer.TryAnalyze(collectionStrategy, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(assignToContextStrategy, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(contextParameterStrategy, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(postConstructorProperty, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(postConstructorViaInitializer, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(nullableUnsupported, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(unsupportedParameter, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(unsupportedConstructorParameter, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(unsupportedConstructorInitializer, analysisContext, out _).Should().BeFalse();

        classContext.Diagnostics.Should().OnlyContain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.ProjectionMappingNotSupported));
        classContext.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(5);
    }

    /// <summary>
    /// Test method-map and invoke-method strategies report invoke-method-not-inlinable diagnostics.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeReportsInvokeMethodNotInlinable()
    {
        const string source = """
                              #nullable enable
                              using System;
                              using System.Linq;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target { public Uri Value { get; set; } = null!; }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                                  public Target MapWithContext(Source input, MappaContext context);
                                  public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                              }
                              """;

        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source, methodName: "ProjectToDto");
        var mapperType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")
                         ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapMethodSymbol = mapperType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapWithContextSymbol = mapperType.GetMembers("MapWithContext").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(mapMethodSymbol, "this", nullableEnabled: true, canBeUsedByStaticMethod: false, attributes: []);
        var mapWithContextMethod = new MapMethod(mapWithContextSymbol, "this", nullableEnabled: true, canBeUsedByStaticMethod: false, attributes: []);
        var methodMapWithContext = new MethodMapStrategy(mapWithContextMethod, "context");
        var methodMapWithoutResolvedInline = new MethodMapStrategy(mapMethod, contextParameterName: null);
        var invokeWithContext = new MappaInvokeMethodAttributeStrategy(
            mapWithContextSymbol.ReturnType,
            mapWithContextSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "MapWithContext"),
            fieldOrProperty: null,
            mapWithContextSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: "context");
        var invokeWithoutContext = new MappaInvokeMethodAttributeStrategy(
            mapMethodSymbol.ReturnType,
            mapMethodSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "MissingMethod"),
            fieldOrProperty: null,
            mapMethodSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: null);

        classContext.TryAddMethod(mapWithContextMethod).Should().BeTrue();
        var invokeResolvedContextRequired = new MappaInvokeMethodAttributeStrategy(
            mapWithContextSymbol.ReturnType,
            mapWithContextSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "MapWithContext"),
            fieldOrProperty: null,
            mapWithContextSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: null);

        ProjectionCapabilityAnalyzer.TryAnalyze(methodMapWithContext, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(methodMapWithoutResolvedInline, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(invokeWithContext, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(invokeWithoutContext, analysisContext, out _).Should().BeFalse();
        ProjectionCapabilityAnalyzer.TryAnalyze(invokeResolvedContextRequired, analysisContext, out _).Should().BeFalse();

        classContext.Diagnostics.Should().OnlyContain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.ProjectionInvokeMethodNotInlinable));
        classContext.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    /// <summary>
    /// Test enum strategies emit projection warnings when case-insensitive member-name matching is enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeReportsEnumStrategyWarningWhenCaseInsensitiveMemberNameIsEnabled()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SourceEnum { One }
                              public enum TargetEnum { One }
                              public class Source { public SourceEnum Value { get; set; } }
                              public class Target { public TargetEnum Value { get; set; } }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                              }
                              """;

        const string editorConfig = """
                                    root = true
                                    mappa.caseinsensitiveenummap = enable
                                    """;
        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source, editorConfig: editorConfig);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var enumMapConfiguration = new EnumMapConfiguration([], MappaMapEnumDefaultBehavior.Throw, null, []);

        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToEnumMapStrategy(intType, intType, EnumToEnumMapSetting.Undefined, BooleanSetting.Enable, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToEnumMapStrategy(intType, intType, EnumToEnumMapSetting.MemberName, BooleanSetting.Enable, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToStringMapStrategy(stringType, intType, EnumStringMapSetting.Undefined, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToStringMapStrategy(stringType, intType, EnumStringMapSetting.MemberName, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new StringToEnumMapStrategy(intType, stringType, BooleanSetting.Enable, EnumStringMapSetting.Undefined, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new StringToEnumMapStrategy(intType, stringType, BooleanSetting.Enable, EnumStringMapSetting.MemberName, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToEnumMapStrategy(intType, intType, EnumToEnumMapSetting.NumericValue, BooleanSetting.Enable, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(
                new EnumToStringMapStrategy(stringType, intType, EnumStringMapSetting.Description, enumMapConfiguration),
                analysisContext,
                out _)
            .Should()
            .BeTrue();

        classContext.Diagnostics.Should().OnlyContain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.ProjectionEnumStrategyNotSupported));
        classContext.Diagnostics.Should().HaveCount(6);
    }

    /// <summary>
    /// Test method-map strategies can be inlined when a non-method strategy is available.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeInlinesMethodMapWhenConstructorStrategyIsAvailable()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target { public int Value { get; set; } }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                                  public partial IQueryable<Target> ProjectToDto(IQueryable<Source> query);
                              }
                              """;

        var (analysisContext, classContext, compilation) = CreateAnalysisContext(source, methodName: "ProjectToDto");
        var mapperType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")
                         ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapMethodSymbol = mapperType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(mapMethodSymbol, "this", nullableEnabled: true, canBeUsedByStaticMethod: false, attributes: []);
        classContext.TryAddMethod(mapMethod).Should().BeTrue();
        var methodMapStrategy = new MethodMapStrategy(mapMethod, contextParameterName: null);
        var invokeWithoutContext = new MappaInvokeMethodAttributeStrategy(
            mapMethodSymbol.ReturnType,
            mapMethodSymbol.Parameters[0].Type,
            new MappaInvokeMethodAttribute("Value", "Map"),
            fieldOrProperty: null,
            mapMethodSymbol,
            sourceProperty: null,
            isNullableEnabled: true,
            contextParameterName: null);

        ProjectionCapabilityAnalyzer.TryAnalyze(methodMapStrategy, analysisContext, out var normalized)
            .Should()
            .BeTrue();
        normalized.Should().NotBeOfType<MethodMapStrategy>();
        ProjectionCapabilityAnalyzer.TryAnalyze(invokeWithoutContext, analysisContext, out _).Should().BeTrue();
    }

    /// <summary>
    /// Test parameter and property strategies are supported when their element strategy is supported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryAnalyzeSupportsParameterAndPropertyStrategies()
    {
        const string source = """
                              #nullable enable
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int Value { get; set; } }
                              public class Target
                              {
                                  public Target(int value) { Value = value; }
                                  public int Value { get; set; }
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                              }
                              """;

        var (analysisContext, _, compilation) = CreateAnalysisContext(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
                         ?? throw new InvalidOperationException("Source type was not found.");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
                         ?? throw new InvalidOperationException("Target type was not found.");
        var sourceProperty = sourceType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var parameter = targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1).Parameters[0];
        var parameterStrategy = new ParameterMapStrategy(
            parameter,
            sourceProperty,
            CreateShallowIdentity(parameter.Type, sourceProperty.Type));
        var propertyStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            CreateShallowIdentity(targetProperty.Type, sourceProperty.Type),
            postConstructorInitializer: false);
        var constructorStrategy = new InvokeConstructorMapStrategy(
            targetType,
            sourceType,
            targetType.InstanceConstructors.Single(candidate => candidate.Parameters.Length == 1),
            [parameterStrategy],
            [propertyStrategy],
            [],
            contextParameterName: null);

        ProjectionCapabilityAnalyzer.TryAnalyze(parameterStrategy, analysisContext, out _).Should().BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(propertyStrategy, analysisContext, out _).Should().BeTrue();
        ProjectionCapabilityAnalyzer.TryAnalyze(constructorStrategy, analysisContext, out _).Should().BeTrue();
    }

    private static IdentityMapStrategy CreateShallowIdentity(ITypeSymbol targetType, ITypeSymbol sourceType)
        => new(targetType, sourceType, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: []);

    private static (ProjectionCapabilityAnalysisContext AnalysisContext, MappaClassGeneratorContext ClassContext, Compilation Compilation) CreateAnalysisContext(
        string source,
        string methodName = "ProjectToDto",
        string editorConfig = "root = true")
    {
        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees[0];
        var classDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "Mapper");
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            syntaxTree);
        var classContext = new MappaClassGeneratorContext(
            globalOptions,
            new MappaDebug(globalOptions, _ => { }),
            compilation,
            classDeclarationSyntax);
        var methodDeclarationSyntax = classDeclarationSyntax.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == methodName);
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);
        var analysisContext = new ProjectionCapabilityAnalysisContext(
            methodContext,
            compilation,
            methodName,
            null,
            TestContext.Current.CancellationToken);
        return (analysisContext, classContext, compilation);
    }
}