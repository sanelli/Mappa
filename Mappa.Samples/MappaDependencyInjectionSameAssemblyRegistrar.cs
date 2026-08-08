// <copyright file="MappaDependencyInjectionSameAssemblyRegistrar.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample registrar demonstrating same-assembly-only discovery (no
/// <see cref="MappaDependencyInjectionAttribute.InjectFromAssemblies"/>) with
/// <see cref="MappaDependencyInjectionAttribute.IgnoreType"/>.
/// The generator emits <c>RegisterMappaSamplesSameAssembly</c>.
/// </summary>
/// <remarks>
/// Call <c>services.RegisterMappaSamplesSameAssembly()</c> after referencing
/// <c>Microsoft.Extensions.DependencyInjection</c>.
/// Static sample mappers cannot be registered with DI and intentionally trigger MP00073.
/// </remarks>
#pragma warning disable MP00073 // Static [Mappa] mappers in this assembly are skipped for DI by design
[MappaDependencyInjection(
    "RegisterMappaSamplesSameAssembly",
    IgnoreType = new[] { typeof(MappaDependencyInjectionMapper), typeof(IdentityStrategyMapper) })]
public static partial class MappaDependencyInjectionSameAssemblyRegistrar
{
}
#pragma warning restore MP00073