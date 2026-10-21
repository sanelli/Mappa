// <copyright file="ProtobufWellKnownTypesMapStrategyIntegrationTests.Wrappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for Protobuf well-known scalar wrapper mapping strategies.
/// </summary>
public sealed partial class ProtobufWellKnownTypesMapStrategyIntegrationTests
{
    private const string StringValueTypeName = "Google.Protobuf.WellKnownTypes.StringValue";
    private const string BoolValueTypeName = "Google.Protobuf.WellKnownTypes.BoolValue";
    private const string BytesValueTypeName = "Google.Protobuf.WellKnownTypes.BytesValue";
    private const string Int32ValueTypeName = "Google.Protobuf.WellKnownTypes.Int32Value";
    private const string Int64ValueTypeName = "Google.Protobuf.WellKnownTypes.Int64Value";
    private const string UInt32ValueTypeName = "Google.Protobuf.WellKnownTypes.UInt32Value";
    private const string UInt64ValueTypeName = "Google.Protobuf.WellKnownTypes.UInt64Value";
    private const string FloatValueTypeName = "Google.Protobuf.WellKnownTypes.FloatValue";
    private const string DoubleValueTypeName = "Google.Protobuf.WellKnownTypes.DoubleValue";
    private const string ByteStringCopyFromMember = "global::Google.Protobuf.ByteString.CopyFrom";

