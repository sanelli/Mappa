// <copyright file="ProtobufPolymorphismMapStrategyDetector.DefaultBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Default behavior validation for <see cref="ProtobufPolymorphismMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ProtobufPolymorphismMapStrategyDetector
{
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
}