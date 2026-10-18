// <copyright file="PreferredInterfaceModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

using System.Collections;

namespace Mappa.Samples.Models;

/// <summary>
/// Collection that implements both <see cref="IEnumerable{T}"/> of <see cref="int"/>
/// (implicit) and <see cref="IEnumerable{T}"/> of <see cref="string"/> (explicit).
/// </summary>
public sealed class DualEnumerable : IEnumerable<int>, IEnumerable<string>
{
    /// <inheritdoc />
    public IEnumerator<int> GetEnumerator()
    {
        yield return 1;
        yield return 2;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        yield return "a";
        yield return "b";
    }
}

/// <summary>
/// Concrete collection that implements both <see cref="ICollection{T}"/> of <see cref="int"/>
/// and <see cref="ICollection{T}"/> of <see cref="string"/>.
/// </summary>
public sealed class DualCollection : ICollection<int>, ICollection<string>
{
    private readonly List<int> ints = [];
    private readonly List<string> strings = [];

    /// <summary>
    /// Gets the values stored via <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    public IReadOnlyList<int> IntValues => this.ints;

    /// <summary>
    /// Gets the values stored via <see cref="ICollection{T}"/> of <see cref="string"/>.
    /// </summary>
    public IReadOnlyList<string> StringValues => this.strings;

    /// <inheritdoc />
    int ICollection<int>.Count => this.ints.Count;

    /// <inheritdoc />
    int ICollection<string>.Count => this.strings.Count;

    /// <inheritdoc />
    bool ICollection<int>.IsReadOnly => false;

    /// <inheritdoc />
    bool ICollection<string>.IsReadOnly => false;

    /// <inheritdoc />
    public void Add(int item) => this.ints.Add(item);

    /// <inheritdoc />
    void ICollection<string>.Add(string item) => this.strings.Add(item);

    /// <inheritdoc />
    public void Clear()
    {
        this.ints.Clear();
        this.strings.Clear();
    }

    /// <inheritdoc />
    void ICollection<string>.Clear() => this.strings.Clear();

    /// <inheritdoc />
    public bool Contains(int item) => this.ints.Contains(item);

    /// <inheritdoc />
    bool ICollection<string>.Contains(string item) => this.strings.Contains(item);

    /// <inheritdoc />
    public void CopyTo(int[] array, int arrayIndex) => this.ints.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    void ICollection<string>.CopyTo(string[] array, int arrayIndex) => this.strings.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(int item) => this.ints.Remove(item);

    /// <inheritdoc />
    bool ICollection<string>.Remove(string item) => this.strings.Remove(item);

    /// <inheritdoc />
    public IEnumerator<int> GetEnumerator() => this.ints.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => this.strings.GetEnumerator();
}

/// <summary>
/// Source DTO that nests <see cref="DualEnumerable"/>.
/// </summary>
public sealed class PreferredInterfaceSourceDto
{
    /// <summary>
    /// Gets or sets the nested dual enumerable.
    /// </summary>
    public DualEnumerable Items { get; set; } = new();
}

/// <summary>
/// Target DTO with a list of integers.
/// </summary>
public sealed class PreferredInterfaceIntTargetDto
{
    /// <summary>
    /// Gets the mapped items.
    /// </summary>
    public IList<int> Items { get; } = new List<int>();
}

/// <summary>
/// Target DTO with a list of strings.
/// </summary>
public sealed class PreferredInterfaceStringTargetDto
{
    /// <summary>
    /// Gets the mapped items.
    /// </summary>
    public IList<string> Items { get; } = new List<string>();
}