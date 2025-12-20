// <copyright file="MappaClassGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// The context of the generator for a single class.
/// </summary>
internal sealed class MappaClassGeneratorContext
{
    private readonly List<MapMethod> mapMethods = [];
    private readonly List<Diagnostic> diagnostics = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaClassGeneratorContext"/> class.
    /// </summary>
    /// <param name="options">The mappa global options.</param>
    /// <param name="mappaDebug">Mappa debug helper.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    public MappaClassGeneratorContext(
        MappaGlobalOptions options,
        MappaDebug mappaDebug,
        Compilation compilation,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        this.Options = options;
        this.MappaDebug = mappaDebug;
        this.Compilation = compilation;
        this.ClassDeclarationSyntax = classDeclarationSyntax;
        this.SemanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        this.ClassSymbol = this.SemanticModel.GetDeclaredSymbol(this.ClassDeclarationSyntax) as INamedTypeSymbol
                           ?? throw new MappaGeneratorException(
                               "Cannot obtain semantic model",
                               this.ClassDeclarationSyntax.GetLocation());
    }

    /// <summary>
    /// Gets the global mappa options.
    /// </summary>
    internal MappaGlobalOptions Options { get; }

    /// <summary>
    /// Gets the debugging tool.
    /// </summary>
    internal MappaDebug MappaDebug { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets the semantic model.
    /// </summary>
    internal SemanticModel SemanticModel { get; }

    /// <summary>
    /// Gets the class declaration syntax of the mapper class begin processed.
    /// </summary>
    internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }

    /// <summary>
    /// Gets the declared symbol.
    /// </summary>
    internal INamedTypeSymbol ClassSymbol { get; }

    /// <summary>
    /// Gets the list of map methods associated to class defined by <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    internal IReadOnlyCollection<MapMethod> MapMethods => this.mapMethods;

    /// <summary>
    /// Gets the diagnostics associated with the mapping.
    /// </summary>
    internal IReadOnlyCollection<Diagnostic> Diagnostics => this.diagnostics;

    /// <summary>
    /// Gets a value indicating whether a diagnostic with severity error has been reported.
    /// </summary>
    internal bool HasErrorDiagnostics =>
        this.diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for a syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to investigate.</param>
    /// <returns><c>true</c> if the nullable context is enabled, <c>false</c> otherwise.</returns>
    internal bool IsNullableEnabled(SyntaxNode syntaxNode)
    {
        var methodNullableContext = this.SemanticModel.GetNullableContext(syntaxNode.SpanStart);

        bool contextInherit = (methodNullableContext & NullableContext.ContextInherited) > 0;
        if (contextInherit)
        {
            var projectEnabled = (this.Compilation.Options.NullableContextOptions & NullableContextOptions.Enable) > 0;
            return projectEnabled;
        }

        bool isNullableEnabled = (methodNullableContext & NullableContext.Enabled) > 0;
        return isNullableEnabled;
    }

    /// <summary>
    /// Try getting the method for mapping from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="nullableEnabled">Nullable enabled.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <returns><c>true</c> if the method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>, <c>false</c> otherwise.</returns>
    internal bool TryGetMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        out MapMethod mapMethod)
    {
        var foundMethod =
            this.mapMethods.Find(method => method.IsMapFor(targetType, sourceType, nullableEnabled));
        mapMethod = foundMethod!;
        return foundMethod is not null;
    }

    /// <summary>
    /// Try getting the method for mapping from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/> by checking the polymorphic methods.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="nullableEnabled">Nullable enabled.</param>
    /// <param name="mapMethod">The map method, if it exists.</param>
    /// <returns><c>true</c> if the method to map from <paramref name="sourceType"/> to
    /// <paramref name="targetType"/>, <c>false</c> otherwise.</returns>
    internal bool TryGetPolymorphicMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool nullableEnabled,
        out MapMethod mapMethod)
    {
        foreach (var method in this.mapMethods)
        {
           var typeMappingAttributes = method.GetAttributes<MappaTypeMappingAttribute>();
           if (typeMappingAttributes.Length <= 0)
           {
               // Only look for methods that have any MappaTypeMappingAttribute.
               break;
           }

           if (nullableEnabled)
           {
                // TODO [#49] Add checks to validate nullability can be satisfied.
           }

           // Search in the attributes to see if there is a mapping that can be used.
           foreach (var typeMappingAttribute in typeMappingAttributes)
           {
               var attributeSourceType = this.Compilation.GetTypeByMetadataName(typeMappingAttribute.SourceType.FullName!);
               var attributeTargetType = this.Compilation.GetTypeByMetadataName(typeMappingAttribute.TargetType.FullName!);

               if (attributeSourceType is not null &&
                   attributeTargetType is not null &&
                   attributeSourceType.IsEqualTo(sourceType, false) &&
                   attributeTargetType.IsEqualTo(targetType, false))
               {
                   mapMethod = method;
                   return true;
               }
           }

           // Pick up the MappaTypeMappingDefault only if defined and it specify the behavior MapSourceType.
           // Not that this will only pick up the setup where the target type is defined.
           // If the attribute target type is the same as the target type we would not even be here because
           // the method would be picked up earlier by the non polymorphic version of this.
           var mappaTypeMappingDefaultAttribute = method.GetAttribute<MappaTypeMappingDefaultAttribute>();
           if (mappaTypeMappingDefaultAttribute is not null &&
               mappaTypeMappingDefaultAttribute.Behavior is MappaTypeMappingDefaultBehavior.MapSourceType &&
               mappaTypeMappingDefaultAttribute.Type is not null)
           {
               var attributeTargetType = this.Compilation.GetTypeByMetadataName(mappaTypeMappingDefaultAttribute.Type.FullName!);
               if (attributeTargetType is not null &&
                   attributeTargetType.IsEqualTo(targetType, false))
               {
                   mapMethod = method;
                   return true;
               }
           }
        }

        mapMethod = null!;
        return false;
    }

    /// <summary>
    /// Adds the map method to the list of methods.
    /// Method is added only if no other method with the same mapping exists.
    /// </summary>
    /// <param name="mapMethod">The method to be added.</param>
    /// <returns><c>true</c> if the method has been added, false otherwise.</returns>
    internal bool TryAddMethod(MapMethod mapMethod)
    {
        if (!this.TryGetMethod(
                mapMethod.TargetType,
                mapMethod.SourceType,
                mapMethod.NullableEnabled,
                out _))
        {
            this.mapMethods.Add(mapMethod);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if all methods have been mapped.
    /// </summary>
    /// <returns>
    /// <c>true</c> if all <see cref="MapMethods"/> have the
    /// <see cref="MapMethod.Mapped"/> flag set to <c>true</c>.</returns>
    internal bool AreAllMethodsMapped()
        => this.mapMethods.TrueForAll(mapMethod => mapMethod.Mapped);

    /// <summary>
    /// Records a new set of diagnostics.
    /// </summary>
    /// <param name="generatedDiagnostics">The diagnostics to be added.</param>
    internal void ReportDiagnostics(IEnumerable<Diagnostic> generatedDiagnostics)
        => this.diagnostics.AddRange(generatedDiagnostics);

    /// <summary>
    /// Record a new diagnostic.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to be added.</param>
    internal void ReportDiagnostic(Diagnostic diagnostic)
        => this.ReportDiagnostics([diagnostic]);
}