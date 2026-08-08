// <copyright file="SyntheticMapMethodNamingTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="SyntheticMapMethodNaming"/>.
/// </summary>
public sealed class SyntheticMapMethodNamingTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.AllocateName"/> returns a stable base name when free.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllocateNameReturnsStableBaseNameWhenAvailable()
    {
        var context = CreateContext("""
                                    using Mappa.Attributes;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    public class Source { }

                                    public class Target { }

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial Target Map(Source input);
                                    }
                                    """);
        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;

        var name = SyntheticMapMethodNaming.AllocateName(context, sourceType, targetType);

        name.Should().Be("Map__Source__To__Target");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.AllocateName"/> appends a numeric suffix on collision.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllocateNameAppendsSuffixWhenBaseNameCollidesWithExistingMember()
    {
        var context = CreateContext("""
                                    using Mappa.Attributes;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    public class Source { }

                                    public class Target { }

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial Target Map(Source input);

                                        private Target Map__Source__To__Target(Source source) => new Target();
                                    }
                                    """);
        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;

        var name = SyntheticMapMethodNaming.AllocateName(context, sourceType, targetType);

        name.Should().Be("Map__Source__To__Target_1");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.AllocateName"/> increments the suffix when <c>_1</c> is also taken.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllocateNameIncrementsSuffixWhenNumberedNameAlsoCollides()
    {
        var context = CreateContext("""
                                    using Mappa.Attributes;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    public class Source { }

                                    public class Target { }

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial Target Map(Source input);

                                        private Target Map__Source__To__Target(Source source) => new Target();

                                        private Target Map__Source__To__Target_1(Source source) => new Target();
                                    }
                                    """);
        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;

        var name = SyntheticMapMethodNaming.AllocateName(context, sourceType, targetType);

        name.Should().Be("Map__Source__To__Target_2");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.AllocateName"/> treats queued map-method names as taken.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllocateNameTreatsQueuedMapMethodNamesAsTaken()
    {
        var context = CreateContext("""
                                    using Mappa.Attributes;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    public class Source { }

                                    public class Target { }

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial Target Map(Source input);
                                    }
                                    """);
        var sourceType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;
        var mapperType = context.Compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")!;
        var queued = MapMethod.CreateSynthetic(
            "Map__Source__To__Target",
            sourceType,
            targetType,
            mapperType,
            nullableEnabled: true,
            isStatic: false,
            sourceParameterName: "source",
            mappaContextParameterName: null,
            location: null);
        context.TryAddMethod(queued).Should().BeTrue();

        var name = SyntheticMapMethodNaming.AllocateName(context, sourceType, targetType);

        name.Should().Be("Map__Source__To__Target_1");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.SanitizeTypeName(ITypeSymbol)"/> replaces non-identifier characters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SanitizeTypeNameReplacesNonIdentifierCharacters()
    {
        var compilation = BuildCompilation("""
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public class Outer
                                           {
                                               public class Inner { }
                                           }
                                           """);
        var innerType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Outer+Inner")!;

        var sanitized = SyntheticMapMethodNaming.SanitizeTypeName(innerType);

        sanitized.Should().MatchRegex("^[A-Za-z_][A-Za-z0-9_]*$");
        sanitized.Should().Contain("Inner");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.SanitizeTypeName(ITypeSymbol)"/> replaces generic arity punctuation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SanitizeTypeNameReplacesGenericArityPunctuation()
    {
        var compilation = BuildCompilation("public class Holder { }");
        var dictionaryOpen = compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2");
        dictionaryOpen.Should().NotBeNull();
        var dictionaryType = dictionaryOpen!.Construct(
            compilation.GetSpecialType(SpecialType.System_String),
            compilation.GetSpecialType(SpecialType.System_Int32));

        var sanitized = SyntheticMapMethodNaming.SanitizeTypeName(dictionaryType);

        sanitized.Should().MatchRegex("^[A-Za-z_][A-Za-z0-9_]*$");
        sanitized.Should().Contain("Dictionary");
        sanitized.Should().Contain("_");
    }

    /// <summary>
    /// Test <see cref="SyntheticMapMethodNaming.SanitizeTypeName(string)"/> prefixes an underscore
    /// when the sanitized fragment would otherwise be empty or start with a digit.
    /// </summary>
    /// <param name="displayName">The raw display name.</param>
    /// <param name="expected">The expected sanitized identifier fragment.</param>
    [Theory]
    [UnitTest]
    [InlineData("", "_")]
    [InlineData("123Abc", "_123Abc")]
    [InlineData("!!!", "___")]
    public void SanitizeTypeNamePrefixesUnderscoreForEmptyOrInvalidStart(string displayName, string expected)
    {
        var sanitized = SyntheticMapMethodNaming.SanitizeTypeName(displayName);

        sanitized.Should().Be(expected);
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
}