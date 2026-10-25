// <copyright file="MappaTransformerAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Marks a method as a value transformer for every generated map in the mapper class.
/// <para>
/// Transformers are discovered on the mapper class and its accessible base-class hierarchy.
/// A transformer method must have the same input and output type, including nullability.
/// Eligible signatures are <c>T Method(T)</c>, <c>T Method(in T)</c>, <c>T Method(T, MappaContext)</c>,
/// or <c>T Method(in T, MappaContext)</c>. Context overloads are considered only when the map method
/// provides a <see cref="Mappa.MappaContext"/> parameter. Methods may be <c>static</c> or instance
/// and may use any declared accessibility, but only methods accessible from the mapper are used.
/// <c>ref</c> and <c>out</c> are not allowed; <c>async</c> methods are not allowed; open generic
/// methods are allowed when they can be bound to the site type with an exact nullability match.
/// </para>
/// <para>
/// For each mapping site, matching transformers are applied to the source value before the next
/// mapping step and to the mapped value before assignment or return. Multiple transformers for the
/// same type are all applied in deterministic order:
/// <see cref="MappaApplyTransformerAttribute"/> declarations first, then local
/// <see cref="MappaTransformerAttribute"/> methods, then base-class transformers, then dependency
/// transformers when <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> is enabled.
/// Declaration order is preserved within each tier.
/// </para>
/// <para>
/// Transformers from <see cref="MappaDependencyAttribute"/> and <see cref="MappaStaticDependencyAttribute"/>
/// types are included only when <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> is enabled.
/// Value transformers are not applied inside <c>IQueryable</c> projection generation; class-level
/// transformers alone do not invalidate projection methods.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaTransformerAttribute
    : Attribute, IMappaMethodAttribute;