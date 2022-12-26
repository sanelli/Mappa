// <copyright file="MappaGlobalOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator.Models;

/// <summary>
/// Global options of the mapper as read from the .editorconfig.
/// Values used are:
/// <list type="bullet">
///     <item>
///         <term><c>mappa_debug</c></term>
///         <description>Enabled the report of debugging messages when value is equal to <c>true</c>.</description>
///     </item>
/// </list>
/// </summary>
internal sealed class MappaGlobalOptions
{
    private const string MappaDebugFlagName = "mappa_debug";

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGlobalOptions"/> class.
    /// </summary>
    /// <param name="analyzerConfigOptionsProvider">The analyzer configuration options.</param>
    /// <param name="syntaxTree">The syntax tree for which obtain the configuration.</param>
    public MappaGlobalOptions(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, SyntaxTree syntaxTree)
    {
        var options = analyzerConfigOptionsProvider.GetOptions(syntaxTree);

        this.MappaDebug = options.TryGetValue(MappaDebugFlagName, out var mappaDebug)
                          && !string.IsNullOrWhiteSpace(mappaDebug)
                          && "true".Equals(mappaDebug, StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets a value indicating whether to report debug INFO diagnostics.
    /// </summary>
    internal bool MappaDebug { get; }
}