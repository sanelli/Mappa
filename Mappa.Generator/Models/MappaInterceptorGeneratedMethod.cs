// <copyright file="MappaInterceptorGeneratedMethod.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Models;

/// <summary>
/// A successfully strategized interceptor method ready for emission.
/// </summary>
internal sealed class MappaInterceptorGeneratedMethod
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInterceptorGeneratedMethod"/> class.
    /// </summary>
    /// <param name="mapMethod">The synthetic map method with strategy.</param>
    /// <param name="interceptableLocation">The interceptable call-site location.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable reference types are enabled at the call site.</param>
    internal MappaInterceptorGeneratedMethod(
        MapMethod mapMethod,
        InterceptableLocation interceptableLocation,
        bool nullableEnabled)
    {
        this.MapMethod = mapMethod;
        this.InterceptableLocation = interceptableLocation;
        this.NullableEnabled = nullableEnabled;
    }

    /// <summary>
    /// Gets the synthetic map method.
    /// </summary>
    internal MapMethod MapMethod { get; }

    /// <summary>
    /// Gets the interceptable location used for <c>[InterceptsLocation]</c>.
    /// </summary>
    internal InterceptableLocation InterceptableLocation { get; }

    /// <summary>
    /// Gets a value indicating whether nullable is enabled for this method.
    /// </summary>
    internal bool NullableEnabled { get; }
}