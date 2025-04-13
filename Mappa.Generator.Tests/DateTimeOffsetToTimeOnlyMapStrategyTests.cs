// <copyright file="DateTimeOffsetToTimeOnlyMapStrategyTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for <see cref="DateTimeOffsetToTimeOnlyMapStrategy"/>.
/// </summary>
public sealed class DateTimeOffsetToTimeOnlyMapStrategyTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="DateTimeOffset"/>
    /// to a <see cref="TimeOnly"/> object.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDateTimeOffsetToTimeOnly()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TimeOnly Map(DateTimeOffset input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(TimeOnly).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(DateTimeOffset).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(TimeOnly).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    "System.TimeOnly.FromDateTime",
                                    syntaxAssertions => syntaxAssertions.BeMemberAccessExpressionSyntax("input.DateTime")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}