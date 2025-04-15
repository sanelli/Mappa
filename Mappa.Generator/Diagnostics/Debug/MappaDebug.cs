// <copyright file="MappaDebug.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Diagnostics.Debug;

/// <summary>
/// Class used to report debug diagnostics.
/// </summary>
internal sealed class MappaDebug
{
    private readonly bool mappaDebug;
    private readonly Action<Diagnostic> reportDiagnostic;
    private readonly DiagnosticDescriptor debugDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaDebug"/> class.
    /// </summary>
    /// <param name="mappaGlobalOptions">The global options.</param>
    /// <param name="reportDiagnostic">Callback to report a diagnostic.</param>
    public MappaDebug(MappaGlobalOptions mappaGlobalOptions, Action<Diagnostic> reportDiagnostic)
    {
        this.mappaDebug = mappaGlobalOptions.MappaDebug;
        this.reportDiagnostic = reportDiagnostic;
        this.debugDescriptor = new DiagnosticDescriptor(
            MappaDiagnosticsKind.Debug.ToDiagnosticId(),
            "Mappa Debug",
            "Mappa debug: {0}",
            "Mappa.Generator.Debug",
            this.mappaDebug ? DiagnosticSeverity.Info : DiagnosticSeverity.Hidden,
            this.mappaDebug);
    }

    /// <summary>
    /// Diagnostic to report a debug diagnostic message.
    /// </summary>
    /// <param name="message">The message to be reported.</param>
    /// <param name="syntaxNode">The syntax node this message is referenced to.</param>
    internal void Debug(string message, CSharpSyntaxNode? syntaxNode)
    {
        if (!this.mappaDebug)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            this.debugDescriptor,
            syntaxNode?.GetLocation(),
            message);

        this.reportDiagnostic(diagnostic);
    }
}