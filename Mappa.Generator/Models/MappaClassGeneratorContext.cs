// <copyright file="MappaClassGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// The context of the generator for a single class.
/// </summary>
internal sealed class MappaClassGeneratorContext
{
    private readonly List<MapMethod> mapMethods = new();
    private readonly List<Diagnostic> diagnostics = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaClassGeneratorContext"/> class.
    /// </summary>
    /// <param name="options">The mappa global options.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    public MappaClassGeneratorContext(
        MappaGlobalOptions options,
        Compilation compilation,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        this.Options = options;
        this.Compilation = compilation;
        this.ClassDeclarationSyntax = classDeclarationSyntax;
        this.SemanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
    }

    /// <summary>
    /// Gets the global mappa options.
    /// </summary>
    internal MappaGlobalOptions Options { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    internal SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the class declarationsyntax of the mapper class begin processed.
    /// </summary>
    internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }

    /// <summary>
    /// Gets the list of map methods associated to class defined by <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    internal IReadOnlyCollection<MapMethod> MapMethods => this.mapMethods;

    /// <summary>
    /// Gets the diagnostics associated with the mapping.
    /// </summary>
    internal IReadOnlyCollection<Diagnostic> Diagnostics => this.diagnostics;

    /// <summary>
    /// Try getting the method for mappung from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <returns><c>true</c> if the method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>, <c>false</c> otherwise.</returns>
    internal bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
    {
        var foundMethod = this.mapMethods.FirstOrDefault(method => method.IsMapFor(targetType, sourceType));
        mapMethod = foundMethod!;
        return foundMethod is not null;
    }

    /// <summary>
    /// Adds the map method to the list of methods.
    /// Method is added only if no other method with the same mapping exists.
    /// </summary>
    /// <param name="mapMethod">The method to be added.</param>
    internal void TryAddMethod(MapMethod mapMethod)
    {
        if (!this.TryGetMethod(mapMethod.TargetType, mapMethod.SourceType, out _))
        {
            this.mapMethods.Add(mapMethod);
        }
    }

    /// <summary>
    /// Check if all methods have been mapped.
    /// </summary>
    /// <returns>
    /// <c>true</c> if all <see cref="MapMethods"/> have the
    /// <see cref="MapMethod.Mapped"/> flag set to <c>true</c>.</returns>
    internal bool AreAllMethodsMapped()
        => this.mapMethods.All(mapMethod => mapMethod.Mapped);

    /// <summary>
    /// Records a new set of diagnostics.
    /// </summary>
    /// <param name="diagnostics">The diagnostics to be added.</param>
    internal void ReportDiagnostics(IEnumerable<Diagnostic> diagnostics)
        => this.diagnostics.AddRange(diagnostics);

    /// <summary>
    /// Record a new diagnostic.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to be added.</param>
    internal void ReportDiagnostic(Diagnostic diagnostic)
        => this.ReportDiagnostics(new[] { diagnostic });
}