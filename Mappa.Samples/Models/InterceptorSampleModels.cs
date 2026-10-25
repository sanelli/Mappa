// <copyright file="InterceptorSampleModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Interceptor sample models share this file by design.
#pragma warning disable SA1649 // File name need not match the first type; models are grouped for the interceptor samples.

namespace Mappa.Samples.Models;

/// <summary>
/// Source enum for interceptor enum-mapping samples.
/// </summary>
public enum InterceptorSourceStatus
{
    /// <summary>
    /// Active status.
    /// </summary>
    Active,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive,
}

/// <summary>
/// Target enum for interceptor enum-mapping samples.
/// </summary>
public enum InterceptorTargetStatus
{
    /// <summary>
    /// Active status.
    /// </summary>
    Active,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive,
}

/// <summary>
/// Person source used by interceptor structured-mapping samples.
/// </summary>
public sealed class InterceptorPersonSource
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }
}

/// <summary>
/// Person target used by interceptor structured-mapping samples.
/// </summary>
public sealed class InterceptorPersonTarget
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }
}

/// <summary>
/// Source with differently named members for <c>MappaUseProperty</c> interceptor samples.
/// </summary>
public sealed class InterceptorRenameSource
{
    /// <summary>
    /// Gets or sets the source value.
    /// </summary>
    public int SourceValue { get; set; }
}

/// <summary>
/// Target with differently named members for <c>MappaUseProperty</c> interceptor samples.
/// </summary>
public sealed class InterceptorRenameTarget
{
    /// <summary>
    /// Gets or sets the target value.
    /// </summary>
    public int TargetValue { get; set; }
}

/// <summary>
/// Target filled from a constant on the interceptor route.
/// </summary>
public sealed class InterceptorConstantTarget
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Target filled from <see cref="Mappa.MappaContext"/> on the interceptor route.
/// </summary>
public sealed class InterceptorContextTarget
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Source with a keep/drop pair for ignore-target interceptor samples.
/// </summary>
public sealed class InterceptorKeepDropSource
{
    /// <summary>
    /// Gets or sets the kept value.
    /// </summary>
    public int Keep { get; set; }

    /// <summary>
    /// Gets or sets the dropped value.
    /// </summary>
    public int Drop { get; set; }
}

/// <summary>
/// Target with a keep/drop pair for ignore-target interceptor samples.
/// </summary>
public sealed class InterceptorKeepDropTarget
{
    /// <summary>
    /// Gets or sets the kept value.
    /// </summary>
    public int Keep { get; set; }

    /// <summary>
    /// Gets or sets the dropped value (left at default when ignored).
    /// </summary>
    public int Drop { get; set; }
}

/// <summary>
/// Source exposing an instance field for interceptor field-mapping samples.
/// </summary>
public sealed class InterceptorFieldSource
{
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Field should be private
#pragma warning disable S1104 // Fields should not be public
    /// <summary>
    /// The value field.
    /// </summary>
    public int Value;
#pragma warning restore S1104
#pragma warning restore SA1401
#pragma warning restore CA1051
}

/// <summary>
/// Target exposing an instance field for interceptor field-mapping samples.
/// </summary>
public sealed class InterceptorFieldTarget
{
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Field should be private
#pragma warning disable S1104 // Fields should not be public
    /// <summary>
    /// The value field.
    /// </summary>
    public int Value;
#pragma warning restore S1104
#pragma warning restore SA1401
#pragma warning restore CA1051
}

/// <summary>
/// Counter mutated by before/after map hooks on the interceptor route.
/// </summary>
public sealed class InterceptorHookCounter
{
    /// <summary>
    /// Gets or sets the counter value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Parent source with a nested child for interceptor method-dependency samples.
/// </summary>
public sealed class InterceptorParentSource
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public InterceptorChildSource Child { get; set; } = new();
}

/// <summary>
/// Nested child source for interceptor method-dependency samples.
/// </summary>
public sealed class InterceptorChildSource
{
    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    public int Code { get; set; }
}

/// <summary>
/// Parent target with a nested child for interceptor method-dependency samples.
/// </summary>
public sealed class InterceptorParentTarget
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public InterceptorChildTarget Child { get; set; } = new();
}

/// <summary>
/// Nested child target for interceptor method-dependency samples.
/// </summary>
public sealed class InterceptorChildTarget
{
    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}