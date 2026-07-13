// <copyright file="ReadonlyDictionaryPropertyMapStrategyIntegrationTests.DictionaryAssignment.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaSettingsAttribute.DictionaryAssignment"/> on
/// <see cref="ReadonlyDictionaryPropertyMapStrategy"/>.
/// </summary>
public sealed partial class ReadonlyDictionaryPropertyMapStrategyIntegrationTests
{
    /// <summary>
    /// Test that a map from <see cref="Dictionary{TKey,TValue}"/> to a get-only dictionary property
    /// uses <see cref="IDictionary{TKey,TValue}.Add"/> when
    /// <see cref="MappaSettingsAttribute.DictionaryAssignment"/> is <see cref="DictionaryAssignmentSetting.Add"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenSetterIsNotProvidedOnTargetPropertyAndDictionaryAssignmentIsAdd()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public Dictionary<int, string> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public Dictionary<string, int> PropertyA {get;}
                                  }

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, string>).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<int, string>).ToString(),
                                "__mappa_tmp_3",
                                expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                forBlockAssertions =>
                                {
                                    forBlockAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Key"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_6",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Value"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_7",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("int.Parse", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.PropertyA.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a map to a get-only dictionary property with explicit <see cref="IDictionary{TKey,TValue}.Add"/>
    /// uses <see cref="IDictionary{TKey,TValue}.Add"/> when
    /// <see cref="MappaSettingsAttribute.DictionaryAssignment"/> is <see cref="DictionaryAssignmentSetting.Add"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenSetterIsNotProvidedOnTargetPropertyWithExplicitIDictionaryAddAndDictionaryAssignmentIsAdd()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public partial class TargetDictionary<K, V> : IDictionary<K, V>
                                  {
                                      void IDictionary<K, V>.Add(K key, V value) { }
                                  }

                                  public class Source
                                  {
                                      public Dictionary<int, string> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      public TargetDictionary<string, int> PropertyA {get;}
                                  }

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, string>).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<int, string>).ToString(),
                                "__mappa_tmp_3",
                                expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                forBlockAssertions =>
                                {
                                    forBlockAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(6)
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Key"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_6",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Value"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_7",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("int.Parse", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                "System.Collections.Generic.IDictionary<string, int>",
                                                "__mappa_tmp_8",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_8.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}