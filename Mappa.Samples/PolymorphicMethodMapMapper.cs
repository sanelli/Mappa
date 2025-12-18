// <copyright file="PolymorphicMethodMapMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper to showcase the ability of Mappa to pick up
/// polymorphic methods.
/// </summary>
// TODO [#49] Add sample to check we can pick up polymorphic method when mapping when types are defined in the MappaTypeMappingDefault attribute explicitly.
// TODO [#49] Add sample to check we can pick up polymorphic method when mapping when types are defined in the MappaTypeMappingDefault attribute implicitly.
[Mappa]
[MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
public sealed partial class PolymorphicMethodMapMapper
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
    /// support the derived type defined by <see cref="MappaTypeMappingAttribute"/>
    /// from <see cref="Models.Polymorphism.One.SourceThirdClass"/>
    /// to <see cref="Models.Polymorphism.One.TargetThirdClass"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial Models.Polymorphism.One.TargetWithDependency Map(Models.Polymorphism.One.SourceWithDependency source);
}