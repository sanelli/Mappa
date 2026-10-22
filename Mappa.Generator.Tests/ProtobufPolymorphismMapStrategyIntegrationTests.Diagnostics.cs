// <copyright file="ProtobufPolymorphismMapStrategyIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="ProtobufPolymorphismMapStrategy"/> (diagnostics).
/// </summary>
public sealed partial class ProtobufPolymorphismMapStrategyIntegrationTests
{
    /// <summary>
    /// Protobuf polymorphism attributes cannot be combined with <see cref="MappaTypeMappingAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtobufAttributesAreMixedWithMappaTypeMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaTypeMapping(typeof(OkDto), typeof(OkDto))]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismMixedWithTypeMapping, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Protobuf polymorphism attributes cannot be combined with <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtobufAttributesAreMixedWithMappaTypeMappingDefault()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw)]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismMixedWithTypeMapping, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingAttribute"/> requires <see cref="MappaProtobufPolymorphismSetupAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtobufTypeMappingIsUsedWithoutSetup()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupMissing, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingDefaultAttribute"/> requires <see cref="MappaProtobufPolymorphismSetupAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtobufTypeMappingDefaultIsUsedWithoutSetup()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Throw)]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupMissing, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Exactly one <see cref="MappaProtobufPolymorphismSetupAttribute"/> is allowed.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMultipleProtobufPolymorphismSetupsAreDefined()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismMultipleSetups, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The single argument setup is ambiguous when both source and target are protobuf messages
    /// defining a property with the same name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenSingleArgumentSetupIsUsedForProtoToProtoMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("Description")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupWrongArity, "Map", "both 'Mappa.Generator.Tests.UnitTests.SourceCode.ResultMessage' and 'Mappa.Generator.Tests.UnitTests.SourceCode.EnvelopeMessage' define 'Description'; use [MappaProtobufPolymorphismSetup(targetOneOfCasePropertyName, sourceOneOfCasePropertyName)] for protobuf to protobuf mappings.")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The two arguments setup is for protobuf to protobuf mappings only.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupIsUsedForProtoToPocoMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupWrongArity, "Map", "only the source type defines a oneof case property; use [MappaProtobufPolymorphismSetup(oneOfCasePropertyName)] for protobuf to POCO or POCO to protobuf mappings.")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The two arguments setup reports both case properties when neither exists on the protobuf messages.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticsAreReturnedWhenBothCasePropertiesOfTwoArgumentsSetupDoNotExist()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("MissingTargetCase", "MissingSourceCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingSourceCase", ResultMessageType)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingTargetCase", EnvelopeMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property named in the setup must exist on the message type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyDoesNotExist()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("MissingCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingCase", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property must be typed with a nested <c>*OneofCase</c> enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyIsNotAOneofCaseEnum()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("Count")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "Count", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case name must be a member of the oneof case enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCaseNameIsUnknown()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Missing")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCaseNameUnknown, "Map", "Missing", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case <c>None</c> cannot be used as an explicit arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCaseNameIsNone()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "None")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCaseNameNone, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Only message-typed oneof arms are supported: integer arms are rejected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofArmIsAnIntegerScalar()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Count")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismScalarArm, "Map", "Count", ResultMessageType, "int")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Only message-typed oneof arms are supported: string arms are rejected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofArmIsAString()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Raw", typeof(OkDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismScalarArm, "Map", "Raw", EnvelopeMessageType, "string")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The same source oneof case cannot be mapped by more than one arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTheSameSourceOneofCaseIsMappedTwice()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMapping(typeof(ErrorDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDuplicateArm, "Map", "Ok")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The same CLR source type cannot be mapped by more than one arm.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTheSameSourceTypeIsMappedTwice()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMapping("Error", typeof(OkDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDuplicateArm, "Map", OkDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/> requires a protobuf target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenNoMapBehaviorIsUsedWithPocoTarget()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap)]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultRequiresProtobufTarget, "Map", "NoMap", ResultDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> requires a protobuf target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToBehaviorIsUsedWithPocoTarget()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultRequiresProtobufTarget, "Map", "MapTo", ResultDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> requires the target oneof case name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToBehaviorDoesNotSpecifyTheTargetCase()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo)]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismMapToCaseMissing, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> target oneof case must exist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToTargetCaseIsUnknown()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Missing")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCaseNameUnknown, "Map", "Missing", EnvelopeMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> target oneof case cannot be <c>None</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToTargetCaseIsNone()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "None")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCaseNameNone, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> target oneof case must be message-typed.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToTargetCaseIsScalar()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Raw")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismScalarArm, "Map", "Raw", EnvelopeMessageType, "string")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// <see cref="MappaProtobufTypeMappingDefaultBehavior.Undefined"/> is not a valid behavior.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenDefaultBehaviorIsUndefined()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Undefined)]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultBehaviorUndefined, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The type used with <see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/> must be an exception.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenThrowBehaviorTypeIsNotAnException()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Throw, typeof(UnrelatedDto))]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TypeMustBeAnException, "Mappa.Generator.Tests.UnitTests.SourceCode.UnrelatedDto")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The CLR target type of an arm must derive from the target type of the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenArmClrTypeDoesNotDeriveFromTheTargetType()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(UnrelatedDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismClrTypeNotCompatible, "Map", "Mappa.Generator.Tests.UnitTests.SourceCode.UnrelatedDto", ResultDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The target oneof arm property must be publicly writable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTargetArmPropertyIsNotWritable()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ChoiceCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      public partial ReadOnlyArmMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismArmPropertyNotWritable, "Map", "Ok", Namespace + ".ReadOnlyArmMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Namespace + ".ReadOnlyArmMessage",
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The constructor form of <see cref="MappaProtobufTypeMappingAttribute"/> must match the mapping scenario.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTypeMappingConstructorDoesNotMatchTheScenario()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismArmFormMismatch, "Map", "protobuf to POCO", "(Type targetType, string sourceOneOfCaseName)")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// When an arm cannot be mapped the algorithm reports that no strategy can be identified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenArmMessageCannotBeMapped()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Locked", typeof(OkDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, OkDtoType, Namespace + ".LockedMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// When the <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/> message cannot be mapped the algorithm
    /// reports that no strategy can be identified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMapToMessageCannotBeMapped()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Locked")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, ResultDtoType, Namespace + ".LockedMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The exception type is not used by <see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/>:
    /// a warning is reported and the code is still generated.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestWarningIsReturnedWhenNoMapBehaviorSpecifiesAnExceptionType()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMapping("Error", typeof(ErrorDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.NoMap, typeof(CustomException))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultArgumentUnused, "Map", "exception type", "NoMap")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                EnvelopeMessageType,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(EnvelopeMessageType)))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeSwitchStatementSyntax(
                            switchExpression => switchExpression.BeIdentifierNameSyntax("input"),
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(OkDtoType, "__mappa_tmp_2"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_3", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_4", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            OkReplyMessageType,
                                            "__mappa_tmp_5",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                OkReplyMessageType,
                                                ("Value", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Ok"), right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCasePattern();
                                labelAssertions[0]
                                    .AsCasePattern()
                                    .HasPattern(pattern => pattern.BeDeclarationPatternSyntax(ErrorDtoType, "__mappa_tmp_6"));

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(IntType, "__mappa_tmp_7", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Code")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(StringType, "__mappa_tmp_8", initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Description")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeLocalDeclarationStatementSyntax(
                                            ErrorReplyMessageType,
                                            "__mappa_tmp_9",
                                            expressionAssertions => expressionAssertions.BeObjectCreationExpressionSyntax(
                                                ErrorReplyMessageType,
                                                ("Code", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")),
                                                ("Description", propertyAssertions => propertyAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Error"), right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            },
                            (labelAssertions, caseBodyAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsDefault();

                                caseBodyAssertions.Should().HaveCount(1);
                                caseBodyAssertions[0].BeBlockStatement();
                                caseBodyAssertions[0]
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statementAssertions => statementAssertions.BeBreakStatement());
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// The constructor form of <see cref="MappaProtobufTypeMappingAttribute"/> must match the POCO to protobuf scenario.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenPocoToProtoTypeMappingUsesTheWrongConstructor()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismArmFormMismatch, "Map", "POCO to protobuf", "(string targetOneOfCaseName, Type sourceType)")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The constructor form of <see cref="MappaProtobufTypeMappingAttribute"/> must match the protobuf to protobuf scenario.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtoToProtoTypeMappingUsesTheWrongConstructor()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismArmFormMismatch, "Map", "protobuf to protobuf", "(string targetOneOfCaseName, string sourceOneOfCaseName)")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// When a protobuf to POCO arm cannot be mapped the algorithm reports that no strategy can be identified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtoToPocoArmMessageCannotBeMapped()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  public sealed class LockedDto : ResultDto { private LockedDto() { } }

                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping(typeof(LockedDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, OkMessageType, Namespace + ".LockedDto")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The source oneof case name of a protobuf to protobuf arm must be a member of the source oneof case enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtoToProtoSourceCaseIsUnknown()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Missing")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCaseNameUnknown, "Map", "Missing", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// When a protobuf to protobuf arm cannot be mapped the algorithm reports that no strategy can be identified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtoToProtoArmMessageCannotBeMapped()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Locked", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, OkMessageType, Namespace + ".LockedMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The source case property of a two arguments setup must be typed with a nested <c>*OneofCase</c> enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupSourceCasePropertyIsNotAOneofCaseEnum()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "Count")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "Count", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The target case property of a two arguments setup must be typed with a nested <c>*OneofCase</c> enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupTargetCasePropertyIsNotAOneofCaseEnum()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("Raw", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "Raw", EnvelopeMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The two arguments setup reports the source case property when it does not exist on the source protobuf message.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupSourceCasePropertyDoesNotExist()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "MissingSourceCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingSourceCase", ResultMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The two arguments setup reports the target case property when it does not exist on the target protobuf message.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupTargetCasePropertyDoesNotExist()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("MissingTargetCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingTargetCase", EnvelopeMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The two arguments setup is for protobuf to protobuf mappings only: a POCO source is reported as a wrong arity.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTwoArgumentsSetupIsUsedForPocoToProtoMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupWrongArity, "Map", "only the target type defines a oneof case property; use [MappaProtobufPolymorphismSetup(oneOfCasePropertyName)] for protobuf to POCO or POCO to protobuf mappings.")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property named in the setup must exist on the protobuf target when the source is a POCO.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyDoesNotExistOnTheProtobufTarget()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("MissingCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingCase", EnvelopeMessageType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The exception type is not used by <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>: a warning is reported, together with the missing target case error.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestWarningAndDiagnosticAreReturnedWhenMapToBehaviorSpecifiesAnExceptionTypeAndNoCase()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(OkDto))]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.MapTo, typeof(CustomException))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultArgumentUnused, "Map", "exception type", "MapTo")
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismMapToCaseMissing, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property must be typed with an enum whose name ends with <c>OneofCase</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyIsAnEnumNotNamedAsAOneofCase()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  public enum SomeKind { None = 0, A = 1 }
                                                  public sealed class PlainEnumMessage { public SomeKind KindCase { get; set; } }

                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("KindCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "A")]
                                                      public partial ResultDto Map(PlainEnumMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "KindCase", Namespace + ".PlainEnumMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                Namespace + ".PlainEnumMessage",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property must be typed with a <c>*OneofCase</c> enum nested in the message type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyIsAOneofCaseEnumNotNestedInTheMessage()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  public enum OuterOneofCase { None = 0, A = 1 }
                                                  public sealed class OuterEnumMessage { public OuterOneofCase KindCase { get; set; } }

                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("KindCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "A")]
                                                      public partial ResultDto Map(OuterEnumMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "KindCase", Namespace + ".OuterEnumMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                Namespace + ".OuterEnumMessage",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The CLR type of a POCO to protobuf arm must derive from the source type of the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenPocoToProtoArmClrTypeDoesNotDeriveFromTheSourceType()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase")]
                                                      [MappaProtobufTypeMapping("Ok", typeof(UnrelatedDto))]
                                                      public partial EnvelopeMessage Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismClrTypeNotCompatible, "Map", Namespace + ".UnrelatedDto", ResultDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The same source oneof case cannot be mapped twice in a protobuf to protobuf mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTheSameSourceOneofCaseIsMappedTwiceInProtoToProtoMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("PayloadCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      [MappaProtobufTypeMapping("Error", "Ok")]
                                                      public partial EnvelopeMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDuplicateArm, "Map", "Ok")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                EnvelopeMessageType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The target arm property of a protobuf to protobuf arm must be writable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenProtoToProtoTargetArmPropertyIsNotWritable()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ChoiceCase", "ResultCase")]
                                                      [MappaProtobufTypeMapping("Ok", "Ok")]
                                                      public partial ReadOnlyArmMessage Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismArmPropertyNotWritable, "Map", "Ok", Namespace + ".ReadOnlyArmMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Namespace + ".ReadOnlyArmMessage",
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property named in the setup is reported on the source type when neither type is a protobuf message.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyDoesNotExistOnPocoToPocoMapping()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("MissingCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial OkDto Map(ResultDto input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotFound, "Map", "MissingCase", ResultDtoType)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                OkDtoType,
                NullableAnnotation.NotAnnotated,
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// The oneof case property must be typed with a named enum type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenOneofCasePropertyIsAnArray()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  public sealed class ArrayCaseMessage { public int[] KindCase { get; set; } }

                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("KindCase")]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "A")]
                                                      public partial ResultDto Map(ArrayCaseMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismCasePropertyNotOneofCaseEnum, "Map", "KindCase", Namespace + ".ArrayCaseMessage")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                Namespace + ".ArrayCaseMessage",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// A setup attribute whose case property name cannot be read is ignored, so the setup is reported as missing.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTheSetupAttributeHasANullCasePropertyName()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup(null)]
                                                      [MappaProtobufTypeMapping(typeof(OkDto), "Ok")]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismSetupMissing, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Type mapping and default attributes with unreadable arguments are ignored: only the undefined default behavior is reported.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenMalformedTypeMappingAndDefaultAttributesAreIgnored()
    {
        // Arrange
        const string sourceCode = ProtobufStubs + """
                                                  [Mappa]
                                                  public sealed partial class Mapper
                                                  {
                                                      [MappaProtobufPolymorphismSetup("ResultCase")]
                                                      [MappaProtobufTypeMapping((string)null, "Ok")]
                                                      [MappaProtobufTypeMappingDefault(MappaProtobufTypeMappingDefaultBehavior.Undefined, (string)null)]
                                                      public partial ResultDto Map(ResultMessage input);
                                                  }
                                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProtobufPolymorphismDefaultBehaviorUndefined, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                ResultDtoType,
                NullableAnnotation.NotAnnotated,
                ResultMessageType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }
}