// <copyright file="AttributeLiteralEscapingIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text;

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for escaped attribute literals in generated mapping code.
/// </summary>
public sealed class AttributeLiteralEscapingIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Gets test data for string values containing special characters.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> SpecialCharacterStringTestData()
    {
        var data = new TheoryData<string, string>();
        data.Add("embeddedDoubleQuote", @"key""with""quote");
        data.Add("backslash", @"key\\with\\slash");
        data.Add("newline", "key\nwith\nnewline".Replace("\\n", "\n", StringComparison.Ordinal));
        return data;
    }

    /// <summary>
    /// Gets test data for char values containing special characters.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, char> SpecialCharacterCharTestData()
    {
        var data = new TheoryData<string, char>();
        data.Add("apostrophe", '\'');
        data.Add("backslash", '\\');
        data.Add("newline", '\n');
        return data;
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromConstantAttribute"/> escapes special characters in a string constant.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="constantValue">The constant value.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterStringTestData))]
    [IntegrationTest]
    public async Task CanAssignStringConstantWithSpecialCharactersViaMappaAssignFromConstantAttribute(string scenario, string constantValue)
    {
        ArgumentNullException.ThrowIfNull(constantValue);
        _ = scenario;

        var constantInAttribute = EscapeForCSharpStringAttribute(constantValue);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source { }
                           public class Target
                           {
                               public string Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), "{{constantInAttribute}}")]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeLiteralExpressionSyntax(constantValue));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromConstantAttribute"/> escapes special characters in a char constant.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="constantValue">The constant value.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterCharTestData))]
    [IntegrationTest]
    public async Task CanAssignCharConstantWithSpecialCharactersViaMappaAssignFromConstantAttribute(string scenario, char constantValue)
    {
        _ = scenario;

        var constantInAttribute = EscapeForCSharpCharAttribute(constantValue);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source { }
                           public class Target
                           {
                               public char Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), {{constantInAttribute}})]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(char).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeLiteralExpressionSyntax(constantValue));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromConstantAttribute"/> escapes special characters in a string array constant.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="constantValue">The constant value.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterStringTestData))]
    [IntegrationTest]
    public async Task CanAssignStringArrayConstantWithSpecialCharactersViaMappaAssignFromConstantAttribute(string scenario, string constantValue)
    {
        ArgumentNullException.ThrowIfNull(constantValue);
        _ = scenario;

        var constantInAttribute = EscapeForCSharpStringAttribute(constantValue);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source { }
                           public class Target
                           {
                               public string[] Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), new string[] { "{{constantInAttribute}}", "plain", "{{constantInAttribute}}" })]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string[]",
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeArrayCreationExpressionSyntax(
                                        "string",
                                        sizeAssertions => sizeAssertions.BeOmittedSizeExpressionSyntax(),
                                        firstElementAssertions => firstElementAssertions.BeLiteralExpressionSyntax(constantValue),
                                        secondElementAssertions => secondElementAssertions.BeLiteralExpressionSyntax("plain"),
                                        thirdElementAssertions => thirdElementAssertions.BeLiteralExpressionSyntax(constantValue));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromConstantAttribute"/> escapes special characters in a char array constant.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="constantValue">The constant value.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterCharTestData))]
    [IntegrationTest]
    public async Task CanAssignCharArrayConstantWithSpecialCharactersViaMappaAssignFromConstantAttribute(string scenario, char constantValue)
    {
        _ = scenario;

        var constantInAttribute = EscapeForCSharpCharAttribute(constantValue);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source { }
                           public class Target
                           {
                               public char[] Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), new char[] { {{constantInAttribute}}, 'a', {{constantInAttribute}} })]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "char[]",
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeArrayCreationExpressionSyntax(
                                        "char",
                                        sizeAssertions => sizeAssertions.BeOmittedSizeExpressionSyntax(),
                                        firstElementAssertions => firstElementAssertions.BeLiteralExpressionSyntax(constantValue),
                                        secondElementAssertions => secondElementAssertions.BeLiteralExpressionSyntax('a'),
                                        thirdElementAssertions => thirdElementAssertions.BeLiteralExpressionSyntax(constantValue));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromContextAttribute"/> escapes special characters in <c>ItemName</c>.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="itemName">The context item name.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterStringTestData))]
    [IntegrationTest]
    public async Task CanAssignFromContextWithSpecialCharacterItemName(string scenario, string itemName)
    {
        ArgumentNullException.ThrowIfNull(itemName);
        _ = scenario;

        var itemNameInAttribute = EscapeForCSharpStringAttribute(itemName);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source { }
                           public class Target
                           {
                               public string Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromContext(nameof(Target.Property), "{{itemNameInAttribute}}")]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                                    typeof(string).ToString(),
                                    castExpression => castExpression.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", itemName)));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignToContextAttribute"/> escapes special characters in <c>ContextKey</c>.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="contextKey">The context key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterStringTestData))]
    [IntegrationTest]
    public async Task CanAssignToContextWithSpecialCharacterContextKey(string scenario, string contextKey)
    {
        ArgumentNullException.ThrowIfNull(contextKey);
        _ = scenario;

        var contextKeyInAttribute = EscapeForCSharpStringAttribute(contextKey);

        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public class Source
                           {
                               public string Property { get; set; }
                           }

                           public class Target
                           {
                               public string Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignToContext("{{contextKeyInAttribute}}", nameof(Target.Property))]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
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
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeAssignToContextStatement("context", contextKey, "__mappa_tmp_2", "Property"))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    private static string EscapeForCSharpStringAttribute(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        return builder.ToString();
    }

    private static string EscapeForCSharpCharAttribute(char value)
    {
        return value switch
        {
            '\\' => "'\\\\'",
            '\'' => "'\\''",
            '\n' => "'\\n'",
            '\r' => "'\\r'",
            _ => $"'{value}'",
        };
    }
}