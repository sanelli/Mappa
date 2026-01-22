// <copyright file="PolymorphicMethodMapMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

// TODO [#185] Method is not picked up if the invoker is static and the invoked is non-static (same class).
// TODO [#185] Method is not picked up if the invoker is static and the invoked method is on a non-static property.
// TODO [#185] Method is not picked up if the invoker is static and the invoked method is on a non-static field.
namespace Mappa.Samples;

#pragma warning disable SA1402

/// <summary>
/// Mapper to showcase the ability of Mappa to pick up
/// polymorphic methods when source and target are defined via
/// <see cref="MappaTypeMappingAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper
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

    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// using <see cref="Map(Models.Polymorphism.One.SourceBaseClass)"/> because it
    /// support the nested property type mapping using the mapping
    /// defined via <see cref="MappaTypeMappingAttribute"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}

/// <summary>
/// Mapper to showcase the ability of Mappa to pick up
/// polymorphic methods when source and target are defined via
/// <see cref="MappaTypeMappingDefaultAttribute"/>.
/// </summary>
// TODO [#49] This should only work when a specific [MappaSetting(PolymorphicMapMethodWithMatchingDefaultAttribute)] is enabled to support mapping using MappaTypeMappingDefault.
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting
    /// polymorphism.
    /// This have custom <see cref="MappaTypeMappingDefaultAttribute"/> that will map from
    /// <see cref="Models.Polymorphism.One.SourceBaseClass"/> to <see cref="Models.Polymorphism.One.TargetUnmappedBaseClass"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(Models.Polymorphism.One.TargetUnmappedBaseClass))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);

    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependencyWithSourceBaseClass"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependencyWithUnmappedBaseClass"/>
    /// using <see cref="Map(Models.Polymorphism.One.SourceBaseClass)"/> because it
    /// support the nested property type mapping using the mapping
    /// defined via <see cref="MappaTypeMappingDefaultAttribute"/>
    /// from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetUnmappedBaseClass"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependencyWithUnmappedBaseClass Map(Models.Polymorphism.One.SourceWithDependencyWithSourceBaseClass source);
}