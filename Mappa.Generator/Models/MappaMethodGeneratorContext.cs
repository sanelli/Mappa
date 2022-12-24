// <copyright file="MappaMethodGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// Context describing the mapping of a single method.
/// </summary>
internal sealed class MappaMethodGeneratorContext
    : MappaMapAlgorithmContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodGeneratorContext"/> class.
    /// </summary>
    /// <param name="classContext">The context of the parent class.</param>
    /// <param name="mapMethod">The method to be mapped.</param>
    public MappaMethodGeneratorContext(
        MappaClassGeneratorContext classContext,
        MapMethod mapMethod)
    {
        this.ClassContext = classContext;
        this.MapMethod = mapMethod;
    }

    /// <summary>
    /// Gets the parent class context.
    /// </summary>
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal MapMethod MapMethod { get; }

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType => throw new NotImplementedException();

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType => throw new NotImplementedException();

    /// <inheritdoc/>
    internal override string PropertyName => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for the method.
    /// </summary>
    /// <returns><c>true</c> if the nullable context is enabled, <c>false</c> otherwise.</returns>
    internal override bool IsNullableEnabled()
    {
        var methodNullableContext = this.ClassContext.SemanticModel
           .GetNullableContext(this.MapMethod.MethodDeclarationSyntax
               .GetLocation()
               .GetLineSpan()
               .StartLinePosition
               .Line);

        switch (methodNullableContext)
        {
            case NullableContext.Enabled:
                return true;
            case NullableContext.Disabled:
                return false;
            case NullableContext.ContextInherited:
                return this.ClassContext.Compilation.Options.NullableContextOptions == NullableContextOptions.Enable;
            default:
                throw new MappaGeneratorException($"Cannot obtain the nullable context for method \"{this.MapMethod.MethodDeclarationSyntax.Identifier}\": unsupported value \"{methodNullableContext}\".", this.MapMethod.MethodDeclarationSyntax.GetLocation());
        }
    }

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ClassContext.TryGetMethod(targetType, sourceType, this.IsNullableEnabled(), out mapMethod);
}