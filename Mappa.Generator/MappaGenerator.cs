// <copyright file="MappaGenerator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator;

/// <summary>
/// The mappa incremental generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class MappaGenerator
    : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}