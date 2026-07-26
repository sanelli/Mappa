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

        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees[0];
        var classDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "Mapper");
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            syntaxTree);
        var classContext = new MappaClassGeneratorContext(
            globalOptions,
            new MappaDebug(globalOptions, _ => { }),
            compilation,
            classDeclarationSyntax);
        var methodDeclarationSyntax = classDeclarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);
        var orderDtoType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.OrderDto")
                           ?? throw new InvalidOperationException("OrderDto type was not found.");
        var orderType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Order")
                        ?? throw new InvalidOperationException("Order type was not found.");
        var targetProperty = orderDtoType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var sourceProperty = orderType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var propertyStrategy = new PropertyMapStrategy(
            targetProperty,
            sourceProperty,
            new IdentityMapStrategy(targetProperty.Type, sourceProperty.Type, IdentityMapDeepCopySetting.ShallowCopy, requiresMemberwiseClone: false, nestedFieldStrategies: Array.Empty<IdentityMapNestedFieldStrategy>()),
            postConstructorInitializer: false);
        var analysisContext = new ProjectionCapabilityAnalysisContext(
            methodContext,
            compilation,
            "ProjectToDto",
            null,
            TestContext.Current.CancellationToken);

        var supported = ProjectionCapabilityAnalyzer.TryAnalyze(propertyStrategy, analysisContext, out _);

        supported.Should().BeFalse();
        classContext.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.ProjectionNestedQueryableNotSupported));
    }
}