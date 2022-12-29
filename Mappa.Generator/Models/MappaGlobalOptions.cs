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
///         <term><c>mappa.debug</c></term>
///         <description>Enabled the report of debugging messages when value is equal to <c>true</c>.</description>
///     </item>
/// </list>
/// </summary>
internal sealed class MappaGlobalOptions
{
    private const string MappaDebugFlagName = "debug";
    private const string MappaDebugCommentsFlagName = "debugcomments";

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGlobalOptions"/> class.
    /// </summary>
    /// <param name="analyzerConfigOptionsProvider">The analyzer configuration options.</param>
    /// <param name="syntaxTree">The syntax tree for which obtain the configuration.</param>
    public MappaGlobalOptions(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, SyntaxTree syntaxTree)
    {
        var options = analyzerConfigOptionsProvider.GetOptions(syntaxTree);

        this.MappaDebug = options.TryGetValue(GetOptionName(MappaDebugFlagName), out var mappaDebug)
                          && !string.IsNullOrWhiteSpace(mappaDebug)
                          && "true".Equals(mappaDebug, StringComparison.OrdinalIgnoreCase);

        this.MappaDebugComments =
            options.TryGetValue(GetOptionName(MappaDebugCommentsFlagName), out var mappaDebugComments)
            && !string.IsNullOrWhiteSpace(mappaDebugComments)
            && "true".Equals(mappaDebugComments, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets a value indicating whether to report debug INFO diagnostics.
    /// </summary>
    internal bool MappaDebug { get; }

    /// <summary>
    /// Gets a value indicating whether to report debug comments in the generated code.
    /// </summary>
    internal bool MappaDebugComments { get; }

    private static string GetOptionName(string name)
#pragma warning disable CA1308 // Normalize strings to uppercase
        => $"{nameof(Mappa)}.{name}".ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
}