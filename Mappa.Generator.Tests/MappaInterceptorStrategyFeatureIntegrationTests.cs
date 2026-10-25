// <copyright file="MappaInterceptorStrategyFeatureIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Happy-path interceptor coverage for the main automatic mapping strategies.
/// </summary>
public sealed class MappaInterceptorStrategyFeatureIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Nullable value-type unwrap works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptNullableToNonNullable()
    {
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public long Map(int? value) => this.MappaMap<long>(value);
                                  }
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "long",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int?",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("long", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("__mappaTypedSource.HasValue"),
                                thenStatement =>
                                {
                                    thenStatement
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(thenNode =>
                                        {
                                            thenNode.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_2",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Value"));
                                        })
                                        .HasNextSyntaxNode(thenNode =>
                                        {
                                            thenNode.BeAssignmentExpressionStatement(
                                                left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                },
                                elseStatement =>
                                {
                                    elseStatement
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(elseNode =>
                                        {
                                            elseNode.BeThrowStatementSyntax<NullReferenceException>(
                                                arg => arg.BeLiteralExpressionSyntax("\"__mappaTypedSource\" is null."));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Enum-to-enum mapping works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptEnumToEnum()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum SourceEnum { One, Two }

                                  public enum TargetEnum { One, Two }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public TargetEnum Map(SourceEnum value) => this.MappaMap<TargetEnum>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetEnum",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceEnum",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.SourceEnum",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetEnum",
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeSwitchStatementSyntax(
                                switchExpression => switchExpression.BeIdentifierNameSyntax("__mappaTypedSource"),
                                (labels, statements) =>
                                {
                                    labels.Should().HaveCount(1);
                                    labels[0].IsCase();
                                    statements.Should().HaveCount(1);
                                },
                                (labels, statements) =>
                                {
                                    labels.Should().HaveCount(1);
                                    labels[0].IsCase();
                                    statements.Should().HaveCount(1);
                                },
                                (labels, statements) =>
                                {
                                    labels.Should().HaveCount(1);
                                    labels[0].IsDefault();
                                    statements.Should().HaveCount(1);
                                });
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// String-to-numeric parse works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptStringToInt()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(string value) => this.MappaMap<int>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "string?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "string",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_1",
                                init => init.BeInvocationExpressionSyntax(
                                    "int.Parse",
                                    arg => arg.BeIdentifierNameSyntax("__mappaTypedSource")));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Date/time conversion works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptLongToDateTimeOffset()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public DateTimeOffset Map(long value) => this.MappaMap<DateTimeOffset>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                typeof(DateTimeOffset).ToString(),
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "long",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "long",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(DateTimeOffset).ToString(),
                                "__mappa_tmp_1",
                                init => init.BeInvocationExpressionSyntax(
                                    "System.DateTimeOffset.FromUnixTimeSeconds",
                                    arg => arg.BeIdentifierNameSyntax("__mappaTypedSource")));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Collection-to-collection mapping works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptCollectionToCollection()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public List<string> Map(List<int> value) => this.MappaMap<List<string>>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                typeof(List<string>).ToString(),
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(List<int>).ToString() + "?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    typeof(List<int>).ToString(),
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    arg => arg.BeMemberAccessExpressionSyntax("__mappaTypedSource.Count")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeForStatementSyntax(
                                declaration => declaration.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                condition => condition.BeBinaryExpressionSyntax(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    right => right.BeMemberAccessExpressionSyntax("__mappaTypedSource.Count")),
                                incrementor => incrementor.BePrefixUnaryExpressionSyntax(
                                    SyntaxKind.PlusPlusToken,
                                    operand => operand.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statement =>
                                {
                                    statement
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                init => init.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax(
                                                    "__mappaTypedSource",
                                                    "__mappa_tmp_2"));
                                        })
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                init => init.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString"));
                                        })
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Dictionary-to-dictionary mapping works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptDictionaryToDictionary()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Dictionary<string, long> Map(Dictionary<string, int> value)
                                          => this.MappaMap<Dictionary<string, long>>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                typeof(Dictionary<string, long>).ToString(),
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<string, int>).ToString() + "?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    typeof(Dictionary<string, int>).ToString(),
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<string, long>).ToString(),
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(typeof(Dictionary<string, long>).ToString()));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeForEachStatementSyntax(
                                typeof(KeyValuePair<string, int>).ToString(),
                                "__mappa_tmp_2",
                                expression => expression.BeIdentifierNameSyntax("__mappaTypedSource"),
                                statement =>
                                {
                                    statement
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Key"));
                                        })
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value"));
                                        })
                                        .HasNextSyntaxNode(inner =>
                                        {
                                            inner.BeAssignmentExpressionStatement(
                                                left => left.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Tuple-to-tuple mapping works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptTupleToTuple()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Tuple<string, string> Map(Tuple<int, string> value)
                                          => this.MappaMap<Tuple<string, string>>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                typeof(Tuple<string, string>).ToString(),
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(Tuple<int, string>).ToString() + "?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    typeof(Tuple<int, string>).ToString(),
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_1",
                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Item1"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_2",
                                init => init.BeInvocationExpressionSyntax("__mappa_tmp_1.ToString"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_3",
                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Item2"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(Tuple<string, string>).ToString(),
                                "__mappa_tmp_4",
                                init => init.BeObjectCreationExpressionSyntax(
                                    typeof(Tuple<string, string>).ToString(),
                                    [
                                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    ],
                                    []));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Guid strategy works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptGuidToByteArray()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public byte[] Map(Guid value) => this.MappaMap<byte[]>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "byte[]",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                typeof(Guid).ToString(),
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    typeof(Guid).ToString(),
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "byte[]",
                                "__mappa_tmp_1",
                                init => init.BeInvocationExpressionSyntax("__mappaTypedSource.ToByteArray"));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Structured empty-constructor property mapping works on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptStructuredPropertyMapping()
    {
        const string sourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
        const string targetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value) => this.MappaMap<Target>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                targetType,
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                sourceType + "?",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    sourceType,
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_1",
                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Value"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                targetType,
                                "__mappa_tmp_2",
                                init => init.BeObjectCreationExpressionSyntax(
                                    targetType,
                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }
}