// <copyright file="PreferredInterfaceMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for preferred collection/dictionary interface resolution.
/// </summary>
public sealed class PreferredInterfaceMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string DualEnumerableTypeDefinition = """
                                                          public sealed class DualEnumerable : System.Collections.Generic.IEnumerable<int>, System.Collections.Generic.IEnumerable<string>
                                                          {
                                                              public System.Collections.Generic.IEnumerator<int> GetEnumerator()
                                                              {
                                                                  yield return 1;
                                                              }

                                                              System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

                                                              System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator()
                                                              {
                                                                  yield return "a";
                                                              }
                                                          }
                                                          """;

    private const string DualEnumerableWithUnmappableTypeDefinition = """
                                                                        public sealed class Unmappable
                                                                        {
                                                                        }

                                                                        public sealed class DualEnumerable : System.Collections.Generic.IEnumerable<int>, System.Collections.Generic.IEnumerable<Unmappable>
                                                                        {
                                                                            public System.Collections.Generic.IEnumerator<int> GetEnumerator()
                                                                            {
                                                                                yield return 1;
                                                                            }

                                                                            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

                                                                            System.Collections.Generic.IEnumerator<Unmappable> System.Collections.Generic.IEnumerable<Unmappable>.GetEnumerator()
                                                                            {
                                                                                yield return new Unmappable();
                                                                            }
                                                                        }
                                                                        """;

    /// <summary>
    /// When a source type has a unique usable implicit <see cref="System.Collections.Generic.IEnumerable{T}"/>,
    /// mapping to <see cref="System.Collections.Generic.List{T}"/> uses that element type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapsDualEnumerableToListUsingUniqueImplicitIEnumerableOfInt()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial List<int> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<int>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<int>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// A source preferred-interface attribute forces an explicit <see cref="System.Collections.Generic.IEnumerable{T}"/> implementation.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SourceAttributeForcesExplicitIEnumerableOfString()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<string>))]
                                        public partial List<string> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<string>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<string>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<string>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.IEnumerable<string>",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Reports MP00079 when a matching preferred source interface cannot be used for the mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceCannotBeUsedWhenAttributeIsNotUsable()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableWithUnmappableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<Unmappable>))]
                                        public partial List<int> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceCannotBeUsed,
                "System.Collections.Generic.IEnumerable<Mappa.Generator.Tests.UnitTests.SourceCode.Unmappable>",
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Unmappable",
                "int");
    }

    /// <summary>
    /// Reports MP00083 for duplicate preferred-interface attributes on the same side, type, and family.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsDuplicatePreferredInterfaceAttribute()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<int>))]
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<string>))]
                                        public partial List<int> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.DuplicatePreferredInterfaceAttribute,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                "Map");
    }
}