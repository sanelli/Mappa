// <copyright file="MappaInvokeMethodAttributeMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable S1118 // Utility classes should not have public constructors
namespace Mappa.Samples;

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(SourceClassModel source, int property)
    {
        return $"{nameof(MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput)}/static/({nameof(SourceClassModel)},int)/{source.ParamA}/{source.ParamB}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceRecordModel"/> to <see cref="TargetRecordModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Non-empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceRecordModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceRecordModel"/> to <see cref="TargetRecordModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceRecordModel"/> and <see cref="int"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(SourceRecordModel.ParamA), nameof(CustomMap))]
    public partial TargetRecordModel Map(SourceRecordModel source);

    private static string CustomMap(SourceRecordModel source, int property)
    {
        return $"{nameof(MapNonEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput)}/static/({nameof(SourceRecordModel)},int)/{source.ParamA}/{source.ParamB}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(SourceClassModel source, int property)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndPropertyInput)}/non-static/({nameof(SourceClassModel)},int)/{source.ParamA}/{source.ParamB}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="object"/> and <see cref="int"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(object source, int property)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndPropertyInput)}/non-static/(object,int)/{source}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalStaticMethodWithSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="long"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(SourceClassModel source, long property)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassAndImplicitConvertiblePropertyInput)}/non-static/({nameof(SourceClassModel)},long)/{source.ParamA}/{source.ParamB}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="object"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="long"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(object source, long property)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConvertibleSourceClassAndImplicitConvertiblePropertyInput)}/non-static/(object,long)/{source}/{property}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput
{
    /// <summary>
    /// The mapping method.
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private string CustomMap(SourceClassModel source)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithSourceClassInput)}/not-static/({nameof(SourceClassModel)})/{source.ParamA}/{source.ParamB}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="object"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput
{
    /// <summary>
    /// The mapping method.
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private string CustomMap(object source)
    {
        return $"{nameof(MapEmptyConstructorWithLocalNonStaticMethodWithImplicitConversionFromSourceClassInput)}/not-static/(object))/{source}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(int source)
    {
        return $"{nameof(MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput)}/static/(int)/{source}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="long"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap(long source)
    {
        return $"{nameof(MapEmptyConstructorWithLocalMethodWithImplicitConversionFromSourcePropertyTypeInput)}/static/(long)/{source}";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithSourcePropertyTypeInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="long"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithLocalMethodWithNoParameters
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithLocalMethodWithNoParameters"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), nameof(CustomMap))]
    public partial TargetClassModel Map(SourceClassModel source);

    private static string CustomMap()
    {
        return $"{nameof(MapEmptyConstructorWithLocalMethodWithNoParameters)}/static/()";
    }
}

/// <summary>
/// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
/// using:
/// <list type="table">
/// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
/// <item><term>Custom method location</term><description><see cref="MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput"/>.</description></item>
/// <item><term>Custom method is static</term><description><c>true</c>.</description></item>
/// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
/// </list>
/// </summary>
[Mappa]
public sealed partial class MapEmptyConstructorWithTypeLocatedMethodWithSourceClassAndPropertyInput
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using:
    /// <list type="table">
    /// <item><term>Mapping mode</term><description>Empty constructor.</description></item>
    /// <item><term>Custom method location</term><description><see cref="MapperDependencyHelper"/>.</description></item>
    /// <item><term>Custom method is static</term><description><c>false</c>.</description></item>
    /// <item><term>Custom method input(s)</term><description><see cref="SourceClassModel"/> and <see cref="int"/>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="source">The source model to map.</param>
    /// <returns>The mapped object.</returns>
    [MappaInvokeMethod(nameof(TargetClassModel.ParamA), typeof(MapperDependencyHelper), nameof(MapperDependencyHelper.StaticMap1))]
    public partial TargetClassModel Map(SourceClassModel source);
}

// TODO [#54] Add missing tests to cover TypeAccess tests with all variants
// TODO [#54] Add missing tests to cover field tests with all variants
// TODO [#54] Add missing tests to cover property tests with all variants

/// <summary>
/// Mapper helper method that can be invoked by other classes.
/// </summary>
public sealed class MapperDependencyHelper
{
    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap1(SourceClassModel source, int property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap1)}/{source.ParamA}/{source.ParamB}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap2(object source, int property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap2)}/{source}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap3(SourceClassModel source, long property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap3)}/{source.ParamA}/{source.ParamB}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap4(object source, long property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap4)}/{source}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap5(SourceClassModel source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap5)}/{source.ParamA}/{source.ParamB}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap6(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(StaticMap6)}/{source}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap7(int property)
    {
        return $"{nameof(StaticMap7)}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public static string StaticMap8(long property)
    {
        return $"{nameof(StaticMap8)}/{property}";
    }

    /// <summary>
    /// Static map to a <see cref="string"/>.
    /// </summary>
    /// <returns>The mapped string.</returns>
    public static string StaticMap9()
    {
        return $"{nameof(StaticMap9)}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map1(SourceClassModel source, int property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map1)}/{source.ParamA}/{source.ParamB}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map2(object source, int property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map2)}/{source}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map3(SourceClassModel source, long property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map3)}/{source.ParamA}/{source.ParamB}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map4(object source, long property)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map4)}/{source}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The mapped string.</returns>
    public string Map5(SourceClassModel source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map5)}/{source.ParamA}/{source.ParamB}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The mapped string.</returns>
    public string Map6(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return $"{nameof(this.Map6)}/{source}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map7(int property)
    {
        return $"{nameof(this.Map7)}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The mapped string.</returns>
    public string Map8(long property)
    {
        return $"{nameof(this.Map8)}/{property}";
    }

    /// <summary>
    /// Map to a <see cref="string"/>.
    /// </summary>
    /// <returns>The mapped string.</returns>
    public string Map9()
    {
        return $"{nameof(this.Map9)}";
    }
}