    /// <summary>
    /// Maps root <see cref="string"/> to <c>StringValue</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToStringValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.StringValue Map(string input);
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
                StringValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                StringValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    StringValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>StringValue</c> to <see cref="string"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringValueToStringWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(Google.Protobuf.WellKnownTypes.StringValue input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                StringValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="bool"/> to <c>BoolValue</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBoolToBoolValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.BoolValue Map(bool input);
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
                BoolValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(bool).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                BoolValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    BoolValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>BoolValue</c> to <see cref="bool"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBoolValueToBoolWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial bool Map(Google.Protobuf.WellKnownTypes.BoolValue input);
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
                typeof(bool).ToString(),
                NullableAnnotation.NotAnnotated,
                BoolValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(bool).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>byte[]</c> to <c>BytesValue</c> via <c>ByteString.CopyFrom</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapByteArrayToBytesValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.BytesValue Map(byte[] input);
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
                BytesValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(byte[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                BytesValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    BytesValueTypeName,
                                    ("Value", a => a.BeInvocationExpressionSyntax(ByteStringCopyFromMember, parameter => parameter.BeIdentifierNameSyntax("input"))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>BytesValue</c> to <c>byte[]</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapBytesValueToByteArrayWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial byte[] Map(Google.Protobuf.WellKnownTypes.BytesValue input);
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
                typeof(byte[]).ToString(),
                NullableAnnotation.NotAnnotated,
                BytesValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(byte[]).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeInvocationExpressionSyntax("input.Value.ToByteArray")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="int"/> to <c>Int32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIntToInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int32Value Map(int input);
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
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Int32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Int32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>Int32Value</c> to <see cref="long"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapInt32ValueToLongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(Google.Protobuf.WellKnownTypes.Int32Value input);
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
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>Int32Value</c> to <see cref="ulong"/> with an explicit cast.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapInt32ValueToULongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial ulong Map(Google.Protobuf.WellKnownTypes.Int32Value input);
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
                typeof(ulong).ToString(),
                NullableAnnotation.NotAnnotated,
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ulong).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeCastExpressionSyntax(
                                    "ulong",
                                    expression => expression.BeMemberAccessExpressionSyntax("input.Value"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="long"/> to <c>Int64Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapLongToInt64ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int64Value Map(long input);
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
                Int64ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Int64ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Int64ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="uint"/> to <c>UInt32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUIntToUInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.UInt32Value Map(uint input);
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
                UInt32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(uint).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                UInt32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    UInt32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="ulong"/> to <c>UInt64Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapULongToUInt64ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.UInt64Value Map(ulong input);
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
                UInt64ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(ulong).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                UInt64ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    UInt64ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="float"/> to <c>FloatValue</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFloatToFloatValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.FloatValue Map(float input);
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
                FloatValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(float).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                FloatValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    FloatValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>FloatValue</c> to <see cref="double"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFloatValueToDoubleWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial double Map(Google.Protobuf.WellKnownTypes.FloatValue input);
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
                typeof(double).ToString(),
                NullableAnnotation.NotAnnotated,
                FloatValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(double).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="double"/> to <c>DoubleValue</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDoubleToDoubleValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.DoubleValue Map(double input);
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
                DoubleValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(double).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                DoubleValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    DoubleValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>Int32Value</c> to <see cref="int"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapInt32ValueToIntWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(Google.Protobuf.WellKnownTypes.Int32Value input);
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
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>byte</c> to <c>Int32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapByteToInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int32Value Map(byte input);
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
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(byte).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Int32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Int32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>sbyte</c> to <c>Int32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapSByteToInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int32Value Map(sbyte input);
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
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(sbyte).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Int32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Int32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>short</c> to <c>Int32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapShortToInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int32Value Map(short input);
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
                Int32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(short).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                Int32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    Int32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <see cref="float"/> to <c>DoubleValue</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFloatToDoubleValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.DoubleValue Map(float input);
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
                DoubleValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(float).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                DoubleValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    DoubleValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// When <c>ProtobufWellKnownTypes</c> is enabled, <see cref="long"/> to <c>Int32Value</c> cannot use the wrapper strategy
    /// (source type is outside the Int32Value widening set).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapLongToInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.Int32Value Map(long input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "long",
                Int32ValueTypeName);
    }

    /// <summary>
    /// Maps root <c>Int64Value</c> to <see cref="long"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapInt64ValueToLongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(Google.Protobuf.WellKnownTypes.Int64Value input);
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
                Int64ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>UInt32Value</c> to <see cref="uint"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUInt32ValueToUIntWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial uint Map(Google.Protobuf.WellKnownTypes.UInt32Value input);
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
                typeof(uint).ToString(),
                NullableAnnotation.NotAnnotated,
                UInt32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(uint).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>UInt32Value</c> to <see cref="long"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUInt32ValueToLongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(Google.Protobuf.WellKnownTypes.UInt32Value input);
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
                UInt32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>UInt32Value</c> to <see cref="ulong"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUInt32ValueToULongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial ulong Map(Google.Protobuf.WellKnownTypes.UInt32Value input);
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
                typeof(ulong).ToString(),
                NullableAnnotation.NotAnnotated,
                UInt32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ulong).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>ushort</c> to <c>UInt32Value</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUShortToUInt32ValueWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.UInt32Value Map(ushort input);
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
                UInt32ValueTypeName,
                NullableAnnotation.NotAnnotated,
                typeof(ushort).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                UInt32ValueTypeName,
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeObjectCreationExpressionSyntax(
                                    UInt32ValueTypeName,
                                    ("Value", a => a.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>UInt64Value</c> to <see cref="ulong"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUInt64ValueToULongWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial ulong Map(Google.Protobuf.WellKnownTypes.UInt64Value input);
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
                typeof(ulong).ToString(),
                NullableAnnotation.NotAnnotated,
                UInt64ValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ulong).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>FloatValue</c> to <see cref="float"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFloatValueToFloatWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial float Map(Google.Protobuf.WellKnownTypes.FloatValue input);
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
                typeof(float).ToString(),
                NullableAnnotation.NotAnnotated,
                FloatValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(float).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps root <c>DoubleValue</c> to <see cref="double"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDoubleValueToDoubleWhenProtobufWellKnownTypesEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial double Map(Google.Protobuf.WellKnownTypes.DoubleValue input);
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
                typeof(double).ToString(),
                NullableAnnotation.NotAnnotated,
                DoubleValueTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(double).ToString(),
                                "__mappa_tmp_1",
                                assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// When <c>ProtobufWellKnownTypes</c> is disabled, <see cref="string"/> to <c>StringValue</c> cannot be mapped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapStringToStringValueWhenProtobufWellKnownTypesDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Google.Protobuf.WellKnownTypes.StringValue Map(string input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "string",
                StringValueTypeName);
    }
}