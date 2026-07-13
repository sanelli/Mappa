// <copyright file="DictionaryMapEntryCodeBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Emits code that inserts a mapped key/value pair into a dictionary target.
/// </summary>
internal static class DictionaryMapEntryCodeBuilder
{
    /// <summary>
    /// Appends code that inserts a mapped entry into the target dictionary.
    /// </summary>
    /// <param name="builder">The code builder.</param>
    /// <param name="context">The builder context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="targetDictionaryType">The target dictionary type.</param>
    /// <param name="dictionaryAssignment">The dictionary assignment setting.</param>
    /// <param name="dictionaryExpression">The expression that references the target dictionary.</param>
    /// <param name="targetKeyTemporary">The temporary holding the mapped key.</param>
    /// <param name="targetValueTemporary">The temporary holding the mapped value.</param>
    internal static void AppendEntry(
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        Compilation compilation,
        ITypeSymbol targetDictionaryType,
        DictionaryAssignmentSetting dictionaryAssignment,
        string dictionaryExpression,
        string targetKeyTemporary,
        string targetValueTemporary)
    {
        var (keyType, valueType) = targetDictionaryType.GetKeyAndValueTypes(compilation);
        var interfaceTypeName = $"global::System.Collections.Generic.IDictionary<{keyType.ToDisplayString()},{valueType.ToDisplayString()}>";

        if (dictionaryAssignment is DictionaryAssignmentSetting.Add)
        {
            if (targetDictionaryType.GetIDictionaryInterfaceAddAccessMode(compilation) is InterfaceMethodAccessMode.InterfaceExplicit)
            {
                var interfaceTemporary = context.NextTemporary();
                builder.AppendLine($"{interfaceTypeName} {interfaceTemporary} = {dictionaryExpression};");
                builder.AppendLine($"{interfaceTemporary}.Add({targetKeyTemporary}, {targetValueTemporary});");
            }
            else
            {
                builder.AppendLine($"{dictionaryExpression}.Add({targetKeyTemporary}, {targetValueTemporary});");
            }
        }
        else if (targetDictionaryType.GetIDictionaryInterfaceIndexerAccessMode(compilation) is InterfaceMethodAccessMode.InterfaceExplicit)
        {
            var interfaceTemporary = context.NextTemporary();
            builder.AppendLine($"{interfaceTypeName} {interfaceTemporary} = {dictionaryExpression};");
            builder.AppendLine($"{interfaceTemporary}[{targetKeyTemporary}] = {targetValueTemporary};");
        }
        else
        {
            builder.AppendLine($"{dictionaryExpression}[{targetKeyTemporary}] = {targetValueTemporary};");
        }
    }
}