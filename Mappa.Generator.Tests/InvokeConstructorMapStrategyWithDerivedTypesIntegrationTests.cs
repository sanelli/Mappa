// <copyright file="InvokeConstructorMapStrategyWithDerivedTypesIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Class to test mapping between structured types works
/// on derived classes.
/// </summary>
public sealed class InvokeConstructorMapStrategyWithDerivedTypesIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test for bug <a href="https://github.com/sanelli/Mappa/issues/153">#153</a>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#153")]
    [IntegrationTest]
    public async Task CanMapUsingSingleEmptyMappingConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed record RequestTrace(string RequestId);
                                  public abstract record BaseResponse(RequestTrace RequestTrace);
                                  public abstract record BaseRequest(RequestTrace RequestTrace);

                                  public sealed record Response(RequestTrace RequestTrace)
                                    : BaseResponse(RequestTrace);
                                  public sealed record Request(RequestTrace RequestTrace)
                                    : BaseRequest(RequestTrace);
                                    
                                  [Mappa]
                                  public static partial class Mapper {
                                      internal static partial Response ToResponse(this Request request);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Response",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Request",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                });
    }
}