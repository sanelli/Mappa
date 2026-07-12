// <copyright file="RelaxedNullabilityMethodMapMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

#pragma warning disable SA1402

/// <summary>
/// Mapper that reuses an existing non-nullable map method when the nested mapping requires a nullable target type.
/// </summary>
[Mappa]
public sealed partial class RelaxedNullabilityMethodMapMapper
{
    private readonly int valueOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelaxedNullabilityMethodMapMapper"/> class.
    /// </summary>
    /// <param name="valueOffset">An optional offset applied to mapped values.</param>
    public RelaxedNullabilityMethodMapMapper(int valueOffset = 0)
    {
        this.valueOffset = valueOffset;
    }

    /// <summary>
    /// Map from <see cref="RelaxedNullabilityInnerSource"/> to <see cref="RelaxedNullabilityInnerTarget"/>.
    /// </summary>
    /// <param name="input">The source inner model.</param>
    /// <returns>The mapped inner target.</returns>
    public RelaxedNullabilityInnerTarget Map(RelaxedNullabilityInnerSource input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new RelaxedNullabilityInnerTarget
        {
            Value = input.Value + this.valueOffset,
        };
    }

    /// <summary>
    /// Map from <see cref="RelaxedNullabilitySource"/> to <see cref="RelaxedNullabilityTarget"/>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The target model.</returns>
    public partial RelaxedNullabilityTarget Map(RelaxedNullabilitySource input);
}

/// <summary>
/// Mapper that reuses an existing method accepting a nullable source parameter when the nested mapping uses a non-nullable source type.
/// </summary>
[Mappa]
public sealed partial class RelaxedNullabilityMethodMapWithNullableParameterMapper
{
    private readonly int valueOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelaxedNullabilityMethodMapWithNullableParameterMapper"/> class.
    /// </summary>
    /// <param name="valueOffset">An optional offset applied to mapped values.</param>
    public RelaxedNullabilityMethodMapWithNullableParameterMapper(int valueOffset = 0)
    {
        this.valueOffset = valueOffset;
    }

    /// <summary>
    /// Map from <see cref="RelaxedNullabilityInnerSource"/> to <see cref="RelaxedNullabilityInnerTarget"/>.
    /// </summary>
    /// <param name="input">The source inner model.</param>
    /// <returns>The mapped inner target.</returns>
    public RelaxedNullabilityInnerTarget Map(RelaxedNullabilityInnerSource? input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new RelaxedNullabilityInnerTarget
        {
            Value = input.Value + this.valueOffset,
        };
    }

    /// <summary>
    /// Map from <see cref="RelaxedNullabilitySource"/> to <see cref="RelaxedNullabilityTargetWithRequiredInner"/>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The target model.</returns>
    public partial RelaxedNullabilityTargetWithRequiredInner Map(RelaxedNullabilitySource input);
}

/// <summary>
/// Inner source model for <see cref="RelaxedNullabilityMethodMapMapper"/>.
/// </summary>
public sealed class RelaxedNullabilityInnerSource
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Inner target model for <see cref="RelaxedNullabilityMethodMapMapper"/>.
/// </summary>
public sealed class RelaxedNullabilityInnerTarget
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Source model with a non-nullable inner property.
/// </summary>
public sealed class RelaxedNullabilitySource
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public RelaxedNullabilityInnerSource Inner { get; set; } = new();
}

/// <summary>
/// Target model with a nullable inner property.
/// </summary>
public sealed class RelaxedNullabilityTarget
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public RelaxedNullabilityInnerTarget? Inner { get; set; }
}

/// <summary>
/// Target model with a non-nullable inner property.
/// </summary>
public sealed class RelaxedNullabilityTargetWithRequiredInner
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public required RelaxedNullabilityInnerTarget Inner { get; set; }
}