// <copyright file="AttributeSyntaxAssertionsExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Mappa.Generator.Tests.Assertions.Extensions;

/// <summary>
/// Assertion extensions for <see cref="AttributeSyntaxAssertions"/>.
/// </summary>
[DebuggerNonUserCode]
public static class AttributeSyntaxAssertionsExtensions
{
    /// <summary>
    /// Assert that the attribute is a <see cref="GeneratedCodeAttribute"/>.
    /// </summary>
    /// <param name="this">The attribute syntax assertions.</param>
    /// <returns>The assertions.</returns>
    public static AttributeSyntaxAssertions BeMappaGeneratedCodeAttribute(this AttributeSyntaxAssertions @this)
    {
        ArgumentNullException.ThrowIfNull(@this);

        return @this.BeGeneratedCodeAttribute("\"Mappa\"", $"\"{MappaGeneratorConsts.MappaGeneratorVersion.ToString()}\"");
    }
}