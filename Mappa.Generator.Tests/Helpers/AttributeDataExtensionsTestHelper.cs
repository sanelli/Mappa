// <copyright file="AttributeDataExtensionsTestHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Helper methods for compiling snippets and obtaining <see cref="AttributeData"/> in unit tests.
/// </summary>
internal static class AttributeDataExtensionsTestHelper
{
    /// <summary>
    /// The namespace used by attribute extension unit tests.
    /// </summary>
    internal const string NamespaceName = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// The metadata name of the test mapper class.
    /// </summary>
    internal const string MapperMetadataName = NamespaceName + ".TestMapper";

    /// <summary>
    /// Gets the attributes applied to a method in the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="typeMetadataName">The metadata name of the type declaring the method.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>The method attributes.</returns>
    internal static ImmutableArray<AttributeData> GetMethodAttributes(
        CSharpCompilation compilation,
        string typeMetadataName,
        string methodName)
    {
        var typeSymbol = compilation.GetTypeByMetadataName(typeMetadataName);
        if (typeSymbol is null)
        {
            throw new InvalidOperationException($"Type '{typeMetadataName}' was not found in the compilation.");
        }

        var method = typeSymbol.GetMembers(methodName).OfType<IMethodSymbol>().SingleOrDefault();
        if (method is null)
        {
            throw new InvalidOperationException($"Method '{methodName}' was not found on type '{typeMetadataName}'.");
        }

        return method.GetAttributes();
    }

    /// <summary>
    /// Gets the attributes applied to a type in the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="typeMetadataName">The metadata name of the type.</param>
    /// <returns>The type attributes.</returns>
    internal static ImmutableArray<AttributeData> GetTypeAttributes(
        CSharpCompilation compilation,
        string typeMetadataName)
    {
        var typeSymbol = compilation.GetTypeByMetadataName(typeMetadataName);
        if (typeSymbol is null)
        {
            throw new InvalidOperationException($"Type '{typeMetadataName}' was not found in the compilation.");
        }

        return typeSymbol.GetAttributes();
    }
}