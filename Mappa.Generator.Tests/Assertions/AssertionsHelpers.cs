// <copyright file="AssertionsHelpers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions helpers.
/// </summary>
internal static class AssertionsHelpers
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

        if (type.EndsWith("[]", StringComparison.Ordinal))
        {
            var elementType = compilation.GetTypeByMetadataName(type.Replace("[]", string.Empty, StringComparison.Ordinal));
            var arraySymbol = compilation.CreateArrayTypeSymbol(elementType!, 1);
            return arraySymbol;
        }

        var typeParts = type.Split("[");
        var namedTypeSymbol = compilation.GetTypeByMetadataName(NormalizeType(typeParts[0]))!;
        if (typeParts.Length > 1)
        {
            var typeArguments = typeParts[^1]
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Split(",")
                .Select(NormalizeType)
                .Select(compilation.GetTypeByMetadataName)
                .Where(t => t is not null)
                .OfType<ITypeSymbol>()
                .ToArray();
            var constructedGenericType = namedTypeSymbol.Construct(typeArguments);
            return constructedGenericType;
        }

        return namedTypeSymbol;
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
                _ => new StatementSyntaxAssertions(statement, semanticModel, compilation),
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
                DefaultSwitchLabelSyntax defaultSwitchLabelSyntax => new DefaultSwitchLabelSyntaxAssertions(defaultSwitchLabelSyntax, semanticModel, compilation),
                _ => throw new ArgumentException($"Unknown switch label of type {statement.GetType().FullName}"),
            })
            .ToArray();

    private static string NormalizeType(string type)
        => type switch
        {
            "sbyte" => typeof(sbyte).ToString(),
            "short" => typeof(short).ToString(),
            "int" => typeof(int).ToString(),
            "long" => typeof(long).ToString(),
            "byte" => typeof(byte).ToString(),
            "ushort" => typeof(ushort).ToString(),
            "uint" => typeof(uint).ToString(),
            "ulong" => typeof(ulong).ToString(),
            "float" => typeof(float).ToString(),
            "double" => typeof(double).ToString(),
            "string" => typeof(string).ToString(),
            "char" => typeof(char).ToString(),
            _ => type,
        };
}