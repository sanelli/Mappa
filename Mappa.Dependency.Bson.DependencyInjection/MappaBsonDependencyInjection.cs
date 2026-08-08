// <copyright file="MappaBsonDependencyInjection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Dependency.Bson.DependencyInjection;

/// <summary>
/// Dependency injection helpers for <see cref="MappaBsonMapper"/>.
/// The generator emits <c>RegisterMappaBson</c> via
/// <see cref="MappaDependencyInjectionAttribute"/> and
/// <see cref="MappaDependencyInjectionAttribute.InjectFromAssemblies"/>.
/// </summary>
/// <remarks>
/// Call <c>services.RegisterMappaBson()</c> after referencing
/// <c>Microsoft.Extensions.DependencyInjection</c>.
/// </remarks>
[MappaDependencyInjection(
    "RegisterMappaBson",
    InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceAndClass,
    InjectFromAssemblies = new[] { typeof(MappaBsonMapper) })]
public static partial class MappaBsonDependencyInjection
{
}