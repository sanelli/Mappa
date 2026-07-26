// <copyright file="ProjectionCapabilityAnalysisContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Context used while analyzing queryable projection capabilities.
/// </summary>
internal sealed class ProjectionCapabilityAnalysisContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCapabilityAnalysisContext"/> class.
    /// </summary>
    /// <param name="algorithmContext">The map algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="methodName">The projection map method name.</param>
    /// <param name="location">The projection map method location.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal ProjectionCapabilityAnalysisContext(
        MappaMapAlgorithmContext algorithmContext,
        Compilation compilation,
        string methodName,
        Location? location,
        CancellationToken cancellationToken)
    {
        this.AlgorithmContext = algorithmContext;
        this.Compilation = compilation;
        this.MethodName = methodName;
        this.Location = location;
        this.CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the map algorithm context.
    /// </summary>
    internal MappaMapAlgorithmContext AlgorithmContext { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    internal CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the projection map method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the projection map method location.
    /// </summary>
    internal Location? Location { get; }
}