// <copyright file="MappaReferenceManager.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Mappa;

/// <summary>
/// Tracks mapped source/target reference pairs and nesting depth for a mapping operation.
/// </summary>
public sealed class MappaReferenceManager
{
    private readonly Dictionary<object, object> references = new(ReferenceEqualityComparer.Instance);
    private short currentDepth;

    /// <summary>
    /// Gets or sets the maximum nesting depth allowed while mapping.
    /// When <c>0</c>, no maximum depth is applied.
    /// </summary>
    public short MaxDepth { get; set; }

    /// <summary>
    /// Attempts to obtain a previously mapped target for <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="TTarget">The expected target type.</typeparam>
    /// <param name="source">The source instance.</param>
    /// <param name="target">The previously mapped target when found.</param>
    /// <returns><c>true</c> when a matching reference pair exists; otherwise <c>false</c>.</returns>
    public bool TryGetReference<TTarget>(object source, out TTarget target)
    {
        target = default!;
        if (source is null)
        {
            return false;
        }

        if (this.references.TryGetValue(source, out var stored)
            && stored is TTarget typedTarget)
        {
            target = typedTarget;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Stores a source/target reference pair so later mappings can reuse the target.
    /// </summary>
    /// <param name="target">The mapped target instance.</param>
    /// <param name="source">The source instance.</param>
    public void AddReferencePair(object target, object source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        this.references[source] = target;
    }

    /// <summary>
    /// Increases the current nesting depth by one for the lifetime of the returned disposable.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that decreases the depth when disposed.</returns>
    /// <exception cref="MappaException">
    /// Thrown when <see cref="MaxDepth"/> is greater than zero and the increased depth exceeds <see cref="MaxDepth"/>.
    /// </exception>
    public IDisposable IncreaseDepth()
    {
        var nextDepth = (short)(this.currentDepth + 1);
        if (this.MaxDepth > 0 && nextDepth > this.MaxDepth)
        {
            throw new MappaException(
                $"The maximum runtime mapping depth of {this.MaxDepth} has been reached.");
        }

        this.currentDepth = nextDepth;
        return new DepthScope(this);
    }

    private void DecreaseDepth()
    {
        this.currentDepth--;
    }

    private sealed class DepthScope
        : IDisposable
    {
        private readonly MappaReferenceManager manager;
        private bool disposed;

        public DepthScope(MappaReferenceManager manager)
        {
            this.manager = manager;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.manager.DecreaseDepth();
            this.disposed = true;
        }
    }

    private sealed class ReferenceEqualityComparer
        : IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Instance { get; } = new();

        bool IEqualityComparer<object>.Equals(object x, object y) => ReferenceEquals(x, y);

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}