// <copyright file="GuidStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper to showcase <see cref="Guid"/> strategy.
/// </summary>
[Mappa]
public sealed partial class GuidStrategyMapper
{
   /// <summary>
   /// Map <see cref="Guid"/> to <see cref="Array"/> of <see cref="byte"/>s.
   /// </summary>
   /// <param name="input">The guid.</param>
   /// <returns>The target of the mapping.</returns>
    public partial byte[] MapFromGuidToArray(Guid input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="Span{T}"/> of <see cref="byte"/>s.
    /// </summary>
    /// <param name="input">The guid.</param>
    /// <returns>The target of the mapping.</returns>
    public partial Span<byte> MapFromGuidToSpan(Guid input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="ReadOnlySpan{T}"/> of <see cref="byte"/>s.
    /// </summary>
    /// <param name="input">The guid.</param>
    /// <returns>The target of the mapping.</returns>
    public partial ReadOnlySpan<byte> MapFromGuidToReadOnlySpan(Guid input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="Memory{T}"/> of <see cref="byte"/>s.
    /// </summary>
    /// <param name="input">The guid.</param>
    /// <returns>The target of the mapping.</returns>
    public partial Memory<byte> MapFromGuidToMemory(Guid input);

    /// <summary>
    /// Map <see cref="Guid"/> to <see cref="ReadOnlyMemory{T}"/> of <see cref="byte"/>s.
    /// </summary>
    /// <param name="input">The guid.</param>
    /// <returns>The target of the mapping.</returns>
    public partial ReadOnlyMemory<byte> MapFromGuidToReadOnlyMemory(Guid input);

    /// <summary>
    /// Map <see cref="Array"/> of <see cref="byte"/>s to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The guid.</returns>
    public partial Guid MapArrayToGuid(byte[] input);

    /// <summary>
    /// Map <see cref="Span{T}"/> of <see cref="byte"/>s to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The guid.</returns>
    public partial Guid MapSpanToGuid(Span<byte> input);

    /// <summary>
    /// Map <see cref="ReadOnlySpan{T}"/> of <see cref="byte"/>s to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The guid.</returns>
    public partial Guid MapReadOnlySpanToGuid(ReadOnlySpan<byte> input);

    /// <summary>
    /// Map <see cref="Memory{T}"/> of <see cref="byte"/>s to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The guid.</returns>
    public partial Guid MapMemoryToGuid(Memory<byte> input);

    /// <summary>
    /// Map <see cref="ReadOnlyMemory{T}"/> of <see cref="byte"/>s to <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The guid.</returns>
    public partial Guid MapReadOnlyMemoryToGuid(ReadOnlyMemory<byte> input);
}