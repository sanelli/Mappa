// <copyright file="MappaCloning.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Reflection;

namespace Mappa;

/// <summary>
/// Helpers for clone operations used by generated mappers.
/// </summary>
public static class MappaCloning
{
#pragma warning disable S3011 // Reflection is required to invoke protected object.MemberwiseClone
    private static readonly MethodInfo MemberwiseCloneMethod = typeof(object).GetMethod(
        "MemberwiseClone",
        BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("Cannot resolve object.MemberwiseClone.");
#pragma warning restore S3011

    /// <summary>
    /// Creates a shallow clone of <paramref name="source"/> using <see cref="object.MemberwiseClone"/>.
    /// </summary>
    /// <typeparam name="T">The reference type to clone.</typeparam>
    /// <param name="source">The source instance.</param>
    /// <returns>A shallow clone of <paramref name="source"/>.</returns>
    public static T MemberwiseClone<T>(T source)
        where T : class
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return (T)MemberwiseCloneMethod.Invoke(source, null)!;
    }
}