// <copyright file="ProtobufPolymorphismMapStrategyDetector.Setup.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Setup (scenario and oneof sides resolution) for <see cref="ProtobufPolymorphismMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ProtobufPolymorphismMapStrategyDetector
{
    private static bool IsMissingCasePropertyOnProtobufMessage(IPropertySymbol? caseProperty, ITypeSymbol type)
        => caseProperty is null && ProtobufOneofHelper.HasAnyOneofCaseEnum(type);

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

        if (IsMissingCasePropertyOnProtobufMessage(sourceProperty, this.context.SourceType)
            || IsMissingCasePropertyOnProtobufMessage(targetProperty, this.context.TargetType)
            || (sourceProperty is null && targetProperty is null))
        {
            this.ReportMissingCasePropertiesForTwoArgSetup(
                rootMapMethod,
                sourceProperty is null ? sourcePropertyName : null,
                targetProperty is null ? targetPropertyName : null);
            return false;
        }

        this.ReportTwoArgSetupWrongArity(rootMapMethod, sourceProperty is null);
        return false;
    }

    private void ReportMissingCasePropertiesForTwoArgSetup(
        MapMethod rootMapMethod,
        string? missingSourcePropertyName,
        string? missingTargetPropertyName)
    {
        if (missingSourcePropertyName is not null)
        {
            this.ReportCasePropertyNotFound(rootMapMethod, missingSourcePropertyName, this.context.SourceType);
        }

        if (missingTargetPropertyName is not null)
        {
            this.ReportCasePropertyNotFound(rootMapMethod, missingTargetPropertyName, this.context.TargetType);
        }
    }

    private void ReportTwoArgSetupWrongArity(MapMethod rootMapMethod, bool sourceIsMissing)
        => this.context.ReportDiagnostic(MappaDiagnostics.ProtobufPolymorphismSetupWrongArity(
            rootMapMethod.Location,
            rootMapMethod.MethodName,
            $"only the {(sourceIsMissing ? "target" : "source")} type defines a oneof case property; use [MappaProtobufPolymorphismSetup(oneOfCasePropertyName)] for protobuf to POCO or POCO to protobuf mappings."));

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

    private sealed class OneofSide(ITypeSymbol messageType, IPropertySymbol caseProperty, INamedTypeSymbol caseEnum)
    {
        internal ITypeSymbol MessageType { get; } = messageType;

        internal IPropertySymbol CaseProperty { get; } = caseProperty;

        internal INamedTypeSymbol CaseEnum { get; } = caseEnum;
    }
}