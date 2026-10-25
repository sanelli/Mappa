// <copyright file="MappaInterceptorAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Marks a partial class for which the Mappa source generator emits interceptable
/// <c>MappaMap</c> (or renamed) instance stubs and C# interceptor methods for inline mapping call sites.
/// <para>
/// May be combined with <see cref="MappaAttribute"/> on the same class; the two attributes use
/// separate generator entry points that share the mapping algorithm where possible.
/// </para>
/// <para>
/// The tagged class must be <c>partial</c>. Configure call sites with
/// <see cref="MappaInterceptorExtensions.With{T}"/>; only <c>new</c> expressions are accepted
/// for <c>With</c> arguments.
/// </para>
/// </summary>
/// <param name="methodName">
/// The name of the interceptable instance mapping method emitted on the class. Defaults to <c>MappaMap</c>.
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MappaInterceptorAttribute(string methodName = "MappaMap")
    : Attribute
{
    /// <summary>
    /// Gets the name of the interceptable instance mapping method. Defaults to <c>MappaMap</c>.
    /// </summary>
    public string MethodName { get; } = methodName;
}