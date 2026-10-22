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
}