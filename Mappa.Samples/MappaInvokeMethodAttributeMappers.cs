// <copyright file="MappaInvokeMethodAttributeMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

#pragma warning disable SA1402 // File may only contain a single type
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