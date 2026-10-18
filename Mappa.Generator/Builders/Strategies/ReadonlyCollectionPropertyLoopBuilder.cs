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
    /// <param name="preferredSourceInterface">The resolved preferred source interface, if any.</param>
    /// <param name="preferredTargetInterface">The resolved preferred target interface, if any.</param>
    /// <returns>The generated code.</returns>
    internal static (string VariableName, string Code) BuildSource(
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        IPropertySymbol targetProperty,
        MapStrategy elementStrategy,
        InsertionMethod insertionMethod,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        INamedTypeSymbol? preferredSourceInterface = null,
        INamedTypeSymbol? preferredTargetInterface = null)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var counterTemporary = context.NextTemporary();

        if (sourceType.IsArray() || sourceType.IsOrImplementIList())
        {
            stringBuilder.AppendLine($"for (int {counterTemporary} = 0; {counterTemporary} < {source}.{GetLengthPropertyName(sourceType)}; ++{counterTemporary})");
            using (stringBuilder.CurlyBracesBlock())
            {
                var elementTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"{GetSourceElementTypeDisplay(sourceType, preferredSourceInterface)} {elementTemporary} = {source}[{counterTemporary}];");
                var (targetElementTemporary, targetElementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                    elementStrategy,
                    elementTemporary,
                    context,
                    mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(targetElementCode))
                {
                    stringBuilder.AppendLine(targetElementCode);
                }

                AppendElement(
                    stringBuilder,
                    context,
                    targetType,
                    targetProperty,
                    targetElementTemporary,
                    insertionMethod,
                    preferredTargetInterface);
            }
        }
        else
        {
            var enumerableExpression = source;
            if (preferredSourceInterface is not null
                && sourceType is INamedTypeSymbol namedSourceType
                && PreferredInterfaceResolver.IsExplicitlyImplemented(
                    namedSourceType,
                    preferredSourceInterface,
                    context.Compilation))
            {
                var preferredInterfaceTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"{preferredSourceInterface.ToDisplayString()} {preferredInterfaceTemporary} = {source};");
                enumerableExpression = preferredInterfaceTemporary;
            }

            stringBuilder.AppendLine($"foreach ({GetSourceElementTypeDisplay(sourceType, preferredSourceInterface)} {counterTemporary} in {enumerableExpression})");
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

                AppendElement(
                    stringBuilder,
                    context,
                    targetType,
                    targetProperty,
                    targetElementTemporary,
                    insertionMethod,
                    preferredTargetInterface);
            }
        }

        return (string.Empty, stringBuilder.ToString());
    }

    private static string GetSourceElementTypeDisplay(ITypeSymbol sourceTypeSymbol, INamedTypeSymbol? preferredSourceInterface)
    {
        if (preferredSourceInterface is { TypeArguments.Length: 1 })
        {
            return preferredSourceInterface.TypeArguments[0].ToDisplayString();
        }

        return sourceTypeSymbol.GetElementType().ToDisplayString();
    }

    private static ITypeSymbol GetTargetElementType(ITypeSymbol targetType, INamedTypeSymbol? preferredTargetInterface)
    {
        if (preferredTargetInterface is { TypeArguments.Length: 1 })
        {
            return preferredTargetInterface.TypeArguments[0];
        }

        return targetType.GetElementType();
    }

    private static void AppendElement(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetType,
        IPropertySymbol targetProperty,
        string targetElementTemporary,
        InsertionMethod insertionMethod,
        INamedTypeSymbol? preferredTargetInterface)
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
                AppendAdd(
                    stringBuilder,
                    context,
                    targetType,
                    targetElementTemporary,
                    targetPropertyAccess,
                    preferredTargetInterface);
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
        string targetPropertyAccess,
        INamedTypeSymbol? preferredTargetInterface)
    {
        var elementType = GetTargetElementType(targetType, preferredTargetInterface);
        var methodAccessMode = targetType.GetInterfaceMethodAccessMode(
            "Add",
            "System.Collections.Generic.ICollection",
            TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
            returnType => returnType.IsVoid(),
            [elementType]);

        if (methodAccessMode == InterfaceMethodAccessMode.InterfaceExplicit)
        {
            var interfaceTemporary = context.NextTemporary();
            var interfaceDisplay = preferredTargetInterface is not null
                && PreferredInterfaceResolver.TryGetFamily(preferredTargetInterface, context.Compilation)
                    is PreferredInterfaceFamily.ICollection
                    or PreferredInterfaceFamily.IList
                    or PreferredInterfaceFamily.ISet
                ? preferredTargetInterface.ToDisplayString()
                : $"System.Collections.Generic.ICollection<{elementType.ToDisplayString()}>";
            stringBuilder.AppendLine($"{interfaceDisplay} {interfaceTemporary} = {targetPropertyAccess};");
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