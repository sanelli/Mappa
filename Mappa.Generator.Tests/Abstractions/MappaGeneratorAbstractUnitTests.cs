// <copyright file="MappaGeneratorAbstractUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

using Mappa.Attributes;
using Mappa.Dependency.Bson;
using Mappa.Dependency.Protobuf;
using Mappa.Generator.Tests.Helpers;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

namespace Mappa.Generator.Tests.Abstractions;

/// <summary>
/// Base class with helper methods for running tests.
/// </summary>
// TODO [#43] Extract to its own project in a different solution/repo.
 #pragma warning disable CA1515
public abstract class MappaGeneratorAbstractUnitTests
 #pragma warning restore CA1515
{
    private const string SourceFilePath = "/Source.cs";

    /// <summary>
    /// Run the generator on the input source.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    protected static Task<GeneratedResults> RunMappaGeneratorAsync(string source, CancellationToken cancellationToken)
        => RunMappaGeneratorAsync(source, null, LanguageVersion.Default, cancellationToken);

    /// <summary>
    /// Run the generator on the input source with an optional <c>.editorconfig</c>.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="editorConfig">The optional <c>.editorconfig</c> content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    protected static Task<GeneratedResults> RunMappaGeneratorAsync(string source, string? editorConfig, CancellationToken cancellationToken)
        => RunMappaGeneratorAsync(source, editorConfig, LanguageVersion.Default, cancellationToken);

    /// <summary>
    /// Run the generator on the input source with a specific language version.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="languageVersion">The C# language version for the compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    protected static Task<GeneratedResults> RunMappaGeneratorAsync(
        string source,
        LanguageVersion languageVersion,
        CancellationToken cancellationToken)
        => RunMappaGeneratorAsync(source, null, languageVersion, cancellationToken);

    /// <summary>
    /// Run the generator on the input source with an optional <c>.editorconfig</c>
    /// and a specific language version.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="editorConfig">The optional <c>.editorconfig</c> content.</param>
    /// <param name="languageVersion">The C# language version for the compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The driver, the cancellation output and the diagnostics.</returns>
    protected static Task<GeneratedResults> RunMappaGeneratorAsync(
        string source,
        string? editorConfig,
        LanguageVersion languageVersion,
        CancellationToken cancellationToken)
    {
        var generator = new MappaGenerator();
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(languageVersion);
        var compilation = BuildCompilation(source, parseOptions);
        AnalyzerConfigOptionsProvider? optionsProvider = editorConfig is null
            ? null
            : TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [generator.AsSourceGenerator()],
            additionalTexts: null,
            parseOptions: parseOptions,
            optionsProvider: optionsProvider);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics,
            cancellationToken);
        GeneratedSourceDumpHelper.TryDumpGeneratedSources(driver);
        return Task.FromResult(new GeneratedResults(driver, outputCompilation, diagnostics.ToArray()));
    }

    /// <summary>
    /// Create a new compilation for the source generator.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <returns>The compilation.</returns>
    protected static CSharpCompilation BuildCompilation(string source)
        => BuildCompilation(source, CSharpParseOptions.Default);

    /// <summary>
    /// Create a new compilation for the source generator.
    /// </summary>
    /// <param name="source">The input source code.</param>
    /// <param name="parseOptions">The parse options.</param>
    /// <returns>The compilation.</returns>
    protected static CSharpCompilation BuildCompilation(string source, CSharpParseOptions parseOptions)
    {
        var frameworkPath = Path.GetDirectoryName(typeof(Attribute).GetTypeInfo().Assembly.Location)!;
        var dateTimeStylesAssembly = typeof(MappaSettingsAttribute).GetProperty(nameof(MappaSettingsAttribute.DateTimeStyle))!.PropertyType.Assembly;
        var descriptionAttributeAssembly = typeof(System.ComponentModel.DescriptionAttribute).Assembly;
        var metadataReferences = new List<PortableExecutableReference>
        {
            MetadataReference.CreateFromFile(typeof(MappaAttribute).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MappaProtobufMapper).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MappaBsonMapper).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Timestamp).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ObjectId).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(dateTimeStylesAssembly.Location),
            MetadataReference.CreateFromFile(descriptionAttributeAssembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Collections.Immutable.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Collections.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Collections.Concurrent.dll")),
            MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
        };

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        return CSharpCompilation.Create(
            typeof(MappaGeneratorAbstractUnitTests).Assembly.FullName,
            [CSharpSyntaxTree.ParseText(source, parseOptions, SourceFilePath)],
            metadataReferences,
            compilationOptions);
    }
}