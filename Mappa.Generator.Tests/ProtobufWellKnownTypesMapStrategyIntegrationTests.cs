// <copyright file="ProtobufWellKnownTypesMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for Protobuf well-known type mapping strategies.
/// </summary>
public sealed partial class ProtobufWellKnownTypesMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string TimestampTypeName = "Google.Protobuf.WellKnownTypes.Timestamp";
    private const string DurationTypeName = "Google.Protobuf.WellKnownTypes.Duration";
    private const string FromDateTimeMember = "global::Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime";
    private const string FromDateTimeOffsetMember = "global::Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset";
    private const string FromTimeSpanMember = "global::Google.Protobuf.WellKnownTypes.Duration.FromTimeSpan";

    /// <summary>
    /// Maps root <see cref="DateTime"/> to <see cref="Timestamp"/> via <c>Timestamp.FromDateTime</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDateTimeToTimestampWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Timestamp Map(DateTime input);
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
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(DateTime).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TimestampTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromDateTimeMember,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Timestamp"/> to <see cref="DateTime"/> via <c>ToDateTime</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTimestampToDateTimeWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateTime Map(Google.Protobuf.WellKnownTypes.Timestamp input);
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
                typeof(DateTime).ToString(),
                NullableAnnotation.NotAnnotated,
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(DateTime).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "input.ToDateTime")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps nested record <see cref="DateTime"/> property to <see cref="Timestamp"/> inline (no dependency mapper).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#121")]
    [IntegrationTest]
    public async Task CanMapNestedDateTimePropertyToTimestampWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source
                                  {
                                      public DateTime TimeStamp { get; set; }
                                  }

                                  #nullable disable
                                  public class Target
                                  {
                                     public Google.Protobuf.WellKnownTypes.Timestamp TimeStamp { get; set; }
                                  }
                                  #nullable restore

                                  #nullable enable
                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
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
                                TimestampTypeName,
                                "__mappa_tmp_2",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromDateTimeMember,
                                    parameter => parameter.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    ("TimeStamp", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Maps root <see cref="TimeSpan"/> to <see cref="Duration"/> via <c>Duration.FromTimeSpan</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTimeSpanToDurationWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Duration Map(TimeSpan input);
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
                DurationTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(TimeSpan).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                DurationTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromTimeSpanMember,
                                    parameter => parameter.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Duration"/> to <see cref="TimeSpan"/> via <c>ToTimeSpan</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDurationToTimeSpanWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TimeSpan Map(Google.Protobuf.WellKnownTypes.Duration input);
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
                typeof(TimeSpan).ToString(),
                NullableAnnotation.NotAnnotated,
                DurationTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(TimeSpan).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "input.ToTimeSpan")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="DateTimeOffset"/> to <see cref="Timestamp"/> via <c>FromDateTimeOffset</c> with <c>ToUniversalTime</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDateTimeOffsetToTimestampWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Timestamp Map(DateTimeOffset input);
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
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(DateTimeOffset).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TimestampTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromDateTimeOffsetMember,
                                    parameter => parameter.BeInvocationExpressionSyntax("input.ToUniversalTime"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Timestamp"/> to <see cref="DateTimeOffset"/> via <c>ToDateTimeOffset</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTimestampToDateTimeOffsetWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateTimeOffset Map(Google.Protobuf.WellKnownTypes.Timestamp input);
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
                typeof(DateTimeOffset).ToString(),
                NullableAnnotation.NotAnnotated,
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(DateTimeOffset).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "input.ToDateTimeOffset")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="DateOnly"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDateOnlyToTimestampWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Timestamp Map(DateOnly input);
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
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(DateOnly).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TimestampTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    FromDateTimeMember,
                                    parameter => parameter.BeInvocationExpressionSyntax(
                                        "input.ToDateTime(System.TimeOnly.MinValue, System.DateTimeKind.Utc).ToUniversalTime"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Timestamp"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTimestampToDateOnlyWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateOnly Map(Google.Protobuf.WellKnownTypes.Timestamp input);
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
                typeof(DateOnly).ToString(),
                NullableAnnotation.NotAnnotated,
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(DateOnly).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "System.DateOnly.FromDateTime",
                                    parameter => parameter.BeInvocationExpressionSyntax("input.ToDateTime"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="Timestamp"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTimestampToTimeOnlyWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TimeOnly Map(Google.Protobuf.WellKnownTypes.Timestamp input);
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
                TimestampTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(TimeOnly).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax(
                                    "System.TimeOnly.FromDateTime",
                                    parameter => parameter.BeInvocationExpressionSyntax("input.ToDateTime"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// When <c>ProtobufWellKnownTypes</c> is unset, <see cref="DateTime"/> to <see cref="Timestamp"/> cannot be mapped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDateTimeToTimestampWhenProtobufWellKnownTypesUndefined()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Timestamp Map(DateTime input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                typeof(DateTime).ToString(),
                TimestampTypeName);
    }

    /// <summary>
    /// When <c>ProtobufWellKnownTypes</c> is disabled, <see cref="DateTime"/> to <see cref="Timestamp"/> cannot be mapped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDateTimeToTimestampWhenProtobufWellKnownTypesDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Timestamp Map(DateTime input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                typeof(DateTime).ToString(),
                TimestampTypeName);
    }
}