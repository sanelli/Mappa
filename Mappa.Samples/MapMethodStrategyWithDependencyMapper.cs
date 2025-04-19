// <copyright file="MapMethodStrategyWithDependencyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Samples.Models;

#pragma warning disable SA1402 // Multiple classes in the same file
#pragma warning disable SA1403 // File contains multiple namespaces
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable S1118 // Add a 'private' constructor or the 'static' keyword to the class declaration.
#pragma warning disable S2325 // Make 'Map' a static method.
#pragma warning disable CA1822 // Member 'Map' does not access instance data and can be marked as static
#pragma warning disable CA1812
#pragma warning disable CA1852
namespace Mappa.Samples
{
    namespace DependencyMapperDependencies
    {
        /// <summary>
        /// Static class mapper with a static method.
        /// </summary>
        internal static class StaticClassWithStaticMethodForStaticDependency
        {
            /// <summary>
            /// Map unsigned long to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal static string Map(float x) => (x + 6).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Static class mapper with a static method.
        /// </summary>
        internal sealed class NonStaticClassWithStaticMethodForStaticDependency
        {
            /// <summary>
            /// Map unsigned long to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal static string Map(double x) => (x + 7).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Mapper using the map method strategy.
        /// </summary>
        [Mappa]
        internal partial class PropertyDependencyMapper
        {
            /// <summary>
            /// Map lo g to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal static string Map(byte x) => (x + 4).ToString(CultureInfo.InvariantCulture);

            /// <summary>
            /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
            /// </summary>
            /// <param name="sourceClassModel">The source model.</param>
            /// <returns>The target model.</returns>
            // ReSharper disable once UnusedMember.Local
            internal partial TargetClassModel Map(SourceClassModel sourceClassModel);
        }

        /// <summary>
        /// Mapper for integer to string.
        /// </summary>
        internal sealed class FieldDependencyMapper
        {
            /// <summary>
            /// Map unsigned long to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal static string Map(sbyte x) => (x + 5).ToString(CultureInfo.InvariantCulture);

            /// <summary>
            /// Map integer to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal string Map(int x) => (x + 1).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Mapper for long to string.
        /// </summary>
        internal sealed class StaticPropertyDependencyMapper
        {
            /// <summary>
            /// Map lo g to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal string Map(long x) => (x + 2).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Mapper for unsigned to string.
        /// </summary>
        internal sealed class StaticFieldDependencyMapper
        {
            /// <summary>
            /// Map unsigned long to string.
            /// </summary>
            /// <param name="x">The original value.</param>
            /// <returns>The mapped value.</returns>
            internal string Map(ulong x) => (x + 3).ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Mapper using the map method strategy.
    /// </summary>
    [Mappa]
    [MappaStaticDependency(typeof(DependencyMapperDependencies.StaticClassWithStaticMethodForStaticDependency))]
    [MappaStaticDependency(typeof(DependencyMapperDependencies.NonStaticClassWithStaticMethodForStaticDependency))]
    public sealed partial class MapMethodStrategyWithDependencyMapper
    {
        [MappaDependency]
        private static readonly DependencyMapperDependencies.StaticFieldDependencyMapper StaticFieldDependency = new();

        [MappaDependency]
        private readonly DependencyMapperDependencies.FieldDependencyMapper fieldDependency = new();

        [MappaDependency]
        private static DependencyMapperDependencies.StaticPropertyDependencyMapper StaticPropertyDependency { get; } = new();

        [MappaDependency]
        private DependencyMapperDependencies.PropertyDependencyMapper PropertyDependency { get; } = new();

        /// <summary>
        /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
        /// </summary>
        /// <param name="input">The source model.</param>
        /// <returns>The target model.</returns>
        public partial TargetClassWithMultipleFieldForDependencyModel Map(SourceClassWithMultipleFieldsForDependencyModel input);
    }
}