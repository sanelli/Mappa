// <copyright file="ProjectionCapabilityAnalyzer.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Determines whether a mapping strategy can be expressed as a queryable projection expression.
/// </summary>
internal static class ProjectionCapabilityAnalyzer
{
    /// <summary>
    /// Returns <c>true</c> when <paramref name="strategy"/> can be translated into a projection expression.
    /// </summary>
    /// <param name="strategy">The strategy to analyze.</param>
    /// <returns><c>true</c> when the strategy is supported.</returns>
    internal static bool IsSupported(MapStrategy strategy)
        => strategy switch
        {
            IdentityMapStrategy identityMapStrategy => IsIdentitySupported(identityMapStrategy),
            NullableStrategy nullableStrategy => IsSupported(nullableStrategy.ElementStrategy),
            InvokeConstructorMapStrategy invokeConstructorMapStrategy => IsConstructorSupported(invokeConstructorMapStrategy),
            ParameterMapStrategy parameterMapStrategy => IsSupported(parameterMapStrategy.ParameterStrategy),
            PropertyMapStrategy propertyMapStrategy => !propertyMapStrategy.PostConstructorInitializer
                                                         && IsSupported(propertyMapStrategy.PropertyStrategy),
            EnumToEnumMapStrategy => true,
            EnumToIntegralMapStrategy => true,
            EnumToStringMapStrategy => true,
            IntegralToEnumMapStrategy => true,
            StringToEnumMapStrategy => true,
            InvokeParseMethodMapStrategy => true,
            InvokeToStringMapStrategy => true,
            InvokeParseStringWithFormatMapStrategy => true,
            InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy => true,
            StringToNumberMapStrategy => true,
            StringToUriMapStrategy => true,
            DateOnlyToDateTimeMapStrategy => true,
            DateOnlyToLongMapStrategy => true,
            DateTimeOffsetToDateOnlyMapStrategy => true,
            DateTimeOffsetToDateTimeMapStrategy => true,
            DateTimeOffsetToLongMapStrategy => true,
            DateTimeOffsetToTimeOnlyMapStrategy => true,
            DateTimeToDateOnlyMapStrategy => true,
            DateTimeToLongMapStrategy => true,
            DateTimeToTimeOnlyMapStrategy => true,
            DoubleToTimeSpanMapStrategy => true,
            LongToDateTimeMapStrategy => true,
            LongToDateTimeOffsetMapStrategy => true,
            TimeSpanToDoubleMapStrategy => true,
            _ => false,
        };

    private static bool IsIdentitySupported(IdentityMapStrategy identityMapStrategy)
        => identityMapStrategy.IdentityMapDeepCopySetting is IdentityMapDeepCopySetting.ShallowCopy
           && !identityMapStrategy.RequiresMemberwiseClone
           && identityMapStrategy.NestedFieldStrategies.Count == 0;

    private static bool IsConstructorSupported(InvokeConstructorMapStrategy invokeConstructorMapStrategy)
    {
        if (invokeConstructorMapStrategy.AssignToContextEntries.Length > 0
            || invokeConstructorMapStrategy.ContextParameterName is not null)
        {
            return false;
        }

        if (invokeConstructorMapStrategy.InitializerStrategies.Any(propertyMapStrategy => propertyMapStrategy.PostConstructorInitializer))
        {
            return false;
        }

        return invokeConstructorMapStrategy.ParametersMapStrategies.All(IsSupported)
               && invokeConstructorMapStrategy.InitializerStrategies.All(propertyMapStrategy =>
                   IsSupported(propertyMapStrategy.PropertyStrategy));
    }
}