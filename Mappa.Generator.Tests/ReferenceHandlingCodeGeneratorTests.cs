// <copyright file="ReferenceHandlingCodeGeneratorTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ReferenceHandlingCodeGenerator"/> predicate and wrap paths
/// (string/enum/value/container early returns for MaxRuntimeDepth and ReferenceReusing).
/// </summary>
public sealed class ReferenceHandlingCodeGeneratorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// MaxRuntimeDepth does not wrap string, enum, value, nullable-value, or container strategies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildNestedSourceDoesNotIncreaseDepthForIneligibleStrategies()
    {
        var (compilation, mapMethod, globalOptions, stringType, enumType, intType, nullableIntType, listType, referenceType)
            = CreateFixture();
        var context = new MappaBuilderContext(compilation);
        mapMethod.SetMaxRuntimeDepth(3);

        using (context.PushMapMethod(mapMethod))
        {
            context.IsMaxRuntimeDepthActive.Should().BeTrue();

            AssertNoIncreaseDepthWrap(
                new IdentityMapStrategy(
                    stringType,
                    stringType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Name",
                context,
                globalOptions);
            AssertNoIncreaseDepthWrap(
                new IdentityMapStrategy(
                    enumType,
                    enumType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Status",
                context,
                globalOptions);
            AssertNoIncreaseDepthWrap(
                new IdentityMapStrategy(
                    intType,
                    intType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Count",
                context,
                globalOptions);
            AssertNoIncreaseDepthWrap(
                new IdentityMapStrategy(
                    nullableIntType,
                    nullableIntType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Optional",
                context,
                globalOptions);
            AssertNoIncreaseDepthWrap(
                new NullableStrategy(nullableIntType, nullableIntType, new IdentityMapStrategy(intType, intType)),
                "input.Optional",
                context,
                globalOptions);
            AssertNoIncreaseDepthWrap(
                new CollectionToCollectionMapStrategy(
                    listType,
                    listType,
                    new IdentityMapStrategy(referenceType, referenceType),
                    methodSymbol: null,
                    BooleanSetting.Undefined,
                    BooleanSetting.Undefined,
                    BooleanSetting.Undefined,
                    EnumerableConcreteTypeSetting.Undefined),
                "input.Items",
                context,
                globalOptions);

            var nestedField = referenceType.GetMembers("Value").OfType<IFieldSymbol>().Single();
            AssertNoIncreaseDepthWrap(
                new IdentityMapStrategy(
                    referenceType,
                    referenceType,
                    IdentityMapDeepCopySetting.NestedDeepCopy,
                    requiresMemberwiseClone: true,
                    nestedFieldStrategies:
                    [
                        new IdentityMapNestedFieldStrategy(
                            nestedField,
                            new IdentityMapStrategy(nestedField.Type, nestedField.Type)),
                    ]),
                "input.Nested",
                context,
                globalOptions);

            var (_, referenceCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                new IdentityMapStrategy(
                    referenceType,
                    referenceType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Nested",
                context,
                globalOptions);
            var increaseDepthExpression = ParseBlock(referenceCode)
                .DescendantNodes()
                .OfType<UsingStatementSyntax>()
                .Should()
                .ContainSingle()
                .Subject
                .Expression;
            if (increaseDepthExpression is null)
            {
                throw new InvalidOperationException("IncreaseDepth using expression was not found.");
            }

            increaseDepthExpression.ToString().Should().Contain("IncreaseDepth");
        }
    }

    /// <summary>
    /// ReferenceReusing does not wrap string or enum strategies, but wraps eligible reference types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildNestedSourceDoesNotReuseReferencesForIneligibleStrategies()
    {
        var (compilation, mapMethod, globalOptions, stringType, enumType, intType, nullableIntType, _, referenceType)
            = CreateFixture();
        var context = new MappaBuilderContext(compilation);
        mapMethod.SetReferenceReusing(BooleanSetting.Enable);

        using (context.PushMapMethod(mapMethod))
        {
            context.IsReferenceReusingActive.Should().BeTrue();

            AssertNoTryGetReferenceWrap(
                new IdentityMapStrategy(stringType, stringType),
                "input.Name",
                context,
                globalOptions);
            AssertNoTryGetReferenceWrap(
                new IdentityMapStrategy(enumType, enumType),
                "input.Status",
                context,
                globalOptions);

            ReferenceHandlingCodeGenerator.ShouldRegisterReferencePairEarly(
                    context,
                    "input.Name",
                    stringType,
                    stringType)
                .Should()
                .BeFalse();
            ReferenceHandlingCodeGenerator.ShouldRegisterReferencePairEarly(
                    context,
                    "input.Count",
                    intType,
                    intType)
                .Should()
                .BeFalse();
            ReferenceHandlingCodeGenerator.ShouldRegisterReferencePairEarly(
                    context,
                    "input.Optional",
                    nullableIntType,
                    nullableIntType)
                .Should()
                .BeFalse();
            ReferenceHandlingCodeGenerator.BuildEarlyAddReferencePairStatement(
                    context,
                    "__target",
                    "input.Name",
                    stringType,
                    stringType)
                .Should()
                .BeNull();

            var (_, referenceCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                new IdentityMapStrategy(
                    referenceType,
                    referenceType,
                    IdentityMapDeepCopySetting.DeepCopy,
                    requiresMemberwiseClone: true),
                "input.Nested",
                context,
                globalOptions);
            ParseBlock(referenceCode)
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Select(invocation => invocation.Expression.ToString())
                .Should()
                .Contain(expression => expression.Contains("TryGetReference", StringComparison.Ordinal));

            ReferenceHandlingCodeGenerator.ShouldRegisterReferencePairEarly(
                    context,
                    "input.Nested",
                    referenceType,
                    referenceType)
                .Should()
                .BeTrue();
            ReferenceHandlingCodeGenerator.BuildEarlyAddReferencePairStatement(
                    context,
                    "__target",
                    "input.Nested",
                    referenceType,
                    referenceType)
                .Should()
                .NotBeNullOrWhiteSpace();
        }
    }

    /// <summary>
    /// Identity strategies without memberwise clone skip reference reuse.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildNestedSourceSkipsReuseForIdentityWithoutMemberwiseClone()
    {
        var (compilation, mapMethod, globalOptions, _, _, _, _, _, referenceType)
            = CreateFixture();
        var context = new MappaBuilderContext(compilation);
        mapMethod.SetReferenceReusing(BooleanSetting.Enable);

        using (context.PushMapMethod(mapMethod))
        {
            AssertNoTryGetReferenceWrap(
                new IdentityMapStrategy(referenceType, referenceType),
                "input.Nested",
                context,
                globalOptions);
        }
    }

    private static void AssertNoIncreaseDepthWrap(
        MapStrategy strategy,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions globalOptions)
    {
        var (_, wrappedCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            strategy,
            source,
            context,
            globalOptions);

        wrappedCode.Should().NotContain("IncreaseDepth");
    }

    private static void AssertNoTryGetReferenceWrap(
        MapStrategy strategy,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions globalOptions)
    {
        var (_, wrappedCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            strategy,
            source,
            context,
            globalOptions);

        wrappedCode.Should().NotContain("TryGetReference");
    }

    private static BlockSyntax ParseBlock(string code)
    {
        var tree = CSharpSyntaxTree.ParseText($"{{ {code} }}");
        return tree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<BlockSyntax>()
            .First();
    }

    private static (
        CSharpCompilation Compilation,
        MapMethod MapMethod,
        MappaGlobalOptions GlobalOptions,
        ITypeSymbol StringType,
        ITypeSymbol EnumType,
        ITypeSymbol IntType,
        ITypeSymbol NullableIntType,
        ITypeSymbol ListType,
        ITypeSymbol ReferenceType) CreateFixture()
    {
        const string source = """
                              using System.Collections.Generic;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum Status
                              {
                                  A,
                                  B,
                              }

                              public class Nested
                              {
                                  public int Value;
                              }

                              public class Source
                              {
                                  public string Name { get; set; } = string.Empty;
                                  public Status Status { get; set; }
                                  public int Count { get; set; }
                                  public int? Optional { get; set; }
                                  public List<Nested> Items { get; set; } = new();
                                  public Nested Nested { get; set; } = null!;
                              }

                              public class Target
                              {
                                  public string Name { get; set; } = string.Empty;
                                  public Status Status { get; set; }
                                  public int Count { get; set; }
                                  public int? Optional { get; set; }
                                  public List<Nested> Items { get; set; } = new();
                                  public Nested Nested { get; set; } = null!;
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input, MappaContext context);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees.Single(tree =>
            tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => method.Identifier.Text == "Map"));
        var methodDeclarationSyntax = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(method => method.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            CancellationToken.None);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            syntaxTree);

        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var nullableIntType = compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(intType);
        var enumType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Status")
            ?? throw new InvalidOperationException("Status enum was not found.");
        var referenceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Nested")
            ?? throw new InvalidOperationException("Nested type was not found.");
        var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")?.Construct(referenceType)
            ?? throw new InvalidOperationException("List<Nested> was not found.");

        return (compilation, mapMethod, globalOptions, stringType, enumType, intType, nullableIntType, listType, referenceType);
    }
}