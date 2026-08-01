// <copyright file="MappaDependencyInjectionRegistrar.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample registrar demonstrating <see cref="MappaDependencyInjectionAttribute"/>.
/// The generator emits <c>RegisterMappaSamples</c>, which registers every
/// <see cref="MappaAttribute"/> mapper type in this assembly as a singleton.
/// </summary>
/// <remarks>
/// Call <c>services.RegisterMappaSamples()</c> after referencing
/// <c>Microsoft.Extensions.DependencyInjection</c>.
/// Static sample mappers (e.g. extension-method and queryable-projection mappers)
/// cannot be registered with DI and intentionally trigger MP00073.
/// </remarks>
#pragma warning disable MP00073 // Static [Mappa] mappers in this assembly are skipped for DI by design
[MappaDependencyInjection("RegisterMappaSamples")]
public static partial class MappaDependencyInjectionRegistrar
{
}
#pragma warning restore MP00073