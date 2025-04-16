// <copyright file="MappaProtobufDependencyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

using Mappa.Dependency.Protobuf;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="Mappa.Dependency.Protobuf.MappaProtobufMapper"/>.
/// </summary>
public sealed class MappaProtobufDependencyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that a mapping can be made from <see cref="DateTime"/> to <see cref="Timestamp"/>
    /// using <see cref="MappaProtobufMapper.MapFromDateTimeToTimestamp"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [Bug("#121")]
    [IntegrationTest]
    public async Task CanMapDateTimeToTimestamp()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source
                                  {
                                      public DateTime TimeStamp { get; set; }
                                  }

                                  #nullable disable
                                  public class Target(
                                  {
                                     public Google.Protobuf.WellKnownTypes.Timestamp TimeStamp { get; set; }
                                  }
                                  #nullable restore

                                  #nullable enable
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Mappa.Dependency.Protobuf.MappaProtobufMapper dependency = new Mappa.Dependency.Protobuf.MappaProtobufMapper();
                                      
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(DateTime).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.TimeStamp")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                    "Google.Protobuf.WellKnownTypes.Timestamp",
                                    "__mappa_tmp_2",
                                    assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                        "this.dependency.MapFromDateTimeToTimestamp",
                                        firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    ("TimeStamp", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test that a mapping can be made from <see cref="Timestamp"/> to <see cref="DateTime"/>
    /// using <see cref="MappaProtobufMapper.MapFromTimestampToDateTime"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [Bug("#121")]
    [IntegrationTest]
    public async Task CanMapTimestampToDateTime()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  #nullable disable
                                  public record Source
                                  {
                                      public Google.Protobuf.WellKnownTypes.Timestamp TimeStamp { get; set; }
                                  }
                                  #nullable restore
                                  
                                  public class Target(
                                  {
                                    public DateTime TimeStamp { get; set; }
                                  }

                                  #nullable enable
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Mappa.Dependency.Protobuf.MappaProtobufMapper dependency = new Mappa.Dependency.Protobuf.MappaProtobufMapper();
                                      
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Google.Protobuf.WellKnownTypes.Timestamp",
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.TimeStamp")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(DateTime).ToString(),
                                "__mappa_tmp_2",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                        "this.dependency.MapFromTimestampToDateTime",
                                        firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    ("TimeStamp", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }
}