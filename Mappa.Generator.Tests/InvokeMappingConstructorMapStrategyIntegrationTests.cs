// <copyright file="InvokeMappingConstructorMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="InvokeMappingConstructorMapStrategy"/> strategy.
/// </summary>
public sealed class InvokeMappingConstructorMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping from two classes using the only
    /// existing mapping constructor on the target.
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

            public class Source
            {
                public Source(int sourceProperty)
                {
                    this.SourceProperty = sourceProperty;
                }

                public int SourceProperty { get; };
            }

            public class Target
            {
                public Target(Source source)
                {
                    this.TargetProperty = source.SourceProperty;
                }

                public int TargetProperty { get; };
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }

    /// <summary>
    /// Test a mapping from enum to class accepting
    /// as mapping constructor a different but compatible
    /// type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSingleMappingConstructorWithMappableParameter()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public enum Source
            {
                One,
                Two,
                Three,
            }

            public class Target
            {
                public Target(int source)
                {
                    this.TargetProperty = (Source)source;
                }

                public Source TargetProperty { get; };
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }

    /// <summary>
    /// Test a mapping from enum to class accepting
    /// as mapping constructor a different but compatible
    /// type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappingConstructorButOnlyOneMatchExactly()
    {
        // Arrange
        const string sourceCode = """
            using Mappa.Attributes;

            namespace Mappa.Generator.Tests.UnitTests.SourceCode;

            public enum Source
            {
                One,
                Two,
                Three,
            }

            public class Target
            {
                public Target(int source)
                {
                    this.TargetProperty = (Source)source;
                }

                public Target(Source source)
                {
                    this.TargetProperty = source;
                }

                public Source TargetProperty { get; };
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
            .HaveGeneratedOneSourceCode()
            .WithCompilationUnit();

        // TODO: Add correct assertions
        compilationUnitSyntaxAssertions.NotBeNull();
        #pragma warning restore
    }
}