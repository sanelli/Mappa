// <copyright file="IdentityStrategyUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Tests;

/// <summary>
/// Tests related to the identity strategy.
/// </summary>
public sealed class IdentityStrategyUnitTest
    : Abstractions.MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from reference type
    /// to <see cref="object"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceTypeToObjectWhenNullableDisabled()
    {
        // Arrange
        var sourceCode = """
            #nullable disable
            using Mappa.Attributes;

            namespace Mappa.Tests.UnitTests.SourceCode;

            [Mappa]
            public sealed partial class Mapper
            {
                public partial object Map(string input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedOneSourceCode();
    }
}