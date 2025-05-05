// <copyright file="GuidStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="GuidStrategyDetector"/>.
/// </summary>
public sealed class GuidStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be made from <see cref="Guid"/>.
    /// </summary>
    /// <param name="type">The target type being tested.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("byte[]")]
    [InlineData("System.Span<byte>")]
    [InlineData("System.ReadOnlySpan<byte>")]
    [InlineData("System.Memory<byte>")]
    [InlineData("System.ReadOnlyMemory<byte>")]
    public async Task CanMapFromGuid(string type)
    {
        // Arrange
        string sourceCode = $$"""
                              #nullable enable
                              using System;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial {{type}} Map(Guid input);
                              }
                              #nullable restore
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
                type,
                NullableAnnotation.NotAnnotated,
                typeof(Guid).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                            type,
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("input.ToByteArray")))
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                        }));
                });
    }

    /// <summary>
    /// Test a mapping can be made from <see cref="Guid"/>.
    /// </summary>
    /// <param name="type">The target type being tested.</param>
    /// <param name="isMemory"><c>true</c> when <paramref name="type"/> is <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("byte[]", false)]
    [InlineData("System.Span<byte>", false)]
    [InlineData("System.ReadOnlySpan<byte>", false)]
    [InlineData("System.Memory<byte>", true)]
    [InlineData("System.ReadOnlyMemory<byte>", true)]
    public async Task CanMapToGuid(string type, bool isMemory)
    {
        // Arrange
        string sourceCode = $$"""
                              #nullable enable
                              using System;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Guid Map({{type}} input);
                              }
                              #nullable restore
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
                typeof(Guid).ToString(),
                NullableAnnotation.NotAnnotated,
                type,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(Guid).ToString(),
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                typeof(Guid).ToString(),
                                parameterAssertions =>
                                {
                                    if (isMemory)
                                    {
                                        parameterAssertions.BeMemberAccessExpressionSyntax("input.Span");
                                    }
                                    else
                                    {
                                        parameterAssertions.BeIdentifierNameSyntax("input");
                                    }
                                })))
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1");
                        }));
                });
    }
}