// <copyright file="FromReferenceNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="FromReferenceNullableMapStrategy"/>.
/// </summary>
public class FromReferenceNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from a
    /// nullable reference type to a non-nullable
    /// reference type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromReferenceNullable()
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
                public partial Target Map(Source? input);
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