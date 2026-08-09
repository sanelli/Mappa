// <copyright file="MappaMapAlgorithmContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describe the minimal properties needed to perform
/// a mapping.
/// </summary>
internal abstract class MappaMapAlgorithmContext
{
    private readonly Stack<(ITypeSymbol TargetType, ITypeSymbol SourceType)> inFlightTypePairs = new();
    private short currentDepth = -1;

    /// <summary>
    /// Gets the parent symbol.
    /// </summary>
    internal abstract ISymbol ParentSymbol { get;  }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal abstract ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal abstract ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal abstract MapMethod? MapMethod { get; }

    /// <summary>
    /// Gets the active nested property path context, if any.
    /// </summary>
    internal virtual PropertyPathContext? PropertyPathContext => null;

    /// <summary>
    /// Gets the context settings.
    /// </summary>
    internal abstract MappaMapAlgorithmContextSettings AlgorithmSettings { get; }

    /// <summary>
    /// Gets the user settings built up to this point.
    /// </summary>
    internal abstract MappaUserSettings MappaUserSettings { get; }

    /// <summary>
    /// Gets a value indicating whether a diagnostic with severity error has been reported.
    /// </summary>
    internal abstract bool HasErrorDiagnostics { get; }

    /// <summary>
    /// Gets the current compile-time strategy-discovery nesting depth.
    /// Starts at <c>-1</c> and increases for each nested <c>GetStrategy</c> call when
    /// <see cref="IMappaUserSettings.MaxCompileTimeDepth"/> is greater than zero.
    /// </summary>
    internal short CurrentDepth => this.GetRootAlgorithmContext().currentDepth;

    /// <summary>
    /// Gets a value indicating whether the nullable flag
    /// is enabled in the current context.
    /// </summary>
    /// <returns><c>true</c> if nullable is enabled, <c>false</c> otherwise.</returns>
    internal abstract bool IsNullableEnabled();

    /// <summary>
    /// Try to obtain a method with the given <paramref name="targetType"/> and <paramref name="sourceType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mapMethod">The map method (if it exists).</param>
    /// <returns><c>true</c> if map method exists, <c>false</c> otherwise.</returns>
    internal abstract bool TryGetMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        out MapMethod mapMethod);

    /// <summary>
    /// Try to obtain a polymorphic method with the given <paramref name="targetType"/> and <paramref name="sourceType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mappaUserSettings">The user settings applied to the method being mapped.</param>
    /// <param name="mapMethod">The map method (if it exists).</param>
    /// <returns><c>true</c> if map method exists, <c>false</c> otherwise.</returns>
    internal abstract bool TryGetPolymorphicMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMappaUserSettings mappaUserSettings,
        out MapMethod mapMethod);

    /// <summary>
    /// Try to obtain a compatible map method with the given <paramref name="targetType"/> and <paramref name="sourceType"/>.
    /// A method is compatible when the required source is implicitly convertible to the method parameter
    /// and the method return type is implicitly convertible to the required target.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="compilation">The compilation used to resolve implicit conversions.</param>
    /// <param name="mapMethod">The map method (if it exists).</param>
    /// <returns><c>true</c> if a compatible map method exists, <c>false</c> otherwise.</returns>
    internal abstract bool TryGetCompatibleMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Compilation compilation,
        out MapMethod? mapMethod);

    /// <summary>
    /// Report a diagnostic.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to report.</param>
    internal abstract void ReportDiagnostic(Diagnostic diagnostic);

    /// <summary>
    /// Get the location being mapped.
    /// </summary>
    /// <returns>The location being mapped.</returns>
    internal abstract Location? GetLocation();

    /// <summary>
    /// Increases the compile-time strategy-discovery depth by one for the lifetime of the returned disposable.
    /// Depth is tracked on the root algorithm context so nested derived contexts share the same counter.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that decreases the depth when disposed.</returns>
    internal IDisposable IncreaseCompileTimeDepth()
    {
        var root = this.GetRootAlgorithmContext();
        root.currentDepth++;
        return new CompileTimeDepthScope(root);
    }

    /// <summary>
    /// Pushes an in-flight <paramref name="targetType"/>/<paramref name="sourceType"/> pair onto the
    /// compile-time mapping-cycle stack when the pair is not already present.
    /// </summary>
    /// <param name="targetType">The target type being mapped.</param>
    /// <param name="sourceType">The source type being mapped.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that pops the pair when disposed, or <c>null</c> when the same
    /// pair is already in flight (cycle).
    /// </returns>
    internal IDisposable? TryPushMappingTypePair(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        var root = this.GetRootAlgorithmContext();
        foreach (var (inFlightTargetType, inFlightSourceType) in root.inFlightTypePairs)
        {
            if (SymbolEqualityComparer.Default.Equals(inFlightTargetType, targetType)
                && SymbolEqualityComparer.Default.Equals(inFlightSourceType, sourceType))
            {
                return null;
            }
        }

        root.inFlightTypePairs.Push((targetType, sourceType));
        return new MappingTypePairScope(root);
    }

    /// <summary>
    /// Gets the map method.
    /// </summary>
    /// <returns>The map method <see cref="MapMethod"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <see cref="MapMethod"/> is <c>null</c>.</exception>
    internal MapMethod GetMapMethod()
    {
        if (this.MapMethod is null)
        {
            throw new MappaGeneratorException("Map method is not defined.");
        }

        return this.MapMethod;
    }

    /// <summary>
    /// Gets the root map method which is actually being mapped.
    /// </summary>
    /// <returns>The map method from the root chain of calls.</returns>
    /// <exception cref="MappaGeneratorException">When the map method cannot be obtained.</exception>
    internal MapMethod GetRootMapMethod()
    {
        return this.GetRootAlgorithmContext().GetMapMethod();
    }

    /// <summary>
    /// Gets the root source type for the map method being generated.
    /// </summary>
    /// <returns>The root source type.</returns>
    internal ITypeSymbol GetRootSourceType()
    {
        return this.GetRootAlgorithmContext().SourceType;
    }

    /// <summary>
    /// Tries to resolve the <see cref="MappaClassGeneratorContext"/> that owns the current mapping
    /// by walking <see cref="DerivedMappaMapAlgorithmContext"/> parents.
    /// </summary>
    /// <param name="classContext">The class generator context when found.</param>
    /// <returns><c>true</c> when a class context is available; otherwise <c>false</c>.</returns>
    internal bool TryGetClassGeneratorContext(out MappaClassGeneratorContext? classContext)
    {
        var root = this.GetRootAlgorithmContext();
        if (root is MappaMethodGeneratorContext methodContext)
        {
            classContext = methodContext.ClassContext;
            return true;
        }

        classContext = null;
        return false;
    }

    private MappaMapAlgorithmContext GetRootAlgorithmContext()
    {
        MappaMapAlgorithmContext context = this;
        while (context is DerivedMappaMapAlgorithmContext algorithmContext)
        {
            context = algorithmContext.ParentContext;
        }

        return context;
    }

    private void DecreaseCompileTimeDepth()
    {
        this.currentDepth--;
    }

    private void PopMappingTypePair()
    {
        this.inFlightTypePairs.Pop();
    }

    private sealed class CompileTimeDepthScope
        : IDisposable
    {
        private readonly MappaMapAlgorithmContext context;
        private bool disposed;

        public CompileTimeDepthScope(MappaMapAlgorithmContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.context.DecreaseCompileTimeDepth();
            this.disposed = true;
        }
    }

    private sealed class MappingTypePairScope
        : IDisposable
    {
        private readonly MappaMapAlgorithmContext context;
        private bool disposed;

        public MappingTypePairScope(MappaMapAlgorithmContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.context.PopMappingTypePair();
            this.disposed = true;
        }
    }
}