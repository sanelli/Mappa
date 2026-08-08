// <copyright file="MappaProtobufDependencyInjection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Dependency.Protobuf.DependencyInjection;

/// <summary>
/// Dependency injection helpers for <see cref="MappaProtobufMapper"/>.
/// The generator emits <c>RegisterMappaProtobuf</c> via
/// <see cref="MappaDependencyInjectionAttribute"/> and
/// <see cref="MappaDependencyInjectionAttribute.InjectFromAssemblies"/>.
/// </summary>
/// <remarks>
/// Call <c>services.RegisterMappaProtobuf()</c> after referencing
/// <c>Microsoft.Extensions.DependencyInjection</c>.
/// </remarks>
[MappaDependencyInjection(
    "RegisterMappaProtobuf",
    InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceAndClass,
    InjectFromAssemblies = new[] { typeof(MappaProtobufMapper) })]
public static partial class MappaProtobufDependencyInjection
{
}