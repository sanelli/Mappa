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
                    // TODO [#121] Add assertions.
                });
    }
}