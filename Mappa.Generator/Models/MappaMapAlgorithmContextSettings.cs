// <copyright file="MappaMapAlgorithmContextSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;

namespace Mappa.Generator.Models;

/// <summary>
/// Settings that can applied to a <see cref="MappaMapAlgorithmContext"/>.
/// </summary>
internal sealed class MappaMapAlgorithmContextSettings
{
    /// <summary>
    /// Gets the stack settings that enable or disable the algorithm
    /// in making sure if a constructor map strategy can be
    /// applied or not.
    /// </summary>
    /// <remarks>
    /// Typically the constructor strategy won't be applied if
    /// we are looking for a strategy to match a constructor
    /// with single parameter.
    /// </remarks>
    internal StackSetting<bool> UseConstructorMapStrategyDetector { get; } = new(true);

    /// <summary>
    /// Gets the stack settings that enable or disable the algorithm
    /// in making sure if a nullable reference map strategy can be
    /// applied or not.
    /// </summary>
    /// <remarks>
    /// Typically the nullable reference strategy won't be applied if
    /// we are looking for a strategy to match a nullable reference strategy.
    /// </remarks>
    internal StackSetting<bool> UseReferenceNullableMapStrategyDetector { get; } = new(true);

    /// <summary>
    /// Apply default values.
    /// </summary>
    /// <returns>The disposable object that once dispose will restore default values.</returns>
    internal IDisposable ApplyDefaults()
        => new MappaMapAlgorithmContextSettingsDefaults(this);

    private sealed class MappaMapAlgorithmContextSettingsDefaults
        : IDisposable
    {
        private readonly List<IDisposable> disposables = new();

        internal MappaMapAlgorithmContextSettingsDefaults(MappaMapAlgorithmContextSettings settings)
        {
            this.disposables.Clear();
            foreach (IStackSetting stackSetting in new[]
                     {
                         settings.UseConstructorMapStrategyDetector,
                         settings.UseReferenceNullableMapStrategyDetector,
                     })
            {
                this.disposables.Add(stackSetting.ApplyDefault());
            }
        }

        public void Dispose()
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }
        }
    }
}