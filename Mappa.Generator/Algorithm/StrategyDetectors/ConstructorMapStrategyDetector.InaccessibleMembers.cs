// <copyright file="ConstructorMapStrategyDetector.InaccessibleMembers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Inaccessible-member helpers for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private InaccessibleSourceMemberOptions? GetInaccessibleSourceOptions()
        => InaccessibleSourceMemberOptions.FromAttribute(
            this.context.MapMethod?.GetAttribute<MappaAllowInaccessibleSourceMembersAttribute>());

    private InaccessibleTargetMemberOptions? GetInaccessibleTargetOptions()
        => InaccessibleTargetMemberOptions.FromAttribute(
            this.context.MapMethod?.GetAttribute<MappaAllowInaccessibleTargetMembersAttribute>());

    private IPropertySymbol[] GetReadableSourceProperties(ITypeSymbol sourceType)
        => sourceType.GetTypeProperties()
            .Where(property => this.TryIsSourcePropertyReadable(property, out _, out _))
            .ToArray();

    private bool TryIsSourcePropertyReadable(
        IPropertySymbol property,
        out bool requiresUnsafeAccessor,
        out bool requiresInterfaceCast)
        => InaccessibleMemberEligibility.TryIsSourcePropertyReadable(
            property,
            this.compilation,
            this.context.GetRootMapMethod(),
            this.GetInaccessibleSourceOptions(),
            this.context.MappaUserSettings.MapSourceExplicitInterfaceProperties,
            out requiresUnsafeAccessor,
            out requiresInterfaceCast);

    private bool TryIsTargetPropertyWritable(
        IPropertySymbol property,
        out bool requiresUnsafeAccessor,
        out bool requiresInterfaceCast)
        => InaccessibleMemberEligibility.TryIsTargetPropertyWritable(
            property,
            this.compilation,
            this.context.GetRootMapMethod(),
            this.GetInaccessibleTargetOptions(),
            this.context.MappaUserSettings.MapTargetExplicitInterfaceProperties,
            out requiresUnsafeAccessor,
            out requiresInterfaceCast);

    private bool TryIsTargetPropertyGetterReadable(IPropertySymbol property, out bool requiresUnsafeAccessor)
    {
        requiresUnsafeAccessor = false;
        if (property.GetMethod is null)
        {
            return false;
        }

        if (property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
        {
            return true;
        }

        // Explicit interface getters are not UnsafeAccessor candidates.
        if (InaccessibleMemberEligibility.IsExplicitInterfaceImplementation(property))
        {
            return false;
        }

        var options = this.GetInaccessibleTargetOptions();
        if (options is null
            || !this.compilation.IsUnsafeAccessorSupported()
            || !options.IsPropertyAllowed(property.GetMappablePropertyName()))
        {
            return false;
        }

        requiresUnsafeAccessor = true;
        return true;
    }

    private IMethodSymbol[] GetInvokableConstructors(int? numberOfArguments = null)
    {
        if (this.context.TargetType is not INamedTypeSymbol namedTypeSymbol)
        {
            return [];
        }

        var options = this.GetInaccessibleTargetOptions();
        return namedTypeSymbol.Constructors
            .Where(constructor => numberOfArguments is null || constructor.Parameters.Length == numberOfArguments)
            .Where(constructor => InaccessibleMemberEligibility.TryIsConstructorInvokable(
                constructor,
                this.compilation,
                this.context.ParentSymbol,
                options,
                out _))
            .ToArray();
    }

    private bool RequiresUnsafeAccessorForConstructor(IMethodSymbol constructor)
    {
        InaccessibleMemberEligibility.TryIsConstructorInvokable(
            constructor,
            this.compilation,
            this.context.ParentSymbol,
            this.GetInaccessibleTargetOptions(),
            out var requiresUnsafeAccessor);
        return requiresUnsafeAccessor;
    }
}