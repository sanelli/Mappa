// <copyright file="MappaBuilderContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Helpers;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// The content of the building.
/// </summary>
internal sealed class MappaBuilderContext
{
    private readonly StackSetting<string> compoundSource = new(string.Empty);
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
    /// Gets a new unique temporary value.
    /// </summary>
    /// <returns>A new temporary value.</returns>
    internal string NextTemporary()
        => $"__mappa_tmp_{++this.temporaryCounter}";

    /// <summary>
    /// Push a new value for the source name for struct, record, classes, etc...
    /// </summary>
    /// <param name="source">The name of the source.</param>
    /// <returns>Disposable value used to remove the source.</returns>
    internal IDisposable PushSource(string source)
        => this.compoundSource.Apply(source);

    /// <summary>
    /// Gets the current value of the source pushed.
    /// </summary>
    /// <returns>The current source.</returns>
    internal string GetSource() => this.compoundSource.CurrentValue;
}