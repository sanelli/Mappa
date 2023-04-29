// <copyright file="ConstructorMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for the constructor strategies.
/// </summary>
internal sealed class ConstructorMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ConstructorMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. Constructor TargetType(SourceType input) exists -> InvokeMappingConstructorStrategy ( IMapStrategy(T.InputParameterType, S) )
        if (this.CanInvokeMappingConstructor(out var invokeConstructor, out var invokeStrategy))
        {
            mapStrategy = new InvokeMappingConstructorMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                invokeConstructor,
                invokeStrategy);
        }

        // 02. Can map individual properties using an empty parameter constructor. -> InvokeConstructorStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        // TODO: Implement me
        // 03. If there is not empty constructor try identifying the best one -> InvokeConstructorStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        // TODO: Implement me
        return mapStrategy is not NoMapStrategy;
    }

    private bool CanInvokeMappingConstructor(out IMethodSymbol constructor,  out IMapStrategy strategy)
    {
        constructor = null!;
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 1 parameter
        // - Are accessible
        // - Have a mapping from source to the type of the parameter
        var matchingConstructorsWithStrategy = ((INamedTypeSymbol)this.context.TargetType)
            .Constructors
            .Where(constructor => constructor.Parameters.Length == 1)
            .Where(constructor => this.compilation.IsSymbolAccessibleWithin(constructor, this.context.ParentSymbol))
            .Select<IMethodSymbol, (IMethodSymbol Constructor, IMapStrategy Strategy)>(constructor =>
            {
                var constructorParameterType = constructor.Parameters.Single().Type;

                if (this.TryGetStrategyBetweenTypes(out var constructorParameterStrategy, constructorParameterType, this.context.SourceType))
                {
                    return (constructor, constructorParameterStrategy);
                }

                return (constructor, noMapStrategy);
            })
            .Where(constructorAndStrategy => constructorAndStrategy.Strategy is not NoMapStrategy)
            .ToArray();

        // Either user the only one that has been found.
        if (matchingConstructorsWithStrategy.Length == 1)
        {
            constructor = matchingConstructorsWithStrategy.Single().Constructor;
            strategy = matchingConstructorsWithStrategy.Single().Strategy;
            return true;
        }

        // Of if more than one has been found check if any of these
        // that has the very same input type and ise that for the mapping.
        else
        {
            var constructorWithSameInputTypeAsSource = matchingConstructorsWithStrategy
                .Where(constructorWithStrategy =>
                    constructorWithStrategy.Constructor.Parameters.Single().Type.IsEqualTo(this.context.SourceType, this.context.IsNullableEnabled()))
                .ToArray();

            if (constructorWithSameInputTypeAsSource.Any())
            {
                constructor = constructorWithSameInputTypeAsSource.Single().Constructor;
                strategy = constructorWithSameInputTypeAsSource.Single().Strategy;
                return true;
            }
        }

        strategy = noMapStrategy;
        return false;
    }

    private bool TryGetStrategyBetweenTypes(out IMapStrategy elementStrategy, ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        using (this.context.Settings.UseConstructorMapStrategyDetector.Apply(false))
        {
            var derivedContext = new DerivedMappaMapAlgorithmContext(
                this.context,
                targetType,
                sourceType);
            var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
            elementStrategy = algorithm.GetStrategy();
            return elementStrategy is not NoMapStrategy;
        }
    }
}