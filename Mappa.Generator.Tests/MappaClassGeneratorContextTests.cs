// <copyright file="MappaClassGeneratorContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaClassGeneratorContext"/>.
/// </summary>
public sealed class MappaClassGeneratorContextTests
    : MappaGeneratorAbstractUnitTests
{
    private const string PolymorphicMapperSource = """
                                                     #nullable enable
                                                     using Mappa.Attributes;

                                                     namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                                     public class SourceBase { }

                                                     public class SourceFirst : SourceBase { }

                                                     public class TargetBase { }

                                                     public class TargetFirst : TargetBase { }

                                                     [Mappa]
                                                     public sealed partial class Mapper
                                                     {
                                                         public partial TargetBase MapDependency(SourceBase input);
                                                     }
                                                     """;

    /// <summary>
    /// Test <see cref="MappaClassGeneratorContext.TryGetPolymorphicMethod"/> ignores
    /// <see cref="MappaTypeMappingAttribute"/> entries whose types cannot be resolved in the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetPolymorphicMethodReturnsFalseWhenTypeMappingAttributeTypesAreNotInCompilation()
    {
        var context = CreateContext(PolymorphicMapperSource);
        var mapMethod = CreatePolymorphicMapMethod(
            [
                new MappaTypeMappingAttribute(
                    typeof(MappaClassGeneratorContextTests),
                    typeof(MappaClassGeneratorContextTests)),
            ]);
        context.TryAddMethod(mapMethod);

        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst")!;
        var userSettings = CreateUserSettingsWithPolymorphicDefaultEnabled(context.ClassDeclarationSyntax.SyntaxTree);

        var found = context.TryGetPolymorphicMethod(
            targetType,
            sourceType,
            nullableEnabled: true,
            requireStaticContext: false,
            userSettings,
            out var resolvedMethod);

        found.Should().BeFalse();
        resolvedMethod.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="MappaClassGeneratorContext.TryGetPolymorphicMethod"/> ignores
    /// <see cref="MappaTypeMappingDefaultAttribute"/> when its behavior is not
    /// <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetPolymorphicMethodReturnsFalseWhenTypeMappingDefaultAttributeHasUnsupportedBehavior()
    {
        var context = CreateContext(PolymorphicMapperSource);
        var mapMethod = CreatePolymorphicMapMethod(
            [
                new MappaTypeMappingAttribute(
                    typeof(MappaClassGeneratorContextTests),
                    typeof(MappaClassGeneratorContextTests)),
                new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw),
            ]);
        context.TryAddMethod(mapMethod);

        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst")!;
        var userSettings = CreateUserSettingsWithPolymorphicDefaultEnabled(context.ClassDeclarationSyntax.SyntaxTree);

        var found = context.TryGetPolymorphicMethod(
            targetType,
            sourceType,
            nullableEnabled: true,
            requireStaticContext: false,
            userSettings,
            out _);

        found.Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MappaClassGeneratorContext.TryGetPolymorphicMethod"/> ignores
    /// <see cref="MappaTypeMappingDefaultAttribute"/> when
    /// <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> is used without a target type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetPolymorphicMethodReturnsFalseWhenTypeMappingDefaultAttributeMapSourceTypeHasNoTargetType()
    {
        var context = CreateContext(PolymorphicMapperSource);
        var mapMethod = CreatePolymorphicMapMethod(
            [
                new MappaTypeMappingAttribute(
                    typeof(MappaClassGeneratorContextTests),
                    typeof(MappaClassGeneratorContextTests)),
                new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.MapSourceType),
            ]);
        context.TryAddMethod(mapMethod);

        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst")!;
        var userSettings = CreateUserSettingsWithPolymorphicDefaultEnabled(context.ClassDeclarationSyntax.SyntaxTree);

        var found = context.TryGetPolymorphicMethod(
            targetType,
            sourceType,
            nullableEnabled: true,
            requireStaticContext: false,
            userSettings,
            out _);

        found.Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MappaClassGeneratorContext.TryGetPolymorphicMethod"/> ignores
    /// <see cref="MappaTypeMappingDefaultAttribute"/> when its target type cannot be resolved in the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetPolymorphicMethodReturnsFalseWhenTypeMappingDefaultAttributeTargetTypeIsNotInCompilation()
    {
        var context = CreateContext(PolymorphicMapperSource);
        var mapMethod = CreatePolymorphicMapMethod(
            [
                new MappaTypeMappingAttribute(
                    typeof(MappaClassGeneratorContextTests),
                    typeof(MappaClassGeneratorContextTests)),
                new MappaTypeMappingDefaultAttribute(
                    MappaTypeMappingDefaultBehavior.MapSourceType,
                    typeof(MappaClassGeneratorContextTests)),
            ]);
        context.TryAddMethod(mapMethod);

        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst")!;
        var userSettings = CreateUserSettingsWithPolymorphicDefaultEnabled(context.ClassDeclarationSyntax.SyntaxTree);

        var found = context.TryGetPolymorphicMethod(
            targetType,
            sourceType,
            nullableEnabled: true,
            requireStaticContext: false,
            userSettings,
            out _);

        found.Should().BeFalse();
    }

    private static MappaClassGeneratorContext CreateContext(string source)
    {
        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees[0];
        var classDeclarationSyntax = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "Mapper");
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            syntaxTree);

        return new MappaClassGeneratorContext(
            globalOptions,
            new MappaDebug(globalOptions, _ => { }),
            compilation,
            classDeclarationSyntax);
    }

    private static MapMethod CreatePolymorphicMapMethod(Attribute[] attributes)
    {
        var compilation = BuildCompilation(PolymorphicMapperSource);
        var mapperType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        if (mapperType is null)
        {
            throw new InvalidOperationException("Expected mapper type to be present in the compilation.");
        }

        var methodSymbol = mapperType.GetMembers("MapDependency").OfType<IMethodSymbol>().Single();
        return new MapMethod(
            methodSymbol,
            "this",
            nullableEnabled: true,
            canBeUsedByStaticMethod: false,
            attributes);
    }

    private static MappaGlobalOptions CreateUserSettingsWithPolymorphicDefaultEnabled(SyntaxTree syntaxTree)
    {
        return new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("""
                root = true
                mappa.polymorphicmapmethodwithmatchingdefaultattribute = enable
                """),
            syntaxTree);
    }
}