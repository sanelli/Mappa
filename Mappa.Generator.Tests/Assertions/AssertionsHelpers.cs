// <copyright file="AssertionsHelpers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions helpers.
/// </summary>
internal static partial class AssertionsHelpers
{
    /// <summary>
    /// Extract a symbol from the type name.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="type">The name of the type.</param>
    /// <returns>The symbol.</returns>
    internal static ITypeSymbol GetTypeSymbol(this Compilation compilation, string type)
    {
        ArgumentNullException.ThrowIfNull(compilation);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        // Remove heading global:: as it does not work very well with this code.
        if (type.StartsWith("global::", StringComparison.Ordinal))
        {
            return compilation.GetTypeSymbol(type["global::".Length..]);
        }

        // Manually handle named tuples
        if (type.StartsWith('(') && ContainSpacesRegex().Count(type) > 0)
        {
            var typeWithoutParenthesis = type.Substring(1, type.Length - 2);
            var elementTypes = typeWithoutParenthesis.Split(',');
            var elementTypeSymbols = new List<ITypeSymbol>();
            var elementTypeNames = new List<string?>();
            var elementLocations = new List<Location?>();
            foreach (var elementType in elementTypes)
            {
                var elementTypeAndName = ContainSpacesRegex().Split(elementType.Trim());
                var actualElementType = elementTypeAndName[0];
                elementTypeSymbols.Add(compilation.GetTypeSymbol(actualElementType));
                elementTypeNames.Add(elementTypeAndName.Length > 1 ? elementTypeAndName[1] : null);
                elementLocations.Add(Location.None);
            }

            var tupleType = compilation.CreateTupleTypeSymbol(
                [..elementTypeSymbols],
                [..elementTypeNames],
                [..elementLocations]);

            return tupleType;
        }

        if (type.EndsWith("[]", StringComparison.Ordinal))
        {
            var elementType = compilation.GetTypeByMetadataName(type.Replace("[]", string.Empty, StringComparison.Ordinal));
            var arraySymbol = compilation.CreateArrayTypeSymbol(elementType!, 1);
            return arraySymbol;
        }

        INamedTypeSymbol namedTypeSymbol;
        if (type.Contains('[', StringComparison.OrdinalIgnoreCase))
        {
            var firstIndexOfOpenBrackets = type.IndexOf('[', StringComparison.OrdinalIgnoreCase);
            var typeName = type[..firstIndexOfOpenBrackets];
            namedTypeSymbol = compilation.GetTypeByMetadataName(TypeSymbolExtensions.NormalizeType(typeName))!;
            var generics = SplitWithBoundaries(type.Substring(firstIndexOfOpenBrackets + 1, type.Length - firstIndexOfOpenBrackets - 2), ',', '[', ']');
            if (generics.Length > 0 && Array.TrueForAll(generics, generic => !string.IsNullOrWhiteSpace(generic)))
            {
                var typeArguments = generics
                    .Select(TypeSymbolExtensions.NormalizeType)
                    .Select(compilation.GetTypeSymbol)
                    .ToArray();
                var constructedGenericType = namedTypeSymbol.Construct(typeArguments);
                return constructedGenericType;
            }
        }
        else if (type.Contains('<', StringComparison.OrdinalIgnoreCase))
        {
            var firstIndexOfOpenAngularBracket = type.IndexOf('<', StringComparison.OrdinalIgnoreCase);

            var typeName = type[..firstIndexOfOpenAngularBracket];
            var generics = SplitWithBoundaries(type.Substring(firstIndexOfOpenAngularBracket + 1, type.Length - firstIndexOfOpenAngularBracket - 2), ',', '<', '>');
            namedTypeSymbol = compilation.GetTypeByMetadataName(TypeSymbolExtensions.NormalizeType($"{typeName}`{generics.Length}"))!;
            if (generics.Length > 0 && Array.TrueForAll(generics, generic => !string.IsNullOrWhiteSpace(generic)))
            {
                var typeArguments = generics
                    .Select(TypeSymbolExtensions.NormalizeType)
                    .Select(compilation.GetTypeSymbol)
                    .ToArray();
                var constructedGenericType = namedTypeSymbol.Construct(typeArguments);
                return constructedGenericType;
            }
        }
        else
        {
            namedTypeSymbol = compilation.GetTypeByMetadataName(TypeSymbolExtensions.NormalizeType(type))!;
        }

        return namedTypeSymbol;

        string[] SplitWithBoundaries(string s, char separator, char open, char close)
        {
            var parts = new List<string>();
            int opened = 0;
            var current = string.Empty;
            foreach (var character in s)
            {
                if (character == separator && opened == 0)
                {
                    parts.Add(current.Trim());
                    current = string.Empty;
                }
                else if (character == open)
                {
                    opened++;
                    current += character;
                }
                else if (character == close)
                {
                    opened--;
                    current += character;
                }
                else
                {
                    current += character;
                }
            }

            if (!string.IsNullOrWhiteSpace(current))
            {
                parts.Add(current.Trim());
            }

            return parts.ToArray();
        }
    }

    /// <summary>
    /// Obtain the correct list of assertions for a list of statement.
    /// </summary>
    /// <param name="statement">The list of statements.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The list of assertions for the statement.</returns>
    internal static IStatementSyntaxBaseAssertions ToAssertion(this StatementSyntax statement, SemanticModel semanticModel, Compilation compilation)
        => statement switch
            {
                BlockSyntax blockSyntax => new BlockSyntaxAssertions(blockSyntax, semanticModel, compilation),
                _ => new StatementSyntaxAssertions(statement),
            };

    /// <summary>
    /// Obtain the correct list of assertions for a list of statement.
    /// </summary>
    /// <param name="statements">The list of statements.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The list of assertions for the statement.</returns>
    internal static IStatementSyntaxBaseAssertions[] ToAssertions(this IEnumerable<StatementSyntax> statements, SemanticModel semanticModel, Compilation compilation)
        => statements
            .Select(statement => ToAssertion(statement, semanticModel, compilation))
            .ToArray();

    /// <summary>
    /// Obtain the correct list of assertions for a list of switch label syntax.
    /// </summary>
    /// <param name="switchSectionSyntaxes">The list of statements.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The list of assertions for the switch label syntax.</returns>
    internal static ISwitchLabelSyntaxAssertions[] ToAssertions(this IEnumerable<SwitchLabelSyntax> switchSectionSyntaxes, SemanticModel semanticModel, Compilation compilation)
        => switchSectionSyntaxes
            .Select(statement => statement switch
            {
                CaseSwitchLabelSyntax caseSwitchLabelSyntax => (ISwitchLabelSyntaxAssertions)new CaseSwitchLabelSyntaxAssertions(caseSwitchLabelSyntax, semanticModel, compilation),
                DefaultSwitchLabelSyntax defaultSwitchLabelSyntax => new DefaultSwitchLabelSyntaxAssertions(defaultSwitchLabelSyntax),
                _ => throw new ArgumentException($"Unknown switch label of type {statement.GetType().FullName}"),
            })
            .ToArray();

    [GeneratedRegex("\\s+")]
    private static partial Regex ContainSpacesRegex();
}