// <copyright file="PolymorphicMethodMapMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

#pragma warning disable SA1402

#pragma warning disable S2094 // Remove this empty class, write its code or make it an "interface".

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
[Mappa]
[MappaSettings(
    CultureInfoSetting = CultureInfoSetting.InvariantCulture,
    PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
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

/// <summary>
/// Mapper thet contain a method that can be used for dependencies.
/// </summary>
[Mappa]
[MappaSettings(
    CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapDependency
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
/// Mapper to showcase the ability of Mappa to pick up
/// polymorphic methods when source and target are defined via
/// <see cref="MappaTypeMappingAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper
{
    [MappaDependency]
    private readonly PolymorphicMethodMapDependency dependency = new();

    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// using <see cref="PolymorphicMethodMapDependency.Map(Models.Polymorphism.One.SourceBaseClass)"/> because it
    /// support the nested property type mapping using the mapping
    /// defined via <see cref="MappaTypeMappingAttribute"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>
    /// on a dependency field.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}

/// <summary>
/// Mapper to showcase the ability of Mappa to avoid picking up a non-static polymorphic
/// method in a static context.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class NonStaticPolymorphicMethodNotInvokedByStaticContextMapper
{
    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// WITHOUT using <see cref="Map(Models.Polymorphism.One.SourceBaseClass)"/> because it
    /// it is not static while this method is static.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public static partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);

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
/// Base class providing a polymorphic map method for inherited mapper samples.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public partial class PolymorphicMethodMapMapperBase
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting polymorphism.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Mapper that inherits a polymorphic map method from a base class.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperWithMapperBaseClass : PolymorphicMethodMapMapperBase
{
    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// using the inherited polymorphic <see cref="PolymorphicMethodMapMapperBase.Map"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}

/// <summary>
/// Mapper that uses a polymorphic map method from a base class of a dependency property type.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperWithDependencyPropertyBaseClass
{
    [MappaDependency]
    private PolymorphicMethodMapDerivedDependency DependencyProperty { get; } = new();

    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// using the polymorphic method on the dependency property base class.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}

/// <summary>
/// Mapper that uses a polymorphic map method from a base class of a dependency field type.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapperWithDependencyFieldBaseClass
{
    [MappaDependency]
    private readonly PolymorphicMethodMapDerivedDependency dependencyField = new();

    /// <summary>
    /// Maps from <see cref="Models.Polymorphism.One.SourceWithDependency"/> to
    /// <see cref="Models.Polymorphism.One.TargetWithDependency"/>
    /// using the polymorphic method on the dependency field base class.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}

/// <summary>
/// Base class providing a polymorphic map method for inherited dependency samples.
/// </summary>
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
internal partial class PolymorphicMethodMapDependencyBase
{
    /// <summary>
    /// Map from <see cref="Models.Polymorphism.One.SourceBaseClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetBaseClass"/> by supporting polymorphism.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetFirstClass), typeof(Models.Polymorphism.One.SourceFirstClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetThirdClass), typeof(Models.Polymorphism.One.SourceThirdClass))]
    [MappaTypeMapping(typeof(Models.Polymorphism.One.TargetSecondClass), typeof(Models.Polymorphism.One.SourceSecondClass))]
    public partial Models.Polymorphism.One.TargetBaseClass Map(Models.Polymorphism.One.SourceBaseClass source);
}

/// <summary>
/// Derived dependency type used by inherited polymorphic dependency samples.
/// </summary>
internal sealed class PolymorphicMethodMapDerivedDependency : PolymorphicMethodMapDependencyBase
{
}