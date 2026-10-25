// <copyright file="MappaInterceptorStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Sample mapper demonstrating major automatic mapping strategies via <see cref="MappaInterceptorAttribute"/>.
/// </summary>
#pragma warning disable CA1002 // List is used intentionally to showcase collection mapping.
#pragma warning disable CS8604 // Nullable value types are boxed into the object MappaMap stub parameter.
[MappaInterceptor]
public sealed partial class MappaInterceptorStrategyMapper
{
    /// <summary>
    /// Unwrap a nullable value type on the interceptor route.
    /// </summary>
    /// <param name="value">The nullable source.</param>
    /// <returns>The non-nullable mapped value.</returns>
    public long MapNullable(int? value) => this.MappaMap<long>(value);

    /// <summary>
    /// Map an enum to another enum on the interceptor route.
    /// </summary>
    /// <param name="value">The source enum.</param>
    /// <returns>The mapped enum.</returns>
    public InterceptorTargetStatus MapEnum(InterceptorSourceStatus value)
        => this.MappaMap<InterceptorTargetStatus>(value);

    /// <summary>
    /// Parse a string to <see cref="int"/> on the interceptor route.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>The parsed integer.</returns>
    public int MapStringToInt(string value) => this.MappaMap<int>(value);

    /// <summary>
    /// Convert Unix seconds to <see cref="DateTimeOffset"/> on the interceptor route.
    /// </summary>
    /// <param name="value">Unix time in seconds.</param>
    /// <returns>The mapped offset.</returns>
    public DateTimeOffset MapLongToDateTimeOffset(long value) => this.MappaMap<DateTimeOffset>(value);

    /// <summary>
    /// Map a list on the interceptor route.
    /// </summary>
    /// <param name="value">The source list.</param>
    /// <returns>The mapped list.</returns>
    public List<string> MapList(List<int> value) => this.MappaMap<List<string>>(value);

    /// <summary>
    /// Map a dictionary on the interceptor route.
    /// </summary>
    /// <param name="value">The source dictionary.</param>
    /// <returns>The mapped dictionary.</returns>
    public Dictionary<string, long> MapDictionary(Dictionary<string, int> value)
        => this.MappaMap<Dictionary<string, long>>(value);

    /// <summary>
    /// Map a tuple on the interceptor route.
    /// </summary>
    /// <param name="value">The source tuple.</param>
    /// <returns>The mapped tuple.</returns>
    public Tuple<string, string> MapTuple(Tuple<int, string> value)
        => this.MappaMap<Tuple<string, string>>(value);

    /// <summary>
    /// Map a <see cref="Guid"/> to a byte array on the interceptor route.
    /// </summary>
    /// <param name="value">The source guid.</param>
    /// <returns>The mapped byte array.</returns>
    public byte[] MapGuid(Guid value) => this.MappaMap<byte[]>(value);

    /// <summary>
    /// Map a structured type with matching properties on the interceptor route.
    /// </summary>
    /// <param name="value">The source person.</param>
    /// <returns>The mapped person.</returns>
    public InterceptorPersonTarget MapPerson(InterceptorPersonSource value)
        => this.MappaMap<InterceptorPersonTarget>(value);
}
#pragma warning restore CS8604
#pragma warning restore CA1002