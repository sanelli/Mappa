// <copyright file="InaccessibleAccessorRegistry.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Deduplicates and stores inaccessible-member <c>UnsafeAccessor</c> methods for a generated file.
/// </summary>
internal sealed class InaccessibleAccessorRegistry
{
    private readonly Dictionary<AccessorKey, InaccessibleAccessorMethodDefinition> accessors = new();

    /// <summary>
    /// Gets the file-local container type name that hosts the generated accessors.
    /// </summary>
    internal string ContainerTypeName { get; } = "__MappaInaccessibleAccessors";

    /// <summary>
    /// Gets the registered accessors.
    /// </summary>
    internal IEnumerable<InaccessibleAccessorMethodDefinition> Accessors
        => this.accessors.Values;

    /// <summary>
    /// Gets or adds a property getter accessor and returns the fully-qualified call target
    /// (container type + method name).
    /// </summary>
    /// <param name="containingType">The declaring type.</param>
    /// <param name="property">The property.</param>
    /// <param name="context">The builder context used to allocate temporary names.</param>
    /// <returns>The call target expression prefix (<c>Container.Method</c>).</returns>
    internal string GetOrAddPropertyGetter(
        ITypeSymbol containingType,
        IPropertySymbol property,
        MappaBuilderContext context)
    {
        var runtimeName = $"get_{property.Name}";
        var key = new AccessorKey(containingType, InaccessibleAccessorUnsafeKind.Method, runtimeName);
        if (!this.accessors.TryGetValue(key, out var definition))
        {
            definition = InaccessibleAccessorMethodDefinition.ForPropertyGetter(
                context.NextTemporary(),
                containingType,
                property);
            this.accessors.Add(key, definition);
        }

        return $"{this.ContainerTypeName}.{definition.MethodName}";
    }

    /// <summary>
    /// Gets or adds a property setter accessor and returns the fully-qualified call target
    /// (container type + method name).
    /// </summary>
    /// <param name="containingType">The declaring type.</param>
    /// <param name="property">The property.</param>
    /// <param name="context">The builder context used to allocate temporary names.</param>
    /// <returns>The call target expression prefix (<c>Container.Method</c>).</returns>
    internal string GetOrAddPropertySetter(
        ITypeSymbol containingType,
        IPropertySymbol property,
        MappaBuilderContext context)
    {
        var runtimeName = $"set_{property.Name}";
        var key = new AccessorKey(containingType, InaccessibleAccessorUnsafeKind.Method, runtimeName);
        if (!this.accessors.TryGetValue(key, out var definition))
        {
            definition = InaccessibleAccessorMethodDefinition.ForPropertySetter(
                context.NextTemporary(),
                containingType,
                property);
            this.accessors.Add(key, definition);
        }

        return $"{this.ContainerTypeName}.{definition.MethodName}";
    }

    /// <summary>
    /// Gets or adds a constructor accessor and returns the fully-qualified call target
    /// (container type + method name).
    /// </summary>
    /// <param name="constructor">The constructor.</param>
    /// <param name="context">The builder context used to allocate temporary names.</param>
    /// <returns>The call target expression prefix (<c>Container.Method</c>).</returns>
    internal string GetOrAddConstructor(IMethodSymbol constructor, MappaBuilderContext context)
    {
        var parameterSignature = string.Join(
            ",",
            constructor.Parameters.Select(parameter => parameter.Type.ToDisplayString()));
        var key = new AccessorKey(
            constructor.ContainingType,
            InaccessibleAccessorUnsafeKind.Constructor,
            $".ctor({parameterSignature})");
        if (!this.accessors.TryGetValue(key, out var definition))
        {
            definition = InaccessibleAccessorMethodDefinition.ForConstructor(
                context.NextTemporary(),
                constructor);
            this.accessors.Add(key, definition);
        }

        return $"{this.ContainerTypeName}.{definition.MethodName}";
    }

    /// <summary>
    /// Builds the file-local static class that hosts all registered accessors.
    /// </summary>
    /// <returns>The source code, or an empty string when no accessors were registered.</returns>
    internal string BuildSource()
    {
        if (this.accessors.Count == 0)
        {
            return string.Empty;
        }

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"file static class {this.ContainerTypeName}");
        using (builder.CurlyBracesBlock())
        {
            foreach (var accessor in this.Accessors)
            {
                builder.AppendLine(accessor.BuildSource());
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Dictionary key for an inaccessible accessor entry.
    /// </summary>
    /// <param name="containingType">The declaring type.</param>
    /// <param name="unsafeKind">The unsafe accessor kind.</param>
    /// <param name="runtimeName">The runtime member name used for deduplication.</param>
    internal readonly struct AccessorKey(
        ITypeSymbol containingType,
        InaccessibleAccessorUnsafeKind unsafeKind,
        string runtimeName)
        : IEquatable<AccessorKey>
    {
        private readonly ITypeSymbol containingType = containingType;
        private readonly InaccessibleAccessorUnsafeKind unsafeKind = unsafeKind;
        private readonly string runtimeName = runtimeName;

        /// <inheritdoc/>
        public bool Equals(AccessorKey other)
            => this.unsafeKind == other.unsafeKind
               && this.runtimeName.Equals(other.runtimeName, StringComparison.Ordinal)
               && SymbolEqualityComparer.Default.Equals(this.containingType, other.containingType);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj is AccessorKey other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = SymbolEqualityComparer.Default.GetHashCode(this.containingType);
                hash = (hash * 397) ^ (int)this.unsafeKind;
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(this.runtimeName);
                return hash;
            }
        }
    }
}