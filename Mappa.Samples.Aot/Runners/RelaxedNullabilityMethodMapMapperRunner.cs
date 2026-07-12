// <copyright file="RelaxedNullabilityMethodMapMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for relaxed nullability method map sample classes.
/// </summary>
internal static class RelaxedNullabilityMethodMapMapperRunner
{
    /// <summary>
    /// Runs all map methods on the relaxed nullability method map sample classes.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var source = new RelaxedNullabilitySource
        {
            Inner = new RelaxedNullabilityInnerSource
            {
                Value = 42,
            },
        };

        report.BeginMapper(nameof(RelaxedNullabilityMethodMapMapper));
        var returnRelaxationMapper = new RelaxedNullabilityMethodMapMapper();
        report.RecordInvocation(
            nameof(RelaxedNullabilityMethodMapMapper.Map),
            nameof(RelaxedNullabilitySource),
            nameof(RelaxedNullabilityTarget),
            source,
            returnRelaxationMapper.Map(source));

        report.BeginMapper(nameof(RelaxedNullabilityMethodMapWithNullableParameterMapper));
        var parameterRelaxationMapper = new RelaxedNullabilityMethodMapWithNullableParameterMapper();
        report.RecordInvocation(
            nameof(RelaxedNullabilityMethodMapWithNullableParameterMapper.Map),
            nameof(RelaxedNullabilitySource),
            nameof(RelaxedNullabilityTargetWithRequiredInner),
            source,
            parameterRelaxationMapper.Map(source));
    }
}