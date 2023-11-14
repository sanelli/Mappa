// <copyright file="MappaGeneratorAbstractUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Models;

namespace Mappa.Generator.Tests.Abstractions;

/// <summary>
/// Base class with helper methods for running tests.
/// </summary>
public abstract class MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Run the generator on the input source.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    protected static Task<GeneratedResults> RunMappaGeneratorAsync(string source, CancellationToken cancellationToken)
        => RunMappaGeneratorAsync(new[] { source }, cancellationToken);

    /// <summary>
    /// Run the generator on the input sources.
    /// </summary>
    /// <param name="sources">The input sources.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    private static Task<GeneratedResults> RunMappaGeneratorAsync(
        IEnumerable<string> sources,
        CancellationToken cancellationToken)
    {
        var generator = new MappaGenerator();
        var compilation = BuildCompilation(sources);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics,
            cancellationToken);
        return Task.FromResult(new GeneratedResults(driver, outputCompilation, diagnostics.ToArray()));
    }

    /// <summary>
    /// Create a new compilation for the source generator.
    /// </summary>
    /// <param name="sources">The source generator.</param>
    /// <returns>The compilation.</returns>
    private static CSharpCompilation BuildCompilation(IEnumerable<string> sources)
    {
        var frameworkPath = Path.GetDirectoryName(typeof(Attribute).GetTypeInfo().Assembly.Location)!;
        var metadataReferences = new List<PortableExecutableReference>
        {
            MetadataReference.CreateFromFile(typeof(MappaAttribute).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Runtime.dll")),
        };

        var compilation = CSharpCompilation.Create(
            typeof(MappaGeneratorAbstractUnitTests).Assembly.FullName,
            sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            metadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation;
    }
}