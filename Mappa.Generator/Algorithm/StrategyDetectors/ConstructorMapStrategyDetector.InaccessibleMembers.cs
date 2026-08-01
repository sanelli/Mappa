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
    {
        var options = this.GetInaccessibleSourceOptions();
        var rootMapMethod = this.context.GetRootMapMethod();
        return sourceType.GetTypeProperties()
            .Where(property => InaccessibleMemberEligibility.TryIsSourcePropertyReadable(
                property,
                this.compilation,
                rootMapMethod,
                options,
                out _))
            .ToArray();
    }

    private bool TryIsSourcePropertyReadable(IPropertySymbol property, out bool requiresUnsafeAccessor)
        => InaccessibleMemberEligibility.TryIsSourcePropertyReadable(
            property,
            this.compilation,
            this.context.GetRootMapMethod(),
            this.GetInaccessibleSourceOptions(),
            out requiresUnsafeAccessor);

    private bool TryIsTargetPropertyWritable(IPropertySymbol property, out bool requiresUnsafeAccessor)
        => InaccessibleMemberEligibility.TryIsTargetPropertyWritable(
            property,
            this.compilation,
            this.context.GetRootMapMethod(),
            this.GetInaccessibleTargetOptions(),
            out requiresUnsafeAccessor);

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

        var options = this.GetInaccessibleTargetOptions();
        if (options is null
            || !this.compilation.IsUnsafeAccessorSupported()
            || !options.IsPropertyAllowed(property.Name))
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