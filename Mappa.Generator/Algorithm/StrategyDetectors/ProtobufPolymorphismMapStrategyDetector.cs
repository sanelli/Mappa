// <copyright file="ProtobufPolymorphismMapStrategyDetector.cs" company="Stefano Anelli">
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
/// Strategy detector to support polymorphism driven by a protobuf <c>oneof</c>.
/// </summary>
internal sealed class ProtobufPolymorphismMapStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
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
        if (typeMappingAttributes.Length == 0 && defaultAttributes.Length == 0 && setupAttributes.Length == 0)
        {
            return false;
        }

        if (!this.TryValidateAttributesPresence(rootMapMethod, typeMappingAttributes.Length + defaultAttributes.Length, setupAttributes.Length))
        {
            return false;
        }

        var defaultAttribute = defaultAttributes.Length > 0
            ? defaultAttributes[0]
            : new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Throw);

        if (!this.TryResolveSides(setupAttributes[0], rootMapMethod, out var scenario, out var sourceSide, out var targetSide))
        {
            return false;
        }

        if (!this.TryValidateDefaultAttribute(defaultAttribute, scenario, rootMapMethod)
            || !this.TryBuildArms(typeMappingAttributes, scenario, sourceSide, targetSide, rootMapMethod, out var arms)
            || !this.TryBuildMapToArm(defaultAttribute, targetSide, rootMapMethod, out var mapToArm))
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

    private bool TryValidateAttributesPresence(MapMethod rootMapMethod, int mappingAndDefaultCount, int setupCount)
    {
        if (rootMapMethod.HasAnyAttribute<MappaTypeMappingAttribute>()
            || rootMapMethod.HasAnyAttribute<MappaTypeMappingDefaultAttribute>())
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismMixedWithTypeMapping(
                rootMapMethod.Location,
                rootMapMethod.MethodName));
            return false;
        }

        if (setupCount == 0 && mappingAndDefaultCount > 0)
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

        return setupCount == 1;
    }

    private bool TryResolveSides(
        MappaProtobufPolymorphismSetupAttribute setup,
        MapMethod rootMapMethod,
        out ProtobufPolymorphismScenario scenario,
        out OneofSide? sourceSide,
        out OneofSide? targetSide)
    {
        scenario = ProtobufPolymorphismScenario.ProtoToPoco;
        sourceSide = null;
        targetSide = null;

        if (setup.OneOfCasePropertyName is { } singlePropertyName)
        {
            return this.TryResolveSingleProperty(singlePropertyName, rootMapMethod, out scenario, out sourceSide, out targetSide);
        }

        if (setup.TargetOneOfCasePropertyName is { } targetPropertyName
            && setup.SourceOneOfCasePropertyName is { } sourcePropertyName)
        {
            return this.TryResolveTwoProperties(targetPropertyName, sourcePropertyName, rootMapMethod, out scenario, out sourceSide, out targetSide);
        }

        throw new MappaGeneratorException("Invalid MappaProtobufPolymorphismSetup attribute.");
    }

    private bool TryResolveSingleProperty(
        string propertyName,
        MapMethod rootMapMethod,
        out ProtobufPolymorphismScenario scenario,
        out OneofSide? sourceSide,
        out OneofSide? targetSide)
    {
        scenario = ProtobufPolymorphismScenario.ProtoToPoco;
        sourceSide = null;
        targetSide = null;

        var sourceProperty = ProtobufOneofHelper.FindProperty(this.context.SourceType, propertyName);
        var targetProperty = ProtobufOneofHelper.FindProperty(this.context.TargetType, propertyName);
        if (sourceProperty is not null && targetProperty is not null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismSetupWrongArity(
                rootMapMethod.Location,
                rootMapMethod.MethodName,
                $"both '{this.context.SourceType.ToDisplayString()}' and '{this.context.TargetType.ToDisplayString()}' define '{propertyName}'; use [MappaProtobufPolymorphismSetup(targetOneOfCasePropertyName, sourceOneOfCasePropertyName)] for protobuf to protobuf mappings."));
            return false;
        }

        if (sourceProperty is not null)
        {
            scenario = ProtobufPolymorphismScenario.ProtoToPoco;
            return this.TryCreateSide(this.context.SourceType, sourceProperty, rootMapMethod, out sourceSide);
        }

        if (targetProperty is not null)
        {
            scenario = ProtobufPolymorphismScenario.PocoToProto;
            return this.TryCreateSide(this.context.TargetType, targetProperty, rootMapMethod, out targetSide);
        }

        var missingType = !ProtobufOneofHelper.HasAnyOneofCaseEnum(this.context.SourceType)
                          && ProtobufOneofHelper.HasAnyOneofCaseEnum(this.context.TargetType)
            ? this.context.TargetType
            : this.context.SourceType;
        this.ReportCasePropertyNotFound(rootMapMethod, propertyName, missingType);
        return false;
    }

    private bool TryResolveTwoProperties(
        string targetPropertyName,
        string sourcePropertyName,
        MapMethod rootMapMethod,
        out ProtobufPolymorphismScenario scenario,
        out OneofSide? sourceSide,
        out OneofSide? targetSide)
    {
        scenario = ProtobufPolymorphismScenario.ProtoToProto;
        sourceSide = null;
        targetSide = null;

        var sourceProperty = ProtobufOneofHelper.FindProperty(this.context.SourceType, sourcePropertyName);
        var targetProperty = ProtobufOneofHelper.FindProperty(this.context.TargetType, targetPropertyName);
        if (sourceProperty is not null && targetProperty is not null)
        {
            return this.TryCreateSide(this.context.SourceType, sourceProperty, rootMapMethod, out sourceSide)
                   && this.TryCreateSide(this.context.TargetType, targetProperty, rootMapMethod, out targetSide);
        }

        var sourceIsMissingAndLooksLikeMessage = sourceProperty is null && ProtobufOneofHelper.HasAnyOneofCaseEnum(this.context.SourceType);
        var targetIsMissingAndLooksLikeMessage = targetProperty is null && ProtobufOneofHelper.HasAnyOneofCaseEnum(this.context.TargetType);
        if ((sourceProperty is null && targetProperty is null)
            || sourceIsMissingAndLooksLikeMessage
            || targetIsMissingAndLooksLikeMessage)
        {
            if (sourceProperty is null)
            {
                this.ReportCasePropertyNotFound(rootMapMethod, sourcePropertyName, this.context.SourceType);
            }

            if (targetProperty is null)
            {
                this.ReportCasePropertyNotFound(rootMapMethod, targetPropertyName, this.context.TargetType);
            }

            return false;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismSetupWrongArity(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            $"only the {(sourceProperty is null ? "target" : "source")} type defines a oneof case property; use [MappaProtobufPolymorphismSetup(oneOfCasePropertyName)] for protobuf to POCO or POCO to protobuf mappings."));
        return false;
    }

    private void ReportCasePropertyNotFound(MapMethod rootMapMethod, string propertyName, ITypeSymbol type)
        => this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismCasePropertyNotFound(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            propertyName,
            type.ToDisplayString()));

    private bool TryCreateSide(
        ITypeSymbol messageType,
        IPropertySymbol caseProperty,
        MapMethod rootMapMethod,
        out OneofSide? side)
    {
        side = null;
        if (ProtobufOneofHelper.GetCaseEnum(caseProperty) is not { } caseEnum)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismCasePropertyNotOneofCaseEnum(
                rootMapMethod.Location,
                rootMapMethod.MethodName,
                caseProperty.Name,
                messageType.ToDisplayString()));
            return false;
        }

        side = new OneofSide(messageType, caseProperty, caseEnum);
        return true;
    }

    private bool TryValidateDefaultAttribute(
        MappaProtobufTypeMappingDefaultAttribute defaultAttribute,
        ProtobufPolymorphismScenario scenario,
        MapMethod rootMapMethod)
    {
        var behavior = defaultAttribute.Behavior;
        switch (behavior)
        {
            case MappaProtobufTypeMappingDefaultBehavior.Throw:
                this.WarnWhenUnused(defaultAttribute.TargetOneOfCaseName, "target oneof case name", behavior, rootMapMethod);
                return this.ValidateExceptionType(defaultAttribute, rootMapMethod);

            case MappaProtobufTypeMappingDefaultBehavior.NoMap:
                this.WarnWhenUnused(defaultAttribute.TargetOneOfCaseName, "target oneof case name", behavior, rootMapMethod);
                this.WarnWhenUnused(defaultAttribute.ExceptionType?.FullName, "exception type", behavior, rootMapMethod);
                return this.ValidateTargetIsProtobuf(behavior, scenario, rootMapMethod);

            case MappaProtobufTypeMappingDefaultBehavior.MapTo:
                this.WarnWhenUnused(defaultAttribute.ExceptionType?.FullName, "exception type", behavior, rootMapMethod);
                return this.ValidateTargetIsProtobuf(behavior, scenario, rootMapMethod)
                       && this.ValidateMapToCaseName(defaultAttribute, rootMapMethod);

            default:
                this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismDefaultBehaviorUndefined(
                    rootMapMethod.Location,
                    rootMapMethod.MethodName));
                return false;
        }
    }

    private bool ValidateExceptionType(MappaProtobufTypeMappingDefaultAttribute defaultAttribute, MapMethod rootMapMethod)
    {
        var exceptionDiagnostics = new List<Diagnostic>();
        var isExceptionValid = MappaTypeMappingDefaultAttributeExtensions.ValidateExceptionType(
            defaultAttribute.ExceptionType,
            this.compilation,
            rootMapMethod.Location,
            exceptionDiagnostics);
        foreach (var exceptionDiagnostic in exceptionDiagnostics)
        {
            this.context.ReportDiagnostic(exceptionDiagnostic);
        }

        return isExceptionValid;
    }

    private void WarnWhenUnused(string? argumentValue, string argumentName, MappaProtobufTypeMappingDefaultBehavior behavior, MapMethod rootMapMethod)
    {
        if (argumentValue is null)
        {
            return;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismDefaultArgumentUnused(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            argumentName,
            behavior.ToString()));
    }

    private bool ValidateTargetIsProtobuf(
        MappaProtobufTypeMappingDefaultBehavior behavior,
        ProtobufPolymorphismScenario scenario,
        MapMethod rootMapMethod)
    {
        if (scenario is not ProtobufPolymorphismScenario.ProtoToPoco)
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismDefaultRequiresProtobufTarget(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            behavior.ToString(),
            this.context.TargetType.ToDisplayString()));
        return false;
    }

    private bool ValidateMapToCaseName(MappaProtobufTypeMappingDefaultAttribute defaultAttribute, MapMethod rootMapMethod)
    {
        if (!string.IsNullOrWhiteSpace(defaultAttribute.TargetOneOfCaseName))
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismMapToCaseMissing(
            rootMapMethod.Location,
            rootMapMethod.MethodName));
        return false;
    }

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
        arm = new ProtobufPolymorphismArm(new NoMapStrategy(this.context.TargetType, this.context.SourceType), null, null, null);
        switch (scenario)
        {
            case ProtobufPolymorphismScenario.ProtoToPoco
                when sourceSide is not null && attribute.TargetType is not null && attribute.SourceOneOfCaseName is not null:
                return this.TryBuildProtoToPocoArm(attribute.TargetType, attribute.SourceOneOfCaseName, sourceSide, rootMapMethod, seenArmKeys, out arm);

            case ProtobufPolymorphismScenario.PocoToProto
                when targetSide is not null && attribute.TargetOneOfCaseName is not null && attribute.SourceType is not null:
                return this.TryBuildPocoToProtoArm(attribute.TargetOneOfCaseName, attribute.SourceType, targetSide, rootMapMethod, seenArmKeys, out arm);

            case ProtobufPolymorphismScenario.ProtoToProto
                when sourceSide is not null && targetSide is not null && attribute.TargetOneOfCaseName is not null && attribute.SourceOneOfCaseName is not null:
                return this.TryBuildProtoToProtoArm(attribute.TargetOneOfCaseName, attribute.SourceOneOfCaseName, sourceSide, targetSide, rootMapMethod, seenArmKeys, out arm);

            default:
                this.ReportArmFormMismatch(scenario, rootMapMethod);
                return false;
        }
    }

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
        arm = new ProtobufPolymorphismArm(new NoMapStrategy(this.context.TargetType, this.context.SourceType), null, null, null);
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
        arm = new ProtobufPolymorphismArm(new NoMapStrategy(this.context.TargetType, this.context.SourceType), null, null, null);
        if (!this.TryResolveWritableArmProperty(targetSide, targetCaseName, rootMapMethod, out var targetArmProperty)
            || !this.TryRegisterArm(seenArmKeys, attributeSourceType.FullName ?? string.Empty, rootMapMethod)
            || !this.TryLoadCompatibleType(attributeSourceType, this.context.SourceType, rootMapMethod, out var clrSourceType))
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
        arm = new ProtobufPolymorphismArm(new NoMapStrategy(this.context.TargetType, this.context.SourceType), null, null, null);
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

    private sealed class OneofSide(ITypeSymbol messageType, IPropertySymbol caseProperty, INamedTypeSymbol caseEnum)
    {
        internal ITypeSymbol MessageType { get; } = messageType;

        internal IPropertySymbol CaseProperty { get; } = caseProperty;

        internal INamedTypeSymbol CaseEnum { get; } = caseEnum;
    }
}