// <copyright file="MappaNullSubstituteInvokeMethodIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>[MappaNullSubstitute]</c> InvokeMethod mode.
/// </summary>
public sealed class MappaNullSubstituteInvokeMethodIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test InvokeMethod via <c>InvokeMethodType</c> emits a null-only conditional invoke before mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteNullSourceStringWithInvokeMethodOnType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class NullSubstituteHelpers
                                  {
                                      public static string WhenNull(string? value) => value ?? "Other";
                                  }

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), nameof(Source.Value), InvokeMethod = nameof(NullSubstituteHelpers.WhenNull), InvokeMethodType = typeof(NullSubstituteHelpers))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Assert
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
                                "string?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeConditionalExpressionSyntax(
                                    condition => condition.BeIsPatternExpressionSyntax(
                                        expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        pattern => pattern.BeConstantPatternSyntax(null)),
                                    whenTrue => whenTrue.BeInvocationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.NullSubstituteHelpers.WhenNull",
                                        parameter => parameter.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                    whenFalse => whenFalse.BeCastExpressionSyntax(
                                        typeof(string).ToString(),
                                        expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }
}