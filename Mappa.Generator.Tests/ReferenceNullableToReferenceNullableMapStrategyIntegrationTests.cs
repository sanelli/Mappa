// <copyright file="ReferenceNullableToReferenceNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="ReferenceNullableToReferenceNullableMapStrategy"/>.
/// </summary>
public class ReferenceNullableToReferenceNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two nullable
    /// reference types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceNullableToReferenceNullable()
    {
        // Arrange
        const string sourceCode = """
            #nullable enable
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public class Source
            {
                public int PropertyA { get; set; }
            }

            public class Target
            {
                public int PropertyA { get; set; }
            }

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Target? Map(Source? input);
            }
            #nullable restore
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }
}