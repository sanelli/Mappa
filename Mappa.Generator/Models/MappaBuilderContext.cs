// <copyright file="MappaBuilderContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// The content of the building.
/// </summary>
internal sealed class MappaBuilderContext
{
    private readonly StackSetting<string> compositeTypeSourceName = new(string.Empty);
    private readonly StackSetting<string> compositeTypeTargetName = new(string.Empty);
    private readonly StackSetting<MapMethod> mapMethodBeingBuilt = new(null!);
    private readonly StackSetting<IPropertySymbol?> currentTargetPropertyForUnsafeAccess = new(null);
    private readonly StackSetting<bool> requiresUnsafeAccessorOnCurrentTargetProperty = new(false);
    private readonly StackSetting<bool> maxRuntimeDepthActive = new(false);
    private readonly StackSetting<short> effectiveMaxRuntimeDepth = new(0);
    private readonly List<Diagnostic> diagnostics = new();
    private uint temporaryCounter;
    private bool referenceManagerAccessorRequired;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaBuilderContext"/> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    public MappaBuilderContext(Compilation compilation)
    {
        this.Compilation = compilation;
    }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    internal Compilation Compilation { get; }

    /// <summary>
    /// Gets any diagnostics reported while generating the code.
    /// </summary>
    internal IReadOnlyList<Diagnostic> Diagnostics => this.diagnostics;

    /// <summary>
    /// Gets the inaccessible-member accessor registry for the current generated file.
    /// </summary>
    internal InaccessibleAccessorRegistry InaccessibleAccessors { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the file-local reference-manager accessor must be emitted.
    /// </summary>
    internal bool ReferenceManagerAccessorRequired => this.referenceManagerAccessorRequired;

    /// <summary>
    /// Gets a value indicating whether MaxRuntimeDepth wrapping is active for the current map method.
    /// </summary>
    internal bool IsMaxRuntimeDepthActive => this.maxRuntimeDepthActive.CurrentValue;

    /// <summary>
    /// Gets the effective MaxRuntimeDepth for the current map method when active; otherwise <c>0</c>.
    /// </summary>
    internal short EffectiveMaxRuntimeDepth => this.effectiveMaxRuntimeDepth.CurrentValue;

    /// <summary>
    /// Gets the target property currently being accessed via an optional unsafe accessor, if any.
    /// </summary>
    internal IPropertySymbol? CurrentTargetPropertyForUnsafeAccess
        => this.currentTargetPropertyForUnsafeAccess.CurrentValue;

    /// <summary>
    /// Gets a value indicating whether the current target property access requires an unsafe accessor.
    /// </summary>
    internal bool RequiresUnsafeAccessorOnCurrentTargetProperty
        => this.requiresUnsafeAccessorOnCurrentTargetProperty.CurrentValue;

    /// <summary>
    /// Gets a new unique temporary value.
    /// </summary>
    /// <returns>A new temporary value.</returns>
    internal string NextTemporary()
        => $"__mappa_tmp_{++this.temporaryCounter}";

    /// <summary>
    /// Push a new value for the source name for struct, record, classes, etc...
    /// </summary>
    /// <param name="sourceName">The name of the source.</param>
    /// <returns>Disposable value used to remove the source.</returns>
    internal IDisposable PushCurrentCompositeTypeSourceName(string sourceName)
        => this.compositeTypeSourceName.Apply(sourceName);

    /// <summary>
    /// Gets the current value of the source pushed.
    /// </summary>
    /// <returns>The current source.</returns>
    internal string GetCompositeTypeSourceName() => this.compositeTypeSourceName.CurrentValue;

    /// <summary>
    /// Push a new value for the target name for struct, record, classes, etc...
    /// </summary>
    /// <param name="sourceName">The name of the target.</param>
    /// <returns>Disposable value used to remove the target.</returns>
    internal IDisposable PushCurrentCompositeTypeTargetName(string sourceName)
        => this.compositeTypeTargetName.Apply(sourceName);

    /// <summary>
    /// Select which method is being built and configure MaxRuntimeDepth state for that method.
    /// </summary>
    /// <param name="mapMethod">The map method.</param>
    /// <returns>Disposable value used to remove the method from the stack.</returns>
    internal IDisposable PushMapMethod(MapMethod mapMethod)
    {
        var mapMethodScope = this.mapMethodBeingBuilt.Apply(mapMethod);
        var activateMaxRuntimeDepth = false;
        var effectiveDepth = (short)0;

        if (mapMethod.MaxRuntimeDepth > 0 && mapMethod.ProvideMappaContextWhenInvoked())
        {
            if (this.Compilation.IsUnsafeAccessorSupported())
            {
                activateMaxRuntimeDepth = true;
                effectiveDepth = mapMethod.MaxRuntimeDepth;
                this.referenceManagerAccessorRequired = true;
            }
            else
            {
                this.ReportDiagnostic(MappaDiagnostics.UnsafeAccessorNotSupported(
                    mapMethod.MethodDeclarationSyntax,
                    mapMethod.MethodName));
            }
        }

#pragma warning disable CA2000 // Disposable scopes are owned by CombinedDisposable.
        var maxRuntimeDepthActiveScope = this.maxRuntimeDepthActive.Apply(activateMaxRuntimeDepth);
        var effectiveMaxRuntimeDepthScope = this.effectiveMaxRuntimeDepth.Apply(effectiveDepth);
#pragma warning restore CA2000
        return new CombinedDisposable(
            mapMethodScope,
            new CombinedDisposable(maxRuntimeDepthActiveScope, effectiveMaxRuntimeDepthScope));
    }

    /// <summary>
    /// Gets the current value of the target pushed.
    /// </summary>
    /// <returns>The current source.</returns>
    internal string GetCompositeTypeTargetName() => this.compositeTypeTargetName.CurrentValue;

    /// <summary>
    /// Gets the current map method.
    /// </summary>
    /// <returns>The current map method.</returns>
    internal MapMethod GetMapMethod() => this.mapMethodBeingBuilt.CurrentValue
        ?? throw new MappaGeneratorException("Cannot obtain the map method.");

    /// <summary>
    /// Pushes the target property that nested builders may need to access via an unsafe accessor.
    /// </summary>
    /// <param name="property">The target property.</param>
    /// <param name="requiresUnsafeAccessor"><c>true</c> when an unsafe accessor is required.</param>
    /// <returns>Disposable value used to restore the previous values.</returns>
    internal IDisposable PushCurrentTargetPropertyUnsafeAccess(IPropertySymbol property, bool requiresUnsafeAccessor)
    {
        var propertyScope = this.currentTargetPropertyForUnsafeAccess.Apply(property);
        var flagScope = this.requiresUnsafeAccessorOnCurrentTargetProperty.Apply(requiresUnsafeAccessor);
        return new CombinedDisposable(propertyScope, flagScope);
    }

    /// <summary>
    /// Report a new diagnostic information.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to be reported.</param>
    internal void ReportDiagnostic(Diagnostic diagnostic) => this.diagnostics.Add(diagnostic);

    private sealed class CombinedDisposable(IDisposable first, IDisposable second)
        : IDisposable
    {
        public void Dispose()
        {
            second.Dispose();
            first.Dispose();
        }
    }
}