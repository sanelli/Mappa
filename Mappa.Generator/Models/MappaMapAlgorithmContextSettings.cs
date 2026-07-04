// <copyright file="MappaMapAlgorithmContextSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Helpers;

namespace Mappa.Generator.Models;

/// <summary>
/// Settings that can applied to a <see cref="MappaMapAlgorithmContext"/>.
/// </summary>
internal sealed class MappaMapAlgorithmContextSettings
{
    /// <summary>
    /// Define the actions available to use or not use
    /// mappa attributes in the constructor strategies detector.
    /// </summary>
    internal enum MappaAttributesForConstructorDetectorSettings
    {
        /// <summary>
        /// Use the attributes.
        /// </summary>
        Enable,

        /// <summary>
        /// Do not use the attributes.
        /// </summary>
        Disable,
    }

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
    internal StackSetting<bool> UseNullableMapStrategyDetector { get; } = new(true);

    /// <summary>
    /// Gets the stack settings that enable or disable the identity map strategy detector.
    /// </summary>
    internal StackSetting<bool> UseIdentityMapStrategyDetector { get; } = new(true);

    /// <summary>
    /// Gets the stack settings that enable or disable the usage
    /// of mappa attributes when performing a constructor mapping.
    /// </summary>
    /// <remarks>
    /// This is used to prevent mappa attribute such as <see cref="MappaInvokeMethodAttribute"/>
    /// to be used on types used by properties/arguments during
    /// the constructor strategy detector.
    /// Defaults of this are not applied by <see cref="ApplyAlgorithmContextDefaults"/>.
    /// </remarks>
    internal StackSetting<MappaAttributesForConstructorDetectorSettings> UseAttributesForConstructorDetectorSettings { get; } = new(MappaAttributesForConstructorDetectorSettings.Enable);

    /// <summary>
    /// Gets the stack of detectors.
    /// </summary>
    /// <remarks>
    /// This can be used to ensure that some detectors only runs in certain
    /// condition (e.g. the polymorphism detector one can only be run as root or after
    /// the nullable detectors one, or that the identity detector does not run at root
    /// when the polymorphism attribute is present but can run at any other time).
    /// </remarks>
    internal StackSetting<Type> Detectors { get; } = new(null!);

    /// <summary>
    /// Apply default values.
    /// </summary>
    /// <returns>The disposable object that once dispose will restore default values.</returns>
    internal IDisposable ApplyAlgorithmContextDefaults()
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
                         settings.UseNullableMapStrategyDetector,
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