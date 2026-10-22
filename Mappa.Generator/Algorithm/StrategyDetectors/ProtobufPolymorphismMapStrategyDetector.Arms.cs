// <copyright file="ProtobufPolymorphismMapStrategyDetector.Arms.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Arms detection for <see cref="ProtobufPolymorphismMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ProtobufPolymorphismMapStrategyDetector
{
    private bool TryBuildArms(
        MappaProtobufTypeMappingAttribute[] typeMappingAttributes,
        ProtobufPolymorphismScenario scenario,
        OneofSide? sourceSide,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        out List<ProtobufPolymorphismArm> arms)
    {
        arms = [];
        var seenArmKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var attribute in typeMappingAttributes)
        {
            this.cancellationToken.ThrowIfCancellationRequested();
            if (!this.TryBuildArm(attribute, scenario, sourceSide, targetSide, rootMapMethod, seenArmKeys, out var arm))
            {
                return false;
            }

            arms.Add(arm);
        }

        return true;
    }

    private bool TryBuildArm(
        MappaProtobufTypeMappingAttribute attribute,
        ProtobufPolymorphismScenario scenario,
        OneofSide? sourceSide,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        if (scenario is ProtobufPolymorphismScenario.ProtoToPoco)
        {
            return this.TryBuildProtoToPocoAttributeArm(attribute, sourceSide, rootMapMethod, seenArmKeys, out arm);
        }

        if (scenario is ProtobufPolymorphismScenario.PocoToProto)
        {
            return this.TryBuildPocoToProtoAttributeArm(attribute, targetSide, rootMapMethod, seenArmKeys, out arm);
        }

        return this.TryBuildProtoToProtoAttributeArm(attribute, sourceSide, targetSide, rootMapMethod, seenArmKeys, out arm);
    }

    private bool TryBuildProtoToPocoAttributeArm(
        MappaProtobufTypeMappingAttribute attribute,
        OneofSide? sourceSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        if (sourceSide is null || attribute.TargetType is null || attribute.SourceOneOfCaseName is null)
        {
            return this.FailArmFormMismatch(ProtobufPolymorphismScenario.ProtoToPoco, rootMapMethod, out arm);
        }

        return this.TryBuildProtoToPocoArm(attribute.TargetType, attribute.SourceOneOfCaseName, sourceSide, rootMapMethod, seenArmKeys, out arm);
    }

    private bool TryBuildPocoToProtoAttributeArm(
        MappaProtobufTypeMappingAttribute attribute,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        if (targetSide is null || attribute.TargetOneOfCaseName is null || attribute.SourceType is null)
        {
            return this.FailArmFormMismatch(ProtobufPolymorphismScenario.PocoToProto, rootMapMethod, out arm);
        }

        return this.TryBuildPocoToProtoArm(attribute.TargetOneOfCaseName, attribute.SourceType, targetSide, rootMapMethod, seenArmKeys, out arm);
    }

    private bool TryBuildProtoToProtoAttributeArm(
        MappaProtobufTypeMappingAttribute attribute,
        OneofSide? sourceSide,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        if (sourceSide is null || targetSide is null || attribute.TargetOneOfCaseName is null || attribute.SourceOneOfCaseName is null)
        {
            return this.FailArmFormMismatch(ProtobufPolymorphismScenario.ProtoToProto, rootMapMethod, out arm);
        }

        return this.TryBuildProtoToProtoArm(attribute.TargetOneOfCaseName, attribute.SourceOneOfCaseName, sourceSide, targetSide, rootMapMethod, seenArmKeys, out arm);
    }

    private bool FailArmFormMismatch(ProtobufPolymorphismScenario scenario, MapMethod rootMapMethod, out ProtobufPolymorphismArm arm)
    {
        arm = this.CreateNoMapArm();
        this.ReportArmFormMismatch(scenario, rootMapMethod);
        return false;
    }

    private ProtobufPolymorphismArm CreateNoMapArm()
        => new(new NoMapStrategy(this.context.TargetType, this.context.SourceType), null, null, null);

    private void ReportArmFormMismatch(ProtobufPolymorphismScenario scenario, MapMethod rootMapMethod)
    {
        var (scenarioName, expected) = scenario switch
        {
            ProtobufPolymorphismScenario.ProtoToPoco => ("protobuf to POCO", "(Type targetType, string sourceOneOfCaseName)"),
            ProtobufPolymorphismScenario.PocoToProto => ("POCO to protobuf", "(string targetOneOfCaseName, Type sourceType)"),
            _ => ("protobuf to protobuf", "(string targetOneOfCaseName, string sourceOneOfCaseName)"),
        };
        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismArmFormMismatch(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            scenarioName,
            expected));
    }

    private bool TryBuildProtoToPocoArm(
        Type attributeTargetType,
        string sourceCaseName,
        OneofSide sourceSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        arm = this.CreateNoMapArm();
        if (!this.TryResolveArmProperty(sourceSide, sourceCaseName, rootMapMethod, out var sourceArmProperty)
            || !this.TryRegisterArm(seenArmKeys, sourceCaseName, rootMapMethod)
            || !this.TryLoadCompatibleType(attributeTargetType, this.context.TargetType, rootMapMethod, out var clrTargetType))
        {
            return false;
        }

        var strategy = this.IdentifyArmStrategy(clrTargetType, sourceArmProperty.Type);
        if (strategy is NoMapStrategy)
        {
            return false;
        }

        arm = new ProtobufPolymorphismArm(strategy, sourceCaseName, sourceArmProperty.Name, null);
        return true;
    }

    private bool TryBuildPocoToProtoArm(
        string targetCaseName,
        Type attributeSourceType,
        OneofSide targetSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        arm = this.CreateNoMapArm();
        if (!this.TryResolveWritableArmProperty(targetSide, targetCaseName, rootMapMethod, out var targetArmProperty)
            || !this.TryLoadCompatibleType(attributeSourceType, this.context.SourceType, rootMapMethod, out var clrSourceType)
            || !this.TryRegisterArm(seenArmKeys, clrSourceType.ToDisplayString(), rootMapMethod))
        {
            return false;
        }

        var strategy = this.IdentifyArmStrategy(targetArmProperty.Type, clrSourceType);
        if (strategy is NoMapStrategy)
        {
            return false;
        }

        arm = new ProtobufPolymorphismArm(strategy, null, null, targetArmProperty.Name);
        return true;
    }

    private bool TryBuildProtoToProtoArm(
        string targetCaseName,
        string sourceCaseName,
        OneofSide sourceSide,
        OneofSide targetSide,
        MapMethod rootMapMethod,
        HashSet<string> seenArmKeys,
        out ProtobufPolymorphismArm arm)
    {
        arm = this.CreateNoMapArm();
        if (!this.TryResolveArmProperty(sourceSide, sourceCaseName, rootMapMethod, out var sourceArmProperty)
            || !this.TryResolveWritableArmProperty(targetSide, targetCaseName, rootMapMethod, out var targetArmProperty)
            || !this.TryRegisterArm(seenArmKeys, sourceCaseName, rootMapMethod))
        {
            return false;
        }

        var strategy = this.IdentifyArmStrategy(targetArmProperty.Type, sourceArmProperty.Type);
        if (strategy is NoMapStrategy)
        {
            return false;
        }

        arm = new ProtobufPolymorphismArm(strategy, sourceCaseName, sourceArmProperty.Name, targetArmProperty.Name);
        return true;
    }

    private bool TryBuildMapToArm(
        MappaProtobufTypeMappingDefaultAttribute defaultAttribute,
        OneofSide? targetSide,
        MapMethod rootMapMethod,
        out ProtobufPolymorphismArm? mapToArm)
    {
        mapToArm = null;
        if (defaultAttribute.Behavior is not MappaProtobufTypeMappingDefaultBehavior.MapTo)
        {
            return true;
        }

        if (targetSide is null || defaultAttribute.TargetOneOfCaseName is not { } caseName)
        {
            throw new MappaGeneratorException("MapTo default behavior requires a protobuf target and a case name.");
        }

        if (!this.TryResolveWritableArmProperty(targetSide, caseName, rootMapMethod, out var targetArmProperty))
        {
            return false;
        }

        var strategy = this.IdentifyArmStrategy(targetArmProperty.Type, this.context.SourceType);
        if (strategy is NoMapStrategy)
        {
            return false;
        }

        mapToArm = new ProtobufPolymorphismArm(strategy, null, null, targetArmProperty.Name);
        return true;
    }

    private bool TryResolveWritableArmProperty(
        OneofSide side,
        string caseName,
        MapMethod rootMapMethod,
        out IPropertySymbol armProperty)
    {
        if (!this.TryResolveArmProperty(side, caseName, rootMapMethod, out armProperty))
        {
            return false;
        }

        if (armProperty.SetMethod is { DeclaredAccessibility: Accessibility.Public })
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismArmPropertyNotWritable(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            armProperty.Name,
            side.MessageType.ToDisplayString()));
        return false;
    }

    private bool TryResolveArmProperty(
        OneofSide side,
        string caseName,
        MapMethod rootMapMethod,
        out IPropertySymbol armProperty)
    {
        armProperty = side.CaseProperty;
        if (string.Equals(caseName, ProtobufOneofHelper.NoneCaseName, StringComparison.Ordinal))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismCaseNameNone(
                rootMapMethod.Location,
                rootMapMethod.MethodName));
            return false;
        }

        var property = ProtobufOneofHelper.HasCaseMember(side.CaseEnum, caseName)
            ? ProtobufOneofHelper.FindProperty(side.MessageType, caseName)
            : null;
        if (property is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismCaseNameUnknown(
                rootMapMethod.Location,
                rootMapMethod.MethodName,
                caseName,
                side.MessageType.ToDisplayString()));
            return false;
        }

        if (!ProtobufOneofHelper.IsMessageType(property.Type))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismScalarArm(
                rootMapMethod.Location,
                rootMapMethod.MethodName,
                caseName,
                side.MessageType.ToDisplayString(),
                property.Type.ToDisplayString()));
            return false;
        }

        armProperty = property;
        return true;
    }

    private bool TryRegisterArm(HashSet<string> seenArmKeys, string armKey, MapMethod rootMapMethod)
    {
        if (seenArmKeys.Add(armKey))
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismDuplicateArm(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            armKey));
        return false;
    }

    private bool TryLoadCompatibleType(
        Type attributeType,
        ITypeSymbol mapMethodType,
        MapMethod rootMapMethod,
        out ITypeSymbol clrType)
    {
        clrType = mapMethodType;
        if (string.IsNullOrWhiteSpace(attributeType.FullName))
        {
            throw new MappaGeneratorException("The type of a protobuf type mapping cannot be loaded at compile time");
        }

        var loadedType = this.compilation.GetTypeByMetadataName(attributeType.FullName)
            ?? throw new MappaGeneratorException($"The type '{attributeType.FullName}' cannot be correctly loaded at compile time.");
        if (!loadedType.IsImplementingOrIsDerivedFromClass(mapMethodType))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismClrTypeNotCompatible(
                rootMapMethod.Location,
                rootMapMethod.MethodName,
                loadedType.ToDisplayString(),
                mapMethodType.ToDisplayString()));
            return false;
        }

        clrType = loadedType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        return true;
    }

    private MapStrategy IdentifyArmStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        // Any nullability concern is already handled by the nullability detector: if we do not force
        // NotAnnotated the annotation would be None, which is handled as annotated in non-nullable contexts.
        var armContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated),
            sourceType.WithNullableAnnotation(NullableAnnotation.NotAnnotated));
        var armAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(armContext, this.compilation, this.cancellationToken);

        // No need to add a diagnostic when no strategy is found: the algorithm already reports one.
        return armAlgorithm.GetStrategy();
    }
}