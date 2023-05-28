// <copyright file="InvokeConstructorMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="InvokeConstructorMapStrategy"/>.
/// </summary>
public sealed class InvokeConstructorMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping from two classes can happen using the
    /// constructor with no arguments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSingleMappingConstructor()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public enum TestEnum
            {
                One,
                Two,
                Three,
            }

            public class Source
            {
                public int PropertyA { get; set; }
                public TestEnum PropertyB { get; set; }
            }

            public class Target
            {
                public string PropertyA { get; set; }
                public int PropertyB { get; set; }
            }

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Target Map(Source input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }

    /// <summary>
    /// Test a mapping from property requiring
    /// the empty constructor strategy as well.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSingleMappingConstructorWithClassesAsProperties()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public enum TestEnum
            {
                One,
                Two,
                Three,
            }

            public class InnerSource
            {
                public int PropertyA { get; set; }
                public TestEnum PropertyB { get; set; }
            }

            public class InnerTarget
            {
                public string PropertyA { get; set; }
                public int PropertyB { get; set; }
            }

            public class Source
            {
                public InnerSource Property { get; }
            }

            public class Target
            {
                public InnerTarget Property { set; }
            }

            [Mappa]
            public sealed partial class Mapper
            {
                public partial Target Map(Source input);
            }
            """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        #pragma warning disable
        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }
}