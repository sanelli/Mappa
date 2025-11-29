// <copyright file="PolymorphismMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402

using Mappa.Attributes;

// TODO [#49] Add more mappers to cover different defaults.
namespace Mappa.Samples;

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapper
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultAttribute"/> default
    /// value (i.e. throw if the input is neither of the expected input values).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>
/// with nullability.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperNullable
{
    /// <summary>
    /// Map from nullable <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to nullable <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultAttribute"/> default
    /// value (i.e. throw if the input is neither of the expected input values).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    public partial Models.Polymorphism.One.TargetBaseClass? Map(Models.Polymorphism.One.SourceBaseClass? source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperBetweenInterfaces
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.Two.ISourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.Two.ITargetBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultAttribute"/> default
    /// value (i.e. throw if the input is neither of the expected input values).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.Two.TargetFirstClass), typeof(Models.Polymorphism.Two.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.Two.TargetThirdClass), typeof(Models.Polymorphism.Two.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.Two.TargetSecondClass), typeof(Models.Polymorphism.Two.SourceSecondClass))]
    public partial Models.Polymorphism.Two.ITargetBaseClass Map(Models.Polymorphism.Two.ISourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>
/// overriding the identity mapping.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperOverridingIdentityMapper
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.Three.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.Three.SourceBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultAttribute"/> default
    /// value (i.e. throw if the input is neither of the expected input values).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.Three.SourceSecondClass), typeof(Models.Polymorphism.Three.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.Three.SourceFirstClass), typeof(Models.Polymorphism.Three.SourceSecondClass))]
    public partial Models.Polymorphism.Three.SourceBaseClass Map(Models.Polymorphism.Three.SourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>
/// overriding the identity mapping but using nullability.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperOverridingIdentityMapperWithNullable
{
    /// <summary>
    /// Map from nullable <see cref="Models.Polymorphism.Three.SourceBaseClass"/>
    /// to nullable <see cref="Models.Polymorphism.Three.SourceBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultAttribute"/> default
    /// value (i.e. throw if the input is neither of the expected input values).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.Three.SourceSecondClass), typeof(Models.Polymorphism.Three.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.Three.SourceFirstClass), typeof(Models.Polymorphism.Three.SourceSecondClass))]
    public partial Models.Polymorphism.Three.SourceBaseClass? Map(Models.Polymorphism.Three.SourceBaseClass? source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>
/// with <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
/// for <see cref="MappaTypeMappingDefaultAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperWithThrowDefaultBehaviour
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// for <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw)]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>
/// with <see cref="MappaTypeMappingDefaultBehavior.Throw"/> and custom exception
/// for <see cref="MappaTypeMappingDefaultAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperWithThrowDefaultAndCustomExceptionBehaviour
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// This will use the <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// for <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Throw, typeof(Models.Polymorphism.PolymorphismCustomException))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/> with
/// <see cref="MappaTypeMappingDefaultAttribute"/> and behavior
/// <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> without
/// specific type.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperWithMapDefaultWithoutExplicitType
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType)]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/> with
/// <see cref="MappaTypeMappingDefaultAttribute"/> and behavior
/// <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> with
/// specific target type.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphismMapperWithMapDefaultWithExplicitType
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(Models.Polymorphism.One.TargetUnmappedBaseClass))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}