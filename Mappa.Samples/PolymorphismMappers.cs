// <copyright file="PolymorphismMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402

using Mappa.Attributes;

// TODO [#49] Source -> Source WITH TypeMapping attribute -> identity is bypassed at root.
// TODO [#49] Source -> Source WITHOUT TypeMapping attribute -> identity works (maybe overkill as this test already exists).
// TODO [#49] Add more mappers in order to cover the interface -> interface scenarios.
// TODO [#49] Add more mappers in order to cover the interface -> class scenarios.
// TODO [#49] Add more mappers in order to cover the class -> interface scenarios.
// TODO [#49] Add more mappers in order to cover the nullable scenarios.
// TODO [#49] Add more mappers to cover different defaults.
namespace Mappa.Samples;

/// <summary>
/// Mapper to showcase the usage of <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
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
public sealed partial class PolymorphismMapperNullable
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
    public partial Models.Polymorphism.One.TargetBaseClass? Map(Models.Polymorphism.One.SourceBaseClass? source);
}