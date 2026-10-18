// <copyright file="PreferredInterfaceResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Selects a closed collection/dictionary interface among multiple implementations.
/// </summary>
internal static class PreferredInterfaceResolver
{
    /// <summary>
    /// Tries to resolve a preferred closed interface for <paramref name="type"/> and <paramref name="family"/>.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="family">The open-generic interface family.</param>
    /// <param name="side">The mapping side.</param>
    /// <param name="context">The algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="isUsable">Returns whether a candidate interface can be used for the current mapping.</param>
    /// <param name="preferred">The selected closed interface when resolved.</param>
    /// <param name="outcome">The resolution outcome.</param>
    /// <returns><c>true</c> when <paramref name="preferred"/> was selected.</returns>
    internal static bool TryResolve(
        INamedTypeSymbol type,
        PreferredInterfaceFamily family,
        PreferredInterfaceSide side,
        MappaMapAlgorithmContext context,
        Compilation compilation,
        Func<INamedTypeSymbol, bool> isUsable,
        out INamedTypeSymbol? preferred,
        out PreferredInterfaceResolutionOutcome outcome)
    {
        preferred = null;
        var session = context.GetOrCreatePreferredInterfaceSession(compilation);

        if (!session.TryEnterResolution(type, family, side))
        {
            outcome = PreferredInterfaceResolutionOutcome.RecursionGuard;
            return false;
        }

        try
        {
            return TryResolveCore(
                type,
                family,
                side,
                context,
                compilation,
                session,
                isUsable,
                out preferred,
                out outcome);
        }
        finally
        {
            session.ExitResolution(type, family, side);
        }
    }

    /// <summary>
    /// Gets the interface family for a closed generic interface, when recognized.
    /// </summary>
    /// <param name="closedInterface">The closed interface.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The family or <c>null</c> when unrecognized.</returns>
    internal static PreferredInterfaceFamily? TryGetFamily(INamedTypeSymbol closedInterface, Compilation compilation)
    {
        var definition = closedInterface.OriginalDefinition;

        if (definition.IsIDictionary(compilation))
        {
            return PreferredInterfaceFamily.IDictionary;
        }

        if (definition.IsIReadOnlyDictionary(compilation))
        {
            return PreferredInterfaceFamily.IReadOnlyDictionary;
        }

        if (definition.IsIImmutableDictionary(compilation))
        {
            return PreferredInterfaceFamily.IImmutableDictionary;
        }

        if (definition.IsICollection())
        {
            return PreferredInterfaceFamily.ICollection;
        }

        if (definition.IsIList())
        {
            return PreferredInterfaceFamily.IList;
        }

        if (definition.IsIReadOnlyList())
        {
            return PreferredInterfaceFamily.IReadOnlyList;
        }

        if (definition.IsIReadOnlyCollection())
        {
            return PreferredInterfaceFamily.IReadOnlyCollection;
        }

        if (definition.IsISet(compilation))
        {
            return PreferredInterfaceFamily.ISet;
        }

        if (definition.IsIReadOnlySet(compilation))
        {
            return PreferredInterfaceFamily.IReadOnlySet;
        }

        if (definition.IsIImmutableSet(compilation))
        {
            return PreferredInterfaceFamily.IImmutableSet;
        }

        if (definition.IsIImmutableList(compilation))
        {
            return PreferredInterfaceFamily.IImmutableList;
        }

        if (definition.IsIImmutableQueue(compilation))
        {
            return PreferredInterfaceFamily.IImmutableQueue;
        }

        if (definition.IsIImmutableStack(compilation))
        {
            return PreferredInterfaceFamily.IImmutableStack;
        }

        if (definition.IsIProducerConsumerCollection(compilation))
        {
            return PreferredInterfaceFamily.IProducerConsumerCollection;
        }

        if (definition.IsIQueryable(compilation))
        {
            return PreferredInterfaceFamily.IQueryable;
        }

        if (definition.IsIEnumerable())
        {
            if (closedInterface.TypeArguments.Length == 1
                && closedInterface.TypeArguments[0].IsKeyValuePair(compilation))
            {
                return PreferredInterfaceFamily.IEnumerableOfKeyValuePair;
            }

            return PreferredInterfaceFamily.IEnumerable;
        }

        return null;
    }

    /// <summary>
    /// Returns whether <paramref name="type"/> is a valid concrete implementing type argument.
    /// </summary>
    /// <param name="type">The type symbol.</param>
    /// <returns><c>true</c> when concrete.</returns>
    internal static bool IsConcreteImplementingType(INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Interface)
        {
            return true;
        }

        if (type.IsUnboundGenericType)
        {
            return false;
        }

        if (!type.IsGenericType)
        {
            return false;
        }

        return !type.TypeArguments.Any(typeArgument => typeArgument is ITypeParameterSymbol);
    }

    /// <summary>
    /// Returns whether <paramref name="type"/> is a closed generic interface type.
    /// </summary>
    /// <param name="type">The type symbol.</param>
    /// <returns><c>true</c> when a closed generic interface.</returns>
    internal static bool IsClosedGenericInterface(INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Interface || !type.IsGenericType || type.IsUnboundGenericType)
        {
            return false;
        }

        return !type.TypeArguments.Any(typeArgument => typeArgument is ITypeParameterSymbol);
    }

    /// <summary>
    /// Enumerates closed interface candidates on <paramref name="type"/> for <paramref name="family"/>.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="family">The interface family.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>Distinct closed interface symbols.</returns>
    internal static IEnumerable<INamedTypeSymbol> EnumerateClosedInterfaceCandidates(
        INamedTypeSymbol type,
        PreferredInterfaceFamily family,
        Compilation compilation)
    {
        var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        if (type.TypeKind == TypeKind.Interface
            && IsClosedGenericInterface(type)
            && TryGetFamily(type, compilation) == family
            && seen.Add(type))
        {
            yield return type;
        }

        foreach (var @interface in type.AllInterfaces)
        {
            if (@interface is not INamedTypeSymbol namedInterface
                || !IsClosedGenericInterface(namedInterface)
                || TryGetFamily(namedInterface, compilation) != family
                || !seen.Add(namedInterface))
            {
                continue;
            }

            yield return namedInterface;
        }
    }

    /// <summary>
    /// Returns whether <paramref name="closedInterface"/> on <paramref name="type"/> is implemented implicitly.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="closedInterface">The closed interface.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> for implicit implementation.</returns>
    internal static bool IsImplicitImplementation(
        INamedTypeSymbol type,
        INamedTypeSymbol closedInterface,
        Compilation compilation)
        => !IsExplicitlyImplemented(type, closedInterface, compilation);

    /// <summary>
    /// Returns whether <paramref name="closedInterface"/> on <paramref name="type"/> is implemented explicitly.
    /// </summary>
    /// <param name="type">The implementing type.</param>
    /// <param name="closedInterface">The closed interface.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> for explicit interface implementation.</returns>
    internal static bool IsExplicitlyImplemented(
        INamedTypeSymbol type,
        INamedTypeSymbol closedInterface,
        Compilation compilation)
        => IsExplicitImplementation(type, closedInterface, compilation);

    private static bool TryResolveCore(
        INamedTypeSymbol type,
        PreferredInterfaceFamily family,
        PreferredInterfaceSide side,
        MappaMapAlgorithmContext context,
        Compilation compilation,
        PreferredInterfaceSession session,
        Func<INamedTypeSymbol, bool> isUsable,
        out INamedTypeSymbol? preferred,
        out PreferredInterfaceResolutionOutcome outcome)
    {
        preferred = null;

        if (session.TryGetMatchingAttribute(type, family, side, compilation, out var matchingAttribute)
            && matchingAttribute?.PreferredClosedInterface is { } attributePreferred)
        {
            session.MarkAttributeNeeded(matchingAttribute);
            if (isUsable(attributePreferred))
            {
                preferred = attributePreferred;
                outcome = PreferredInterfaceResolutionOutcome.Resolved;
                return true;
            }

            context.ReportDiagnostic(
                MappaDiagnostics.PreferredInterfaceCannotBeUsed(
                    matchingAttribute.Location,
                    attributePreferred,
                    type));
            outcome = PreferredInterfaceResolutionOutcome.AttributeCannotBeUsed;
            return false;
        }

        var candidates = EnumerateClosedInterfaceCandidates(type, family, compilation).ToArray();
        if (candidates.Length == 0)
        {
            outcome = PreferredInterfaceResolutionOutcome.NotResolved;
            return false;
        }

        var usableImplicit = candidates
            .Where(candidate => IsImplicitImplementation(type, candidate, compilation) && isUsable(candidate))
            .ToArray();
        if (usableImplicit.Length == 1)
        {
            preferred = usableImplicit[0];
            outcome = PreferredInterfaceResolutionOutcome.Resolved;
            return true;
        }

        var usableExplicit = candidates
            .Where(candidate => IsExplicitImplementation(type, candidate, compilation) && isUsable(candidate))
            .ToArray();
        if (usableExplicit.Length == 1)
        {
            preferred = usableExplicit[0];
            outcome = PreferredInterfaceResolutionOutcome.Resolved;
            return true;
        }

        outcome = PreferredInterfaceResolutionOutcome.NotResolved;
        return false;
    }

    private static bool IsExplicitImplementation(
        INamedTypeSymbol type,
        INamedTypeSymbol closedInterface,
        Compilation compilation)
    {
        var member = GetRepresentativeInterfaceMember(closedInterface, compilation);
        if (member is null)
        {
            return false;
        }

        var implementation = type.FindImplementationForInterfaceMember(member);
        if (implementation is IMethodSymbol { MethodKind: MethodKind.Ordinary })
        {
            return false;
        }

        if (implementation is IMethodSymbol { MethodKind: MethodKind.ExplicitInterfaceImplementation } explicitMethod)
        {
            // Types such as Queue<T>/Stack<T> expose a public GetEnumerator() and also an explicit
            // IEnumerable<T>.GetEnumerator(). Foreach can use the public method without a cast when
            // it yields the preferred element type (unlike DualEnumerable's string interface).
            if (member is IMethodSymbol { Name: "GetEnumerator" }
                && closedInterface.TypeArguments.Length == 1
                && HasPublicEnumeratorForElementType(type, closedInterface.TypeArguments[0]))
            {
                return false;
            }

            return explicitMethod.ExplicitInterfaceImplementations.Any(implementedMember =>
                SymbolEqualityComparer.Default.Equals(implementedMember.ContainingType, closedInterface));
        }

        return implementation is IPropertySymbol { ExplicitInterfaceImplementations.Length: > 0 };
    }

    private static bool HasPublicEnumeratorForElementType(INamedTypeSymbol type, ITypeSymbol elementType)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            foreach (var method in current.GetMembers("GetEnumerator").OfType<IMethodSymbol>())
            {
                if (method.MethodKind != MethodKind.Ordinary
                    || method.DeclaredAccessibility != Accessibility.Public
                    || method.Parameters.Length != 0)
                {
                    continue;
                }

                if (EnumeratorYieldsElementType(method.ReturnType, elementType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool EnumeratorYieldsElementType(ITypeSymbol enumeratorType, ITypeSymbol elementType)
    {
        if (enumeratorType is INamedTypeSymbol { TypeArguments.Length: 1 } namedEnumerator
            && namedEnumerator.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerator_T
            && SymbolEqualityComparer.Default.Equals(namedEnumerator.TypeArguments[0], elementType))
        {
            return true;
        }

        foreach (var @interface in enumeratorType.AllInterfaces)
        {
            if (@interface is INamedTypeSymbol { TypeArguments.Length: 1 } enumeratorInterface
                && enumeratorInterface.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerator_T
                && SymbolEqualityComparer.Default.Equals(enumeratorInterface.TypeArguments[0], elementType))
            {
                return true;
            }
        }

        return false;
    }

    private static ISymbol? GetRepresentativeInterfaceMember(INamedTypeSymbol closedInterface, Compilation compilation)
    {
        var family = TryGetFamily(closedInterface, compilation);
        return family switch
        {
            PreferredInterfaceFamily.IDictionary
                or PreferredInterfaceFamily.IReadOnlyDictionary
                or PreferredInterfaceFamily.IImmutableDictionary
                => closedInterface.GetMembers()
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(method => method.Name == "Add" && method.Parameters.Length == 2),
            PreferredInterfaceFamily.ICollection
                or PreferredInterfaceFamily.IList
                or PreferredInterfaceFamily.ISet
                or PreferredInterfaceFamily.IProducerConsumerCollection
                => closedInterface.GetMembers()
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(method => method.Name == "Add" && method.Parameters.Length == 1),
            PreferredInterfaceFamily.IQueryable
                => closedInterface.GetMembers()
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault(property => property.Name == "Provider"),
            PreferredInterfaceFamily.IEnumerable
                or PreferredInterfaceFamily.IEnumerableOfKeyValuePair
                or PreferredInterfaceFamily.IReadOnlyCollection
                or PreferredInterfaceFamily.IReadOnlyList
                or PreferredInterfaceFamily.IReadOnlySet
                or PreferredInterfaceFamily.IImmutableList
                or PreferredInterfaceFamily.IImmutableSet
                or PreferredInterfaceFamily.IImmutableQueue
                or PreferredInterfaceFamily.IImmutableStack
                => closedInterface.GetMembers()
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(method => method.Name == "GetEnumerator"),
            _ => closedInterface.GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(method => method.Name == "GetEnumerator"),
        };
    }
}