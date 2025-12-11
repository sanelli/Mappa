// <copyright file="PolymorphismMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Strategy detector to support polymorphism.
/// </summary>
internal sealed class PolymorphismMapStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context = context;
    private readonly Compilation compilation = compilation;
    private readonly CancellationToken cancellationToken = cancellationToken;

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Check the method has the type mapping attributes.
        var typeMappingAttributes = this.GetTypeMappingAttributes();
        if (typeMappingAttributes.Length == 0)
        {
            return false;
        }

        // Check the attributes.
        var sourceTypeFullNames = new HashSet<string>();
        var subtypesMappingsStrategies = new List<MapStrategy>();
        var rootMapMethod = this.context.GetRootMapMethod();
        foreach (var attribute in typeMappingAttributes)
        {
            // Check attribute source type name is valid.
            if (string.IsNullOrWhiteSpace(attribute.SourceType.FullName))
            {
                // We ignore attribute will null values so this should never happen.
                throw new MappaGeneratorException("The source type cannot be loaded at compile time");
            }

            // Check attribute source type is not duplicated.
            if (!sourceTypeFullNames.Add(attribute.SourceType.FullName))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MappaTypeMappingAttributeHaveTheSameSourceType(attribute.SourceType.FullName, rootMapMethod.Location));
                return false;
            }

            // Check attribute target type name is valid.
            if (string.IsNullOrWhiteSpace(attribute.TargetType.FullName))
            {
                // We ignore attribute will null values so this should never happen.
                throw new MappaGeneratorException("The target type cannot be loaded at compile time");
            }

            // Check attribute source type can be loaded.
            var attributeSourceType = this.compilation.GetTypeByMetadataName(attribute.SourceType.FullName);
            if (attributeSourceType is null)
            {
                throw new MappaGeneratorException($"The source type '{attribute.SourceType.FullName}' cannot be correctly laded at compile time.");
            }

            // Check attribute target type can be loaded.
            var attributeTargetType = this.compilation.GetTypeByMetadataName(attribute.TargetType.FullName);
            if (attributeTargetType is null)
            {
                throw new MappaGeneratorException($"The target type '{attribute.SourceType.FullName}' cannot be correctly laded at compile time.");
            }

            // Check attribute source type and map method source type are different types.
            if (SymbolEqualityComparer.Default.Equals(attributeSourceType, this.context.SourceType))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MappaTypeMappingAttributeMapsSourceType(attribute.SourceType.FullName, rootMapMethod.Location));
                return false;
            }

            // Check attribute source type is derived form the source type in the map method somehow.
            if (!attributeSourceType.IsImplementingOrIsDerivedFromClass(this.context.SourceType))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType(
                    attributeSourceType.ToDisplayString(),
                    this.context.SourceType.ToDisplayString(),
                    rootMapMethod.Location));
                return false;
            }

            // Check attribute target type is derived form the target type in the map method somehow.
            if (!attributeTargetType.IsImplementingOrIsDerivedFromClass(this.context.TargetType))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType(
                        attributeTargetType.ToDisplayString(),
                        this.context.TargetType.ToDisplayString(),
                        rootMapMethod.Location));
                return false;
            }

            // At this stage any nullability concern is already handled by the nullability detector
            // and if we do not force it to be NotAnnotated it will be None which is handled like
            // annotated to support the non-nullable context.
            var sourceType = attributeSourceType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var targetType = attributeTargetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

            // Identify mapping from attribute source type to attribute target type.
            var attributeContext = new DerivedMappaMapAlgorithmContext(this.context, targetType, sourceType);
            var attributeAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(attributeContext, this.compilation, this.cancellationToken);
            var attributeStrategy = attributeAlgorithm.GetStrategy();
            if (attributeStrategy is NoMapStrategy)
            {
                // No need to add a diagnostic: the algorithm will add one if needed.
                return false;
            }

            subtypesMappingsStrategies.Add(attributeStrategy);
        }

        var mappaTypeMappingDefaultAttribute = this.GetTypeMappingDefaultAttribute()
                                               ?? new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw);

        var methodSymbolContainingSymbol = rootMapMethod.MethodSymbol.ContainingSymbol as ITypeSymbol ?? throw new MappaGeneratorException("Method parent is not a type symbol");
        var mapMethodHasTwoParameters = rootMapMethod.MethodSymbol.Parameters.Length == 2;
        var validationSuccessful = mappaTypeMappingDefaultAttribute.IsValid(
            this.context.TargetType,
            this.context.SourceType,
            methodSymbolContainingSymbol,
            rootMapMethod.NullableEnabled,
            mapMethodHasTwoParameters,
            this.compilation,
            rootMapMethod.Location,
            out var validationDiagnosis);
        foreach (var diagnostic in validationDiagnosis)
        {
            this.context.ReportDiagnostic(diagnostic);
        }

        if (!validationSuccessful)
        {
            // No need to report any extra diagnostic.
            return false;
        }

        // Identify a mapping if required.
        MapStrategy defaultMappingStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (mappaTypeMappingDefaultAttribute.Behavior is MappaTypeMappingDefaultBehavior.MapSourceType)
        {
            var targetTypeFullNameFromAttribute = mappaTypeMappingDefaultAttribute.Type?.FullName;

            var targetSymbol = (!string.IsNullOrWhiteSpace(targetTypeFullNameFromAttribute) ? this.compilation.GetTypeByMetadataName(targetTypeFullNameFromAttribute!) : null)
                               ?? this.context.TargetType;

            if (targetSymbol.TypeKind == TypeKind.Interface)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.TypeMustBeConcrete(targetSymbol.ToDisplayString(), rootMapMethod.Location));
                return false;
            }

            if (targetSymbol.IsAbstract)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.TypeMustBeConcrete(targetSymbol.ToDisplayString(), rootMapMethod.Location));
                return false;
            }

            var derivedContext = new DerivedMappaMapAlgorithmContext(this.context, targetSymbol, this.context.SourceType);
            var algorithm = new TypeMapIdentifierAlgorithm(derivedContext, this.compilation, this.cancellationToken);
            defaultMappingStrategy = algorithm.GetStrategy();

            if (defaultMappingStrategy is NoMapStrategy)
            {
                // No need to add a diagnostic: the algorithm will add one if needed.
                return false;
            }
        }

        mapStrategy = new PolymorphismMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            [.. subtypesMappingsStrategies],
            mappaTypeMappingDefaultAttribute,
            defaultMappingStrategy,
            rootMapMethod.NullableEnabled,
            rootMapMethod.MaybeGetMappaContextParameterName());
        return true;
    }

    private MappaTypeMappingAttribute[] GetTypeMappingAttributes()
        => this.context.GetRootMapMethod().GetAttributes<MappaTypeMappingAttribute>();

    private MappaTypeMappingDefaultAttribute? GetTypeMappingDefaultAttribute()
        => this.context.GetRootMapMethod().GetAttribute<MappaTypeMappingDefaultAttribute>();
}