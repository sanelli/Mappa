// <copyright file="MappaBuilderContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// The content of the building.
/// </summary>
internal sealed class MappaBuilderContext
{
    private readonly StackSetting<string> compositeTypeSourceName = new(string.Empty);
    private readonly StackSetting<string> compositeTypeTargetName = new(string.Empty);
    private readonly StackSetting<MapMethod> mapMethodBeingBuilt = new(null!);
    private readonly List<Diagnostic> diagnostics = new();
    private uint temporaryCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaBuilderContext"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    public MappaBuilderContext(Compilation compilation)
    {
        this.Compilation = compilation;
    }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets any diagnostics reported while generating the code.
    /// </summary>
    internal IReadOnlyList<Diagnostic> Diagnostics => this.diagnostics;

    /// <summary>
    /// Gets the projection element methods queued during code generation.
    /// </summary>
    internal List<ProjectionElementMethodDefinition> ProjectionElementMethods { get; } = [];

    /// <summary>
    /// Gets a new unique temporary value.
    /// </summary>
    /// <returns>A new temporary value.</returns>
    internal string NextTemporary()
        => $"__mappa_tmp_{++this.temporaryCounter}";

    /// <summary>
    /// Push a new value for the source name for struct, record, classes, etc...
    /// </summary>
    /// <param name="sourceName">The name of the source.</param>
    /// <returns>Disposable value used to remove the source.</returns>
    internal IDisposable PushCurrentCompositeTypeSourceName(string sourceName)
        => this.compositeTypeSourceName.Apply(sourceName);

    /// <summary>
    /// Gets the current value of the source pushed.
    /// </summary>
    /// <returns>The current source.</returns>
    internal string GetCompositeTypeSourceName() => this.compositeTypeSourceName.CurrentValue;

    /// <summary>
    /// Push a new value for the target name for struct, record, classes, etc...
    /// </summary>
    /// <param name="sourceName">The name of the target.</param>
    /// <returns>Disposable value used to remove the target.</returns>
    internal IDisposable PushCurrentCompositeTypeTargetName(string sourceName)
        => this.compositeTypeTargetName.Apply(sourceName);

    /// <summary>
    /// Select which method is being built.
    /// </summary>
    /// <param name="mapMethod">The map method.</param>
    /// <returns>Disposable value used to remove the method from the stack.</returns>
    internal IDisposable PushMapMethod(MapMethod mapMethod)
        => this.mapMethodBeingBuilt.Apply(mapMethod);

    /// <summary>
    /// Gets the current value of the target pushed.
    /// </summary>
    /// <returns>The current source.</returns>
    internal string GetCompositeTypeTargetName() => this.compositeTypeTargetName.CurrentValue;

    /// <summary>
    /// Gets the current map method.
    /// </summary>
    /// <returns>The current map method.</returns>
    internal MapMethod GetMapMethod() => this.mapMethodBeingBuilt.CurrentValue
        ?? throw new MappaGeneratorException("Cannot obtain the map method.");

    /// <summary>
    /// Report a new diagnostic information.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to be reported.</param>
    internal void ReportDiagnostic(Diagnostic diagnostic) => this.diagnostics.Add(diagnostic);
}