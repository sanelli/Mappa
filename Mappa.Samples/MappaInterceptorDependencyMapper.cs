// <copyright file="MappaInterceptorDependencyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Interceptor dependency samples share this file by design.
#pragma warning disable SA1649 // File name need not match the first type; samples share this file by design.

using System.Globalization;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Static helper used as an interceptor static dependency.
/// </summary>
public static class InterceptorStaticDependencyHelper
{
    /// <summary>
    /// Map an integer identity used as a nested dependency.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The mapped value.</returns>
    public static int Map(int value) => value;
}

/// <summary>
/// Sample mapper demonstrating dual <see cref="MappaAttribute"/> + <see cref="MappaInterceptorAttribute"/>
/// so classic map methods are eligible as interceptor dependencies.
/// </summary>
[Mappa]
[MappaInterceptor]
public sealed partial class MappaInterceptorDualMapper
{
    /// <summary>
    /// Classic map method used as a dependency from the interceptor parent map.
    /// </summary>
    /// <param name="value">The child source.</param>
    /// <returns>The mapped child.</returns>
    public partial InterceptorChildTarget MapChild(InterceptorChildSource value);

    /// <summary>
    /// Map a parent graph on the interceptor route, reusing <see cref="MapChild"/>.
    /// </summary>
    /// <param name="value">The parent source.</param>
    /// <returns>The mapped parent.</returns>
    public InterceptorParentTarget MapParent(InterceptorParentSource value)
        => this.MappaMap<InterceptorParentTarget>(value);
}

/// <summary>
/// Sample mapper demonstrating <see cref="MappaInterceptorMethodDependencyAttribute"/>.
/// </summary>
[MappaInterceptor]
public sealed partial class MappaInterceptorMethodDependencyMapper
{
    /// <summary>
    /// Hand-written dependency used only on the interceptor generation path.
    /// </summary>
    /// <param name="value">The child source.</param>
    /// <returns>The mapped child.</returns>
    [MappaInterceptorMethodDependency]
    public static InterceptorChildTarget MapChildDependency(InterceptorChildSource value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new InterceptorChildTarget
        {
            Code = value.Code.ToString(CultureInfo.InvariantCulture),
        };
    }

    /// <summary>
    /// Map a parent graph using a helper marked for interceptor-path dependency use.
    /// </summary>
    /// <param name="value">The parent source.</param>
    /// <returns>The mapped parent.</returns>
    public InterceptorParentTarget MapParent(InterceptorParentSource value)
        => this.MappaMap<InterceptorParentTarget>(value);
}

/// <summary>
/// Sample mapper demonstrating a static dependency supplied via <c>With</c>.
/// </summary>
[MappaInterceptor]
public sealed partial class MappaInterceptorStaticDependencyMapper
{
    /// <summary>
    /// Map using a static dependency registered through <c>With</c> for a nested conversion.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorRenameTarget MapWithStaticDependency(InterceptorRenameSource value)
    {
        return this.MappaMap<InterceptorRenameTarget>(value)
            .With(
                mappaStaticDependencies:
                [
                    new MappaStaticDependencyAttribute(typeof(InterceptorStaticDependencyHelper)),
                ])
            .With(
                mappaMethodAttributes:
                [
                    new MappaUsePropertyAttribute(
                        nameof(InterceptorRenameTarget.TargetValue),
                        nameof(InterceptorRenameSource.SourceValue)),
                ]);
    }
}

/// <summary>
/// Sample mapper demonstrating field mapping via <c>With</c> settings.
/// </summary>
[MappaInterceptor]
public sealed partial class MappaInterceptorFieldMapper
{
    /// <summary>
    /// Map instance fields when enabled via <see cref="MappaSettingsAttribute"/> on <c>With</c>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorFieldTarget MapFields(InterceptorFieldSource value)
    {
        return this.MappaMap<InterceptorFieldTarget>(value)
            .With(new MappaSettingsAttribute
            {
                MapSourceFields = BooleanSetting.Enable,
                MapTargetFields = BooleanSetting.Enable,
            });
    }
}