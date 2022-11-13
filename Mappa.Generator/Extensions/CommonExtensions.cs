// <copyright file="CommonExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Common extension methods.
/// </summary>
internal static class CommonExtensions
{
    /// <summary>
    /// Check if <paramref name="o"/> is not null.
    /// </summary>
    /// <typeparam name="TType">The type of the object.</typeparam>
    /// <param name="o">The object to investigate.</param>
    /// <returns><c>true</c> if <paramref name="o"/> is not <c>null</c>, <c>false</c> otherwise.</returns>
    internal static bool IsNotNull<TType>(this TType? o)
            where TType : class
        => o is not null;
}