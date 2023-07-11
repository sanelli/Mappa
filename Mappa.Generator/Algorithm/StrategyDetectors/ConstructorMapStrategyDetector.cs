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
    // TODO: if nullable is not enabled we might want to throw if input is null.
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. Constructor TargetType(SourceType input) exists -> InvokeMappingConstructorStrategy ( IMapStrategy(T.InputParameterType, S) )
        if (this.CanInvokeMappingConstructor(out var invokeConstructor, out var argumentStrategy))
        {
            mapStrategy = new InvokeMappingConstructorMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                invokeConstructor,
                argumentStrategy);
        }

        // 02. Can map individual properties using an empty parameter constructor. -> InvokeConstructorStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        else if (this.CanInvokeEmptyConstructor(out var emptyConstructorStrategy))
        {
            mapStrategy = emptyConstructorStrategy;
        }

        // 03. If there is not empty constructor try identifying the best one -> InvokeConstructorStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        // TODO: Implement me
        return mapStrategy is not NoMapStrategy;
    }

    private bool CanInvokeMappingConstructor(out IMethodSymbol constructor, out IMapStrategy strategy)
    {
        constructor = null!;
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 1 argument
        // - Are accessible
        // - Have a mapping from source to the type of the parameter
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol, 1);
        var constructorsWithStrategy = constructors
            .Select<IMethodSymbol, (IMethodSymbol Constructor, IMapStrategy Strategy)>(constructor =>
            {
                var constructorParameterType = constructor.Parameters.Single().Type;

                if (this.TryGetStrategyBetweenTypes(constructorParameterType, this.context.SourceType, false, out var constructorParameterStrategy))
                {
                    return (constructor, constructorParameterStrategy);
                }

                return (constructor, noMapStrategy);
            })
            .Where(constructorAndStrategy => constructorAndStrategy.Strategy is not NoMapStrategy)
            .ToArray();

        // Either user the only one that has been found.
        if (constructorsWithStrategy.Length == 1)
        {
            constructor = constructorsWithStrategy.Single().Constructor;
            strategy = constructorsWithStrategy.Single().Strategy;
        }

        // If more than one has been found check if any of these
        // that has the very same input type and ise that for the mapping.
        else
        {
            var constructorWithSameInputTypeAsSource = constructorsWithStrategy
                .Where(constructorWithStrategy =>
                    constructorWithStrategy.Constructor.Parameters.Single().Type.IsEqualTo(this.context.SourceType, this.context.IsNullableEnabled()))
                .ToArray();

            if (constructorWithSameInputTypeAsSource.Any())
            {
                constructor = constructorWithSameInputTypeAsSource.Single().Constructor;
                strategy = constructorWithSameInputTypeAsSource.Single().Strategy;
            }

            // No matching constructor has been found
            else
            {
                strategy = noMapStrategy;
            }
        }

        return strategy is not NoMapStrategy;
    }

    private bool CanInvokeEmptyConstructor(out IMapStrategy strategy)
    {
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 0 parameter
        // - Is accessible
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol, 0);

        // If there is no constructor with zero parameters cannot apply this strategy.
        if (!constructors.Any())
        {
            strategy = noMapStrategy;
        }
        else
        {
            // Gets the target properties
            // TODO: Allow to ignore some target properties.
            // TODO: ensure property setter method is accessible.
            var targetProperties = this.context.TargetType.GetTypeProperties()

                // Ignore indexer properties.
                // Ignore properties without a setter.
                // TODO: We might want to include properties with a getter implementing IList<T> to support protobuf?
                .Where(property => property.IsIndexer is false && property.SetMethod is not null)
                .ToArray();

            // If no target properties exist there is no point in applying this strategy
            // this way we can avoid to attempt using this strategy for basic types
            // like string, int, etc...
            if (targetProperties.Any())
            {
                // Gets the source properties.
                var sourceProperties = this.context.SourceType.GetTypeProperties()

                    // Ignore indexer properties.
                    // Ignore properties without a setter.
                    // TODO: ensure property getter method is accessible.
                    .Where(property => property.IsIndexer is false && property.GetMethod is not null)

                    // Map them to a dictionary
                    .ToDictionary(property => property.Name);

                // Match target property with a source property.
                var initializerStrategies = targetProperties
                    .Select(
                        targetProperty =>
                        {
                            // TODO: Allow to use a source property with a different name.
                            if (!sourceProperties.TryGetValue(targetProperty.Name, out var sourceProperty))
                            {
                                return new PropertyMapStrategy(targetProperty, null!, noMapStrategy);
                            }

                            var targetPropertyType = targetProperty.Type;
                            var sourcePropertyType = sourceProperty.Type;

                            if (this.TryGetStrategyBetweenTypes(targetPropertyType, sourcePropertyType, true, out var propertyStrategy))
                            {
                                return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategy);
                            }

                            // TODO: If target property is a get-only whose type is implementing the IList<T> we can invoke a specific strategy to just add the items
                            return new PropertyMapStrategy(targetProperty, null!, noMapStrategy);
                        })
                    .ToArray();

                // Check if any property strategy is required but no strategy has been found
                if (Array
                    .Exists(initializerStrategies, propertyStrategy => propertyStrategy.TargetProperty.IsRequired && propertyStrategy.PropertyStrategy is NoMapStrategy))
                {
                    strategy = noMapStrategy;
                }
                else
                {
                    // Filter out properties that have no mapping.
                    // TODO: Allow to prevent skipping some non required parameters.
                    initializerStrategies = initializerStrategies
                        .Where(propertyStrategy => propertyStrategy.PropertyStrategy is not NoMapStrategy)
                        .ToArray();

                    // TODO: Allow to return an error if some source properties are not mapped.
                    strategy = new InvokeConstructorMapStrategy(
                        MappaAlgorithmRule.InvokeEmptyConstructor,
                        this.context.TargetType,
                        this.context.SourceType,
                        constructors.Single(),
                        Array.Empty<PropertyMapStrategy>(),
                        initializerStrategies);
                }
            }
            else
            {
                strategy = noMapStrategy;
            }
        }

        return strategy is not NoMapStrategy;
    }

    private bool TryGetStrategyBetweenTypes(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool useConstructorMapStrategyDetector,
        out IMapStrategy elementStrategy)
    {
        using (this.context.Settings.UseConstructorMapStrategyDetector.Apply(useConstructorMapStrategyDetector))
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