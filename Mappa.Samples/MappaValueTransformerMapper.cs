// <copyright file="MappaValueTransformerMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaTransformerAttribute"/>, <see cref="MappaApplyTransformerAttribute"/>
/// and <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/>.
/// </summary>
/// <remarks>
/// Transformers run on the source value before it is mapped and on the mapped value before it is assigned.
/// Per mapping site the order is: <see cref="MappaApplyTransformerAttribute"/> declarations, local
/// <see cref="MappaTransformerAttribute"/> methods, then dependency transformers (only when enabled).
/// </remarks>
#pragma warning disable MP00016 // The static dependency only provides a value transformer, not a map method.
[Mappa]
[MappaStaticDependency(typeof(SpaceCollapser))]
public sealed partial class MappaValueTransformerMapper
#pragma warning restore MP00016
{
    /// <summary>
    /// Map a model trimming every string (class-level transformer) and upper-casing only <c>Code</c>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped model.</returns>
    [MappaApplyTransformer(nameof(ValueTransformerSourceModel.Code), nameof(ToUpper))]
    public partial ValueTransformerTargetModel MapTrimmed(ValueTransformerSourceModel input);

    /// <summary>
    /// Map a model truncating <c>Note</c> using the <c>maxLength</c> context value
    /// before the class-level trim runs.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The mapped model.</returns>
    [MappaApplyTransformer(nameof(ValueTransformerSourceModel.Note), nameof(Truncate))]
    public partial ValueTransformerNoteModel MapTruncated(ValueTransformerSourceModel input, MappaContext context);

    /// <summary>
    /// Map a model also running the transformers exposed by the <see cref="MappaStaticDependencyAttribute"/> dependency.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped model.</returns>
    [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
    public partial ValueTransformerLabelModel MapWithDependencyTransformers(ValueTransformerSourceModel input);

    [MappaTransformer]
    [MappaIgnore]
    private static string Trim(string value) => value.Trim();

    [MappaIgnore]
    private static string ToUpper(string value) => value.ToUpperInvariant();

    [MappaIgnore]
    private static string Truncate(string value, MappaContext context)
    {
        return context.TryGetValue<int>("maxLength", out var maxLength) && value.Length > maxLength
            ? value[..maxLength]
            : value;
    }

    private static class SpaceCollapser
    {
        [MappaTransformer]
        [MappaIgnore]
        public static string CollapseSpaces(string value) => string.Join(' ', value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}