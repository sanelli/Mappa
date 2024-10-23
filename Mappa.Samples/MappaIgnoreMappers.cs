// <copyright file="MappaIgnoreMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable S1118 // Utility classes should not have public constructors

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to demonstrate the use of <see cref="MappaIgnoreAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaIgnoreLocalMethodMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// while ignoring <see cref="CustomIntToStringMapper"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The mapper model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);

    [MappaIgnore]
    private static string CustomIntToStringMapper(int input)
    {
        return $"This is custom {input}";
    }
}

/// <summary>
/// Mapper used to demonstrate the use of <see cref="MappaIgnoreAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaIgnoreDependencyMethodMapper
{
    [MappaDependency]
 #pragma warning disable CA1823
    private readonly DependencyMapper dependency = new();
 #pragma warning restore CA1823

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// while ignoring <see cref="DependencyMapper.CustomIntToStringMapper"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The mapper model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);
}

/// <summary>
/// Helper mapper.
/// </summary>
internal sealed class DependencyMapper
{
    /// <summary>
    /// Custom mapper.
    /// </summary>
    /// <param name="input">The integer input.</param>
    /// <returns>The string output.</returns>
    [MappaIgnore]
 #pragma warning disable CA1822
    internal string CustomIntToStringMapper(int input)
 #pragma warning restore CA1822
    {
        return $"This is custom {input}";
    }
}