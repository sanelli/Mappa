// <copyright file="ProtobufPolymorphismMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Strategy detector to support polymorphism driven by a protobuf <c>oneof</c>.
/// </summary>
internal sealed partial class ProtobufPolymorphismMapStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context = context;
    private readonly Compilation compilation = compilation;
    private readonly CancellationToken cancellationToken = cancellationToken;

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var rootMapMethod = this.context.GetRootMapMethod();
        var typeMappingAttributes = rootMapMethod.GetAttributes<MappaProtobufTypeMappingAttribute>();
        var defaultAttributes = rootMapMethod.GetAttributes<MappaProtobufTypeMappingDefaultAttribute>();
        var setupAttributes = rootMapMethod.GetAttributes<MappaProtobufPolymorphismSetupAttribute>();
        if (!this.TryValidateAttributesPresence(rootMapMethod, setupAttributes.Length)
            || !this.TryResolveSides(setupAttributes[0], rootMapMethod, out var scenario, out var sourceSide, out var targetSide))
        {
            return false;
        }

        var defaultAttribute = GetDefaultAttribute(defaultAttributes);
        if (!this.TryBuildDefaultAndArms(
                defaultAttribute,
                typeMappingAttributes,
                scenario,
                sourceSide,
                targetSide,
                rootMapMethod,
                out var arms,
                out var mapToArm))
        {
            return false;
        }

        mapStrategy = new ProtobufPolymorphismMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            scenario,
            sourceSide?.CaseProperty,
            sourceSide?.CaseEnum,
            [.. arms],
            defaultAttribute,
            mapToArm);
        return true;
    }

    private static MappaProtobufTypeMappingDefaultAttribute GetDefaultAttribute(MappaProtobufTypeMappingDefaultAttribute[] defaultAttributes)
        => defaultAttributes.Length > 0
            ? defaultAttributes[0]
            : new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Throw);

    private bool TryValidateAttributesPresence(MapMethod rootMapMethod, int setupCount)
    {
        if (rootMapMethod.HasAnyAttribute<MappaTypeMappingAttribute>()
            || rootMapMethod.HasAnyAttribute<MappaTypeMappingDefaultAttribute>())
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismMixedWithTypeMapping(
                rootMapMethod.Location,
                rootMapMethod.MethodName));
            return false;
        }

        if (setupCount == 0)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismSetupMissing(
                rootMapMethod.Location,
                rootMapMethod.MethodName));
            return false;
        }

        if (setupCount > 1)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismMultipleSetups(
                rootMapMethod.Location,
                rootMapMethod.MethodName));
            return false;
        }

        return true;
    }

    private bool TryBuildDefaultAndArms(
        MappaProtobufTypeMappingDefaultAttribute defaultAttribute,
        MappaProtobufTypeMappingAttribute[] typeMappingAttributes,
        ProtobufPolymorphismScenario scenario,
        OneofSide? sourceSide,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        out List<ProtobufPolymorphismArm> arms,
        out ProtobufPolymorphismArm? mapToArm)
    {
        arms = [];
        mapToArm = null;
        return this.TryValidateDefaultAttribute(defaultAttribute, scenario, rootMapMethod)
               && this.TryBuildArms(typeMappingAttributes, scenario, sourceSide, targetSide, rootMapMethod, out arms)
               && this.TryBuildMapToArm(defaultAttribute, targetSide, rootMapMethod, out mapToArm);
    }
}