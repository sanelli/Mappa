// <copyright file="StringToNumberMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="StringToNumberMapStrategy"/>.
/// </summary>
public sealed class StringToNumberMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Returns the test data for <see cref="CanMapStringToNumber"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapStringToNumber"/>.</returns>
    public static IEnumerable<object[]> CanMapStringToNumberTestData()
    {
        yield return new object[] { "sbyte", typeof(sbyte).ToString() };
        yield return new object[] { "byte", typeof(byte).ToString() };
        yield return new object[] { "short", typeof(short).ToString() };
        yield return new object[] { "ushort", typeof(ushort).ToString() };
        yield return new object[] { "int", typeof(int).ToString() };
        yield return new object[] { "uint", typeof(uint).ToString() };
        yield return new object[] { "long", typeof(long).ToString() };
        yield return new object[] { "ulong", typeof(ulong).ToString() };
        yield return new object[] { "float", typeof(float).ToString() };
        yield return new object[] { "double", typeof(double).ToString() };
        yield return new object[] { "decimal", typeof(decimal).ToString() };
    }

    /// <summary>
    /// Test a mapping can be created from a string
    /// to a number.
    /// </summary>
    /// <param name="aliasNumericType">The type (e.g. <c>int</c>, <c>float</c>, ...).</param>
    /// <param name="numericType">The type fullname (e.g. <c>typeof(int).ToString()</c>).</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [MemberData(nameof(CanMapStringToNumberTestData))]
    public async Task CanMapStringToNumber(string aliasNumericType, string numericType)
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = """
                         using Mappa.Attributes;

                         namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                         [Mappa]
                         public sealed partial class Mapper
                         {
                             public partial %numeric-type% Map(string input);
                         }
                         """.Replace("%numeric-type%", aliasNumericType, StringComparison.Ordinal);

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodes(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    syntaxAssertions => syntaxAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}