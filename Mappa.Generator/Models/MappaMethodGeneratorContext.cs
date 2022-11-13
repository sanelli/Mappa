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
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodGeneratorContext"/> class.
    /// </summary>
    /// <param name="classContext">The context of the parent class.</param>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    public MappaMethodGeneratorContext(
        MappaClassGeneratorContext classContext,
        MethodDeclarationSyntax methodDeclarationSyntax)
    {
        this.ClassContext = classContext;
        this.MethodDeclarationSyntax = methodDeclarationSyntax;
        this.MethodSymbol = classContext.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol
            ?? throw new MappaGeneratorException($"Cannot obtain the semantic model for method \"{methodDeclarationSyntax.Identifier}\".", methodDeclarationSyntax.GetLocation());
    }

    /// <summary>
    /// Gets the parent class context.
    /// </summary>
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal MethodDeclarationSyntax MethodDeclarationSyntax { get; }

    /// <summary>
    /// Gets the method symbol.
    /// </summary>
    internal IMethodSymbol MethodSymbol { get; }

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for the method.
    /// </summary>
    /// <returns><c>true</c> if the nullable context is enabled, <c>false</c> otherwise.</returns>
    internal bool IsNullableEnabled()
    {
        var methodNullableContext = this.ClassContext.SemanticModel
           .GetNullableContext(this.MethodDeclarationSyntax
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
                throw new MappaGeneratorException($"Cannot obtain the nullable context for method \"{this.MethodDeclarationSyntax.Identifier}\": unsupported value \"{methodNullableContext}\".", this.MethodDeclarationSyntax.GetLocation());
        }
    }
}