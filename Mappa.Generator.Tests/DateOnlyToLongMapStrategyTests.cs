// <copyright file="DateOnlyToLongMapStrategyTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for <see cref="DateOnlyToLongMapStrategy"/>.
/// </summary>
public sealed class DateOnlyToLongMapStrategyTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="DateOnly"/>
    /// to a <see cref="long"/> object.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDateOnlyToLong()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(DateOnly input);
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
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(DateOnly).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                identifierName,
                                expressionSyntaxAssertions =>
                                expressionSyntaxAssertions.BeCastExpressionSyntax(
                                    typeof(long).ToString(),
                                    castAssertions => castAssertions.BeMemberAccessExpressionSyntax("new System.DateTime(input, System.TimeOnly.MinValue, System.DateTimeKind.Utc).ToUniversalTime().Subtract(System.DateTime.UnixEpoch).TotalSeconds")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}