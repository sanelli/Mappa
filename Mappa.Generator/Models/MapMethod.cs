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
    private readonly Attribute[] attributes;
    private MethodParameterMapStrategy? methodParameterMapStrategy;
    private bool pragmaWarningHasBeenSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethod"/> class.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="nullableEnabled"><c>true</c> if reference nullable is enabled.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// The method is NOT considered mapped.
    /// </remarks>
    public MapMethod(
        MethodDeclarationSyntax methodDeclarationSyntax,
        SemanticModel semanticModel,
        bool nullableEnabled,
        CancellationToken cancellationToken)
    {
        this.MethodDeclarationSyntax = methodDeclarationSyntax;
        this.AccessFieldName = methodDeclarationSyntax.IsStatic() ? string.Empty : "this";
        this.MethodName = methodDeclarationSyntax.Identifier.ToFullString();
        this.MethodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax, cancellationToken)
            ?? throw new MappaGeneratorException($"Cannot obtain the method symbol for method \"{methodDeclarationSyntax.Identifier}\" syntax node.", methodDeclarationSyntax.GetLocation());
        this.TargetType = this.MethodSymbol.ReturnType;
        this.SourceType = this.MethodSymbol.Parameters[0].Type;
        this.SourceParameterName = this.MethodSymbol.Parameters[0].Name;
        this.Mapped = false;
        this.Location = methodDeclarationSyntax.GetLocation();
        this.NullableEnabled = nullableEnabled;
        this.PragmaWarning = PragmaWarningSetting.Undefined;
        this.attributes = this.MethodSymbol.GetMethodMappaAttributes(semanticModel.Compilation);
        this.CanBeUsedByStaticMethod = this.MethodSymbol.IsStatic;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethod"/> class.
    /// </summary>
    /// <param name="methodSymbol">The method symbol.</param>
    /// <param name="accessFiledName">The name of the field or property that can be used to access the method.</param>
    /// <param name="nullableEnabled"><c>true</c> if reference nullable is enabled.</param>
    /// <param name="canBeUsedByStaticMethod"><c>true</c> if the method can be invoked by a static method, <c>false</c> otherwise.</param>
    /// <param name="attributes">The attribute relevant to the mapping (might be less than the ones actually applied).</param>
    /// <remarks>
    /// The method is already considered mapped.
    /// </remarks>
    public MapMethod(
        IMethodSymbol methodSymbol,
        string accessFiledName,
        bool nullableEnabled,
        bool canBeUsedByStaticMethod,
        Attribute[] attributes)
    {
        this.MethodDeclarationSyntax = null;
        this.AccessFieldName = accessFiledName;
        this.MethodName = methodSymbol.Name;
        this.MethodSymbol = methodSymbol;
        this.TargetType = this.MethodSymbol.ReturnType;
        this.SourceType = this.MethodSymbol.Parameters[0].Type;
        this.SourceParameterName = this.MethodSymbol.Parameters[0].Name;
        this.Mapped = true;
        this.NullableEnabled = nullableEnabled;
        this.attributes = attributes;
        this.PragmaWarning = PragmaWarningSetting.Undefined;
        this.CanBeUsedByStaticMethod = canBeUsedByStaticMethod;
    }

    /// <summary>
    /// Gets a value indicating whether reference nullable is enabled.
    /// </summary>
    internal bool NullableEnabled { get; }

    /// <summary>
    /// Gets the pragma warning settings specific for the method being mapped.
    /// </summary>
    internal PragmaWarningSetting PragmaWarning { get; private set; }

    /// <summary>
    /// Gets the field name to access method.
    /// </summary>
    internal string AccessFieldName { get; }

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
    /// Gets the method symbol.
    /// </summary>
    internal IMethodSymbol MethodSymbol { get; }

    /// <summary>
    /// Gets the source parameter name.
    /// </summary>
    internal string SourceParameterName { get; }

    /// <summary>
    /// Gets a value indicating whether the method has been mapped.
    /// </summary>
    internal bool Mapped { get; private set; }

    /// <summary>
    /// Gets the location.
    /// </summary>
    internal Location? Location { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal MethodDeclarationSyntax? MethodDeclarationSyntax { get; }

    /// <summary>
    /// Gets a value indicating whether the method can be used in a static context.
    /// </summary>
    internal bool CanBeUsedByStaticMethod { get; }

    /// <summary>
    /// Gets the method strategy.
    /// </summary>
    internal MethodParameterMapStrategy Strategy => this.methodParameterMapStrategy
        ?? throw new MappaGeneratorException($"Strategy for method\"{this.MethodName}\" has not been identified yet.", this.Location);

    /// <summary>
    /// Gets a value indicating whether the strategy has been set.
    /// </summary>
    internal bool HasStrategy => this.methodParameterMapStrategy is not null;

    /// <summary>
    /// Mark the method as being mapped.
    /// </summary>
    internal void MarkMapped() => this.Mapped = true;

    /// <summary>
    /// Sets the strategy for the method.
    /// </summary>
    /// <param name="strategy">The strategy to be applied to the method.</param>
    /// <exception cref="MappaGeneratorException">When the strategy has been already set.</exception>
    internal void SetStrategy(MethodParameterMapStrategy strategy)
    {
        if (this.HasStrategy)
        {
            throw new MappaGeneratorException($"Strategy for method\"{this.MethodName}\" has already been identified.", this.Location);
        }

        this.methodParameterMapStrategy = strategy;
    }

    /// <summary>
    /// Check if the method is map from <paramref name="sourceType"/>
    /// to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="includeNullability"><c>true</c> to include nullability for reference types.</param>
    /// <returns><c>true</c> if the method is a map from
    /// <paramref name="sourceType"/> to <paramref name="targetType"/>.</returns>
    internal bool IsMapFor(ITypeSymbol targetType, ITypeSymbol sourceType, bool includeNullability)
        => this.TargetType.IsEqualTo(targetType, includeNullability)
               && this.SourceType.IsEqualTo(sourceType, includeNullability);

    /// <summary>
    /// Gets all the attributes of type <typeparamref name="TAttribute"/>
    /// applied to the method.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute required.</typeparam>
    /// <returns>The attributes of type <typeparamref name="TAttribute"/> applied to the method.</returns>
    internal TAttribute[] GetAttributes<TAttribute>()
        where TAttribute : Attribute
    {
        return [.. this.attributes.OfType<TAttribute>()];
    }

    /// <summary>
    /// Check if any parameter of type <typeparamref name="TAttribute"/> exist
    /// on the method.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns><c>true</c> if any attribute of type <typeparamref name="TAttribute"/> exist, <c>false</c> otherwise.</returns>
    internal bool HasAnyAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return this.attributes.Any(attribute => attribute is TAttribute);
    }

    /// <summary>
    /// Gets the attribute of type <typeparamref name="TAttribute"/>
    /// applied to the method, or <c>null</c> if the attribute does not exist.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute required.</typeparam>
    /// <returns>The attribute of type <typeparamref name="TAttribute"/> applied to the method, or <c>null</c> if the attribute does not exist.</returns>
    internal TAttribute? GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return this.attributes.OfType<TAttribute>().FirstOrDefault();
    }

    /// <summary>
    /// Returns <c>true</c> when the method require a mappa context to be invoked.
    /// </summary>
    /// <returns><c>true</c> when the method require a mappa context to be invoked, <c>false</c> otherwise.</returns>
    internal bool RequireMappaContextWhenInvoked()
    {
        return this.MethodSymbol.Parameters.Length == 2;
    }

    /// <summary>
    /// Returns <c>true</c> when the method provide a mappa context.
    /// </summary>
    /// <returns><c>true</c> when the method provide a mappa context, <c>false</c> otherwise.</returns>
    internal bool ProvideMappaContextWhenInvoked()
    {
        return this.MethodSymbol.Parameters.Length == 2;
    }

    /// <summary>
    /// Gets the name of the mappa context parameter.
    /// </summary>
    /// <returns>The name of the mappa context parameter.</returns>
    /// <exception cref="MappaGenerator">When the method does not have a mappa context parameter.</exception>
    internal string GetMappaContextParameterName()
        => this.MaybeGetMappaContextParameterName() ?? throw new MappaGeneratorException("Method does not have a mappa context parameter.");

    /// <summary>
    /// Gets the name of the mappa context parameter, returns <c>null</c> if not present.
    /// </summary>
    /// <returns>The name of the mappa context parameter, or <c>null</c> if not present.</returns>
    internal string? MaybeGetMappaContextParameterName()
    {
        if (this.MethodSymbol.Parameters.Length < 2)
        {
            return null;
        }

        return this.MethodSymbol.Parameters[1].Name;
    }

    /// <summary>
    /// Set the <see cref="PragmaWarning"/> field.
    /// </summary>
    /// <param name="pragmaWarning">The value to apply to <see cref="PragmaWarning"/>.</param>
    /// <exception cref="MappaGeneratorException">When the field has been set already.</exception>
    internal void SetPragmaWarning(PragmaWarningSetting pragmaWarning)
    {
        if (this.pragmaWarningHasBeenSet)
        {
            throw new MappaGeneratorException("You are trying to set a pragma warning multiple times.");
        }

        this.pragmaWarningHasBeenSet = true;
        this.PragmaWarning = pragmaWarning;
    }
}