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
    /// Test <see cref="SyntheticMapMethodNaming.SanitizeTypeName"/> replaces non-identifier characters.
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