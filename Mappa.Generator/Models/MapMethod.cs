// <copyright file="MapMethod.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Models;

/// <summary>
/// Describe a method that can be used for mapping.
/// </summary>
internal sealed class MapMethod
{
    private MethodParameterMapStrategy? methodParameterMapStrategy = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethod"/> class.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public MapMethod(
        MethodDeclarationSyntax methodDeclarationSyntax,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        this.MethodDeclarationSyntax = methodDeclarationSyntax;
        this.FieldName = this.MethodDeclarationSyntax.IsStatic() ? string.Empty : "this";
        this.MethodName = methodDeclarationSyntax.Identifier.ToFullString();
        this.MethodSymbol = semanticModel.GetSymbolInfo(this.MethodDeclarationSyntax, cancellationToken).Symbol as IMethodSymbol
            ?? throw new MappaGeneratorException($"Cannot obtain the method symbol for method \"{this.MethodDeclarationSyntax.Identifier}\" syntax node.", methodDeclarationSyntax.GetLocation());
        this.TargetType = this.MethodSymbol.ReturnType;
        this.SourceType = this.MethodSymbol.Parameters.First().Type;
        this.SourceParameterName = this.MethodSymbol.Parameters.First().Name;
        this.Mapped = false;
    }

    /// <summary>
    /// Gets the field name to access method.
    /// </summary>
    internal string FieldName { get; }

    /// <summary>
    /// Gets the method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal MethodDeclarationSyntax MethodDeclarationSyntax { get; }

    /// <summary>
    /// Gets the method symbol.
    /// </summary>
    internal IMethodSymbol MethodSymbol { get; }

    /// <summary>
    /// Gets the source parameter name.
    /// </summary>
    internal string SourceParameterName { get; }

    /// <summary>
    /// Gets a value indicating whether the.
    /// </summary>
    internal bool Mapped { get; private set; }

    /// <summary>
    /// Gets the method strategy.
    /// </summary>
    internal MethodParameterMapStrategy Strategy => this.methodParameterMapStrategy
        ?? throw new MappaGeneratorException($"Strategy for method\"{this.MethodDeclarationSyntax.Identifier}\" has not been identified yet.", this.MethodDeclarationSyntax.GetLocation());

    /// <summary>
    /// Gets a value indicating whether the strategy has been set.
    /// </summary>
    internal bool HasStrategy => this.methodParameterMapStrategy is not null;

    /// <summary>
    /// Mark the method as being mapped.
    /// </summary>
    internal void MarkMapped() => this.Mapped = true;

    /// <summary>
    /// Sets the startegy for the method.
    /// </summary>
    /// <param name="strategy">The strategy to be applied to the method.</param>
    /// <exception cref="MappaGeneratorException">When the strategy has been already set.</exception>
    internal void SetStrategy(MethodParameterMapStrategy strategy)
    {
        if (this.HasStrategy)
        {
            throw new MappaGeneratorException($"Strategy for method\"{this.MethodDeclarationSyntax.Identifier}\" has already been identified.", this.MethodDeclarationSyntax.GetLocation());
        }

        this.methodParameterMapStrategy = strategy;
    }

    /// <summary>
    /// Check if the method is map from <paramref name="sourceType"/>
    /// to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <returns><c>true</c> if the method is a map from
    /// <paramref name="sourceType"/> to <paramref name="targetType"/>.</returns>
    internal bool IsMapFor(ITypeSymbol targetType, ITypeSymbol sourceType)
        => SymbolEqualityComparer.Default.Equals(targetType, this.TargetType)
            && SymbolEqualityComparer.Default.Equals(sourceType, this.SourceType);
}