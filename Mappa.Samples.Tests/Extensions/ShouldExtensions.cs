// <copyright file="ShouldExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

namespace Mappa.Samples.Tests.Extensions;

/// <summary>
/// Extension methods for assertions.
/// </summary>
internal static class ShouldExtensions
{
    /// <summary>
    /// Start asserting over <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="subject">The subject of the assertions.</param>
    /// <param name="expected">The expected list of elements.</param>
    /// <typeparam name="T">The element type.</typeparam>
    internal static void ShouldBeExactly<T>(this Span<T> subject, IList<T> expected)
    {
        subject.Length.Should().Be(expected.Count);
        for (int index = 0; index < expected.Count; ++index)
        {
            subject[index].Should().Be(expected[index], "Mismatch at index '{0}'", index);
        }
    }
}