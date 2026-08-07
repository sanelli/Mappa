// <copyright file="ReadonlyCollectionPropertyLoopBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Shared loop generation for readonly collection property map strategies.
/// </summary>
internal static class ReadonlyCollectionPropertyLoopBuilder
{
    /// <summary>
    /// The method used to append mapped elements to the target collection property.
    /// </summary>
    internal enum InsertionMethod
    {
        /// <summary>
        /// Use <c>Push</c>.
        /// </summary>
        Push,

        /// <summary>
        /// Use <c>Enqueue</c>.
        /// </summary>
        Enqueue,

        /// <summary>
        /// Use <c>Add</c>.
        /// </summary>
        Add,
    }

    /// <summary>
    /// Builds the source code that maps a source collection into a readonly target collection property.
    /// </summary>
    /// <param name="sourceType">The source collection type.</param>
    /// <param name="targetType">The target collection type.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="elementStrategy">The element mapping strategy.</param>
    /// <param name="insertionMethod">The method used to append mapped elements.</param>
    /// <param name="source">The source variable name.</param>
    /// <param name="context">The builder context.</param>
    /// <param name="mappaGlobalOptions">The global options.</param>
    /// <returns>The generated code.</returns>
    internal static (string VariableName, string Code) BuildSource(
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        IPropertySymbol targetProperty,
        MapStrategy elementStrategy,
        InsertionMethod insertionMethod,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var counterTemporary = context.NextTemporary();

        if (sourceType.IsArray() || sourceType.IsOrImplementIList())
        {
            stringBuilder.AppendLine($"for (int {counterTemporary} = 0; {counterTemporary} < {source}.{GetLengthPropertyName(sourceType)}; ++{counterTemporary})");
            using (stringBuilder.CurlyBracesBlock())
            {
                var elementTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"{sourceType.GetElementType().ToDisplayString()} {elementTemporary} = {source}[{counterTemporary}];");
                var (targetElementTemporary, targetElementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                    elementStrategy,
                    elementTemporary,
                    context,
                    mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(targetElementCode))
                {
                    stringBuilder.AppendLine(targetElementCode);
                }

                AppendElement(stringBuilder, context, targetType, targetProperty, targetElementTemporary, insertionMethod);
            }
        }
        else
        {
            stringBuilder.AppendLine($"foreach ({sourceType.GetElementType().ToDisplayString()} {counterTemporary} in {source})");
            using (stringBuilder.CurlyBracesBlock())
            {
                var (targetElementTemporary, targetElementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                    elementStrategy,
                    counterTemporary,
                    context,
                    mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(targetElementCode))
                {
                    stringBuilder.AppendLine(targetElementCode);
                }

                AppendElement(stringBuilder, context, targetType, targetProperty, targetElementTemporary, insertionMethod);
            }
        }

        return (string.Empty, stringBuilder.ToString());
    }

    private static void AppendElement(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetType,
        IPropertySymbol targetProperty,
        string targetElementTemporary,
        InsertionMethod insertionMethod)
    {
        var targetPropertyAccess = InaccessibleMemberAccessHelper.BuildTargetPropertyReadExpression(
            context.GetCompositeTypeTargetName(),
            targetProperty,
            context);

        switch (insertionMethod)
        {
            case InsertionMethod.Push:
                stringBuilder.AppendLine($"{targetPropertyAccess}.Push({targetElementTemporary});");
                break;
            case InsertionMethod.Enqueue:
                stringBuilder.AppendLine($"{targetPropertyAccess}.Enqueue({targetElementTemporary});");
                break;
            case InsertionMethod.Add:
                AppendAdd(stringBuilder, context, targetType, targetElementTemporary, targetPropertyAccess);
                break;
            default:
                throw new MappaGeneratorException($"Unexpected insertion method {insertionMethod}.");
        }
    }

    private static void AppendAdd(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetType,
        string targetElementTemporary,
        string targetPropertyAccess)
    {
        var elementType = targetType.GetElementType();
        var methodAccessMode = targetType.GetInterfaceMethodAccessMode(
            "Add",
            "System.Collections.Generic.ICollection",
            TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
            returnType => returnType.IsVoid(),
            [elementType]);

        if (methodAccessMode == InterfaceMethodAccessMode.InterfaceExplicit)
        {
            var interfaceTemporary = context.NextTemporary();
            stringBuilder.AppendLine($"System.Collections.Generic.ICollection<{elementType}> {interfaceTemporary} = {targetPropertyAccess};");
            stringBuilder.AppendLine($"{interfaceTemporary}.Add({targetElementTemporary});");
        }
        else
        {
            stringBuilder.AppendLine($"{targetPropertyAccess}.Add({targetElementTemporary});");
        }
    }

    private static string GetLengthPropertyName(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsArray())
        {
            return nameof(Array.Length);
        }

        if (typeSymbol.IsOrImplementICollection())
        {
            return nameof(ICollection<int>.Count);
        }

        throw new MappaGeneratorException($"Unable to get length property name for {typeSymbol}");
    }
}