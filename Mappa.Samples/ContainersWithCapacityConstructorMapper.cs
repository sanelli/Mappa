// <copyright file="ContainersWithCapacityConstructorMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper to test custom containers with capacity constructors.
/// </summary>
[Mappa]
[MappaSettings(ContainerCapacityConstructors = BooleanSetting.Enable)]
public sealed partial class ContainersWithCapacityConstructorMapper
{
    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomICollectionWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The custom collection.</returns>
    public partial CustomICollectionWithCapacityConstructor<string> MapFromArrayToCustomCollection(int[] input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomICollectionWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input enumerable.</param>
    /// <returns>The custom collection.</returns>
    public partial CustomICollectionWithCapacityConstructor<string> MapFromEnumerableToCustomCollection(IEnumerable<int> input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomISetWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The custom set.</returns>
    public partial CustomISetWithCapacityConstructor<string> MapFromArrayToCustomSet(int[] input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomISetWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input enumerable.</param>
    /// <returns>The custom set.</returns>
    public partial CustomISetWithCapacityConstructor<string> MapFromEnumerableToCustomSet(IEnumerable<int> input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomStackWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomStackWithCapacityConstructor<string> MapFromArrayToCustomStack(int[] input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomStackWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input enumerable.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomStackWithCapacityConstructor<string> MapFromEnumerableToCustomStack(IEnumerable<int> input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomQueueWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomQueueWithCapacityConstructor<string> MapFromArrayToCustomQueue(int[] input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomQueueWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input enumerable.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomQueueWithCapacityConstructor<string> MapFromEnumerableToCustomQueue(IEnumerable<int> input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomBlockingCollectionWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomBlockingCollectionWithCapacityConstructor<string> MapFromArrayToCustomBlockingCollection(int[] input);

    /// <summary>
    /// Map from array of integer <see cref="int"/> to
    /// <see cref="CustomBlockingCollectionWithCapacityConstructor{T}"/>
    /// of <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input enumerable.</param>
    /// <returns>The custom stack.</returns>
    public partial CustomBlockingCollectionWithCapacityConstructor<string> MapFromEnumerableToCustomBlockingCollection(IEnumerable<int> input);
}