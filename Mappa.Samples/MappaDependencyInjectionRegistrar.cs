// <copyright file="MappaDependencyInjectionRegistrar.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample registrar demonstrating <see cref="MappaDependencyInjectionAttribute"/> with
/// <see cref="MappaDependencyInjectionAttribute.IgnoreType"/>.
/// The generator emits <c>RegisterMappaSamples</c>.
/// </summary>
/// <remarks>
/// Call <c>services.RegisterMappaSamples()</c> after referencing
/// <c>Microsoft.Extensions.DependencyInjection</c>.
/// Static sample mappers (e.g. extension-method and queryable-projection mappers)
/// cannot be registered with DI and intentionally trigger MP00073.
/// Cross-assembly discovery via <c>InjectFromAssemblies</c> is covered by generator tests;
/// this sample focuses on same-assembly registration and <c>IgnoreType</c>.
/// </remarks>
#pragma warning disable MP00073 // Static [Mappa] mappers in this assembly are skipped for DI by design
[MappaDependencyInjection(
    "RegisterMappaSamples",
    IgnoreType = new[] { typeof(IdentityStrategyMapper), typeof(GuidStrategyMapper) })]
public static partial class MappaDependencyInjectionRegistrar
{
}
#pragma warning restore MP00073