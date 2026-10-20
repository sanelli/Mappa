// <copyright file="ConstructorMapStrategyDetector.UseConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// <see cref="Mappa.Attributes.MappaUseConstructorAttribute"/> forcing for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private static bool ConstructorParameterTypesMatch(IMethodSymbol constructor, ITypeSymbol[] expectedParameterTypes)
    {
        if (constructor.Parameters.Length != expectedParameterTypes.Length)
        {
            return false;
        }

        for (var index = 0; index < expectedParameterTypes.Length; index++)
        {
            if (!SymbolEqualityComparer.Default.Equals(constructor.Parameters[index].Type, expectedParameterTypes[index]))
            {
                return false;
            }
        }

        return true;
    }

    private bool TryDetectForcedUseConstructor(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var session = this.context.GetOrCreateMappaUseConstructorSession(this.compilation);
        if (!session.TryGetForTargetType(this.context.TargetType, out var attribute) || attribute is null)
        {
            return false;
        }

        var methodName = this.context.GetRootMapMethod().MethodName;
        if (!this.TryBuildForcedUseConstructorStrategy(attribute, methodName, out mapStrategy))
        {
            return true;
        }

        session.MarkUsed(attribute);
        return true;
    }

    private bool TryBuildForcedUseConstructorStrategy(
        MappaUseConstructorAttributeData attribute,
        string methodName,
        out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (attribute.ConstructorType == Mappa.ConstructorType.NoParameters)
        {
            return this.TryBuildForcedEmptyConstructor(attribute, methodName, out mapStrategy);
        }

        return this.TryBuildForcedParameterizedConstructor(attribute, methodName, out mapStrategy);
    }

    private bool TryBuildForcedEmptyConstructor(
        MappaUseConstructorAttributeData attribute,
        string methodName,
        out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var constructors = this.GetInvokableConstructors(0);
        if (constructors.Length == 0)
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.MappaUseConstructorNoEmptyConstructor(
                    attribute.Location,
                    methodName,
                    this.context.TargetType));
            return false;
        }

        if (!this.TryBuildEmptyCtorLikePropertyInitializers(
                requireAtLeastOneMappedProperty: false,
                out var propertiesWithStrategies))
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.MappaUseConstructorCannotMap(
                    attribute.Location,
                    methodName,
                    this.context.TargetType));
            return false;
        }

        var selectedConstructor = constructors[0];
        mapStrategy = new InvokeConstructorMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            selectedConstructor,
            [],
            propertiesWithStrategies,
            [],
            null,
            this.RequiresUnsafeAccessorForConstructor(selectedConstructor));
        return true;
    }

    private bool TryBuildForcedParameterizedConstructor(
        MappaUseConstructorAttributeData attribute,
        string methodName,
        out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var matchingConstructors = this.GetInvokableConstructors()
            .Where(constructor => ConstructorParameterTypesMatch(constructor, attribute.ParameterTypes))
            .ToArray();

        if (matchingConstructors.Length == 0)
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.MappaUseConstructorNoMatchingParameterizedConstructor(
                    attribute.Location,
                    methodName,
                    this.context.TargetType));
            return false;
        }

        var selectedConstructor = matchingConstructors[0];
        if (selectedConstructor.Parameters.Length == 1
            && selectedConstructor.Parameters[0].Type.IsEqualTo(
                this.context.SourceType,
                this.context.GetRootMapMethod().NullableEnabled))
        {
            mapStrategy = new InvokeMappingConstructorMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                selectedConstructor,
                new IdentityMapStrategy(selectedConstructor.Parameters[0].Type, this.context.SourceType),
                this.RequiresUnsafeAccessorForConstructor(selectedConstructor));
            return true;
        }

        if (!this.TryMapFactoryParameters(selectedConstructor, out var parameterMapStrategies))
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.MappaUseConstructorCannotMap(
                    attribute.Location,
                    methodName,
                    this.context.TargetType));
            return false;
        }

        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var sourceProperties = this.GetReadableSourceProperties(this.context.SourceType);
        var leftoverInitializerStrategies = this.TryBuildExplicitInterfaceLeftoverInitializersAfterParameterizedConstructor(
            selectedConstructor,
            sourceProperties,
            noMapStrategy);
        mapStrategy = new InvokeConstructorMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            selectedConstructor,
            parameterMapStrategies,
            leftoverInitializerStrategies,
            [],
            null,
            this.RequiresUnsafeAccessorForConstructor(selectedConstructor));
        return true;
    }
}