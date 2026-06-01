// <copyright file="MapMethodStrategyWithInheritedMapMethodMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Samples.Models;

#pragma warning disable SA1402 // Multiple classes in the same file
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable S1118 // Add a 'private' constructor or the 'static' keyword to the class declaration.
#pragma warning disable S2094 // Remove this empty class, write its code or make it an "interface".
#pragma warning disable S2325 // Make 'Map' a static method.
#pragma warning disable CA1822 // Member 'Map' does not access instance data and can be marked as static
#pragma warning disable CA1812
#pragma warning disable CA1852
#pragma warning disable SA1518 // File may not end with a newline character

namespace Mappa.Samples;

/// <summary>
/// Base class providing a map method for inherited mapper samples.
/// </summary>
public class MapMethodStrategyInheritedMapperBase
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The target model.</returns>
    public TargetClassModel Map(SourceClassModel input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new TargetClassModel
        {
            ParamA = input.ParamA.ToString(CultureInfo.InvariantCulture),
            ParamB = (int)input.ParamB,
        };
    }
}

/// <summary>
/// Mapper that inherits a map method from a base class.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyWithMapperBaseClass : MapMethodStrategyInheritedMapperBase
{
    /// <summary>
    /// Map from <see cref="SourceClassWithInnerClassModel"/> to <see cref="TargetClassWithInnerClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}

/// <summary>
/// Mapper that uses a map method from a base class of a dependency property type.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyWithDependencyPropertyBaseClass
{
    [MappaDependency]
    private MapMethodStrategyInheritedDerivedDependency DependencyProperty { get; } = new();

    /// <summary>
    /// Map from <see cref="SourceClassWithInnerClassModel"/> to <see cref="TargetClassWithInnerClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}

/// <summary>
/// Mapper that uses a map method from a base class of a dependency field type.
/// </summary>
[Mappa]
public sealed partial class MapMethodStrategyWithDependencyFieldBaseClass
{
    [MappaDependency]
    private MapMethodStrategyInheritedDerivedDependency dependencyField = new();

    /// <summary>
    /// Map from <see cref="SourceClassWithInnerClassModel"/> to <see cref="TargetClassWithInnerClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}

/// <summary>
/// Base class providing a map method for inherited dependency samples.
/// </summary>
internal class MapMethodStrategyInheritedDependencyBase
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The target model.</returns>
    internal TargetClassModel Map(SourceClassModel input)
    {
        return new TargetClassModel
        {
            ParamA = input.ParamA.ToString(CultureInfo.InvariantCulture),
            ParamB = (int)input.ParamB + 50,
        };
    }
}

/// <summary>
/// Derived dependency type used by inherited dependency samples.
/// </summary>
internal sealed class MapMethodStrategyInheritedDerivedDependency : MapMethodStrategyInheritedDependencyBase
{
}