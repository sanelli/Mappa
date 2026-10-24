// <copyright file="PropertyMapNameMatcherTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyMapNameMatcher"/>.
/// </summary>
public sealed class PropertyMapNameMatcherTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Prefers a matching property over a field with the same name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PrefersPropertyOverFieldWhenBothMatch()
    {
        const string source = """
                              public class Sample
                              {
                                  public int Value { get; set; }
                                  public int value;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var type = compilation.GetTypeByMetadataName("Sample");
        type.Should().NotBeNull();

        var properties = type.GetMembers().OfType<IPropertySymbol>().ToArray();
        var fields = type.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field.Name.Equals("value", StringComparison.Ordinal))
            .ToArray();

        var found = PropertyMapNameMatcher.TryFindSourceMember(
            properties,
            fields,
            "Value",
            BooleanSetting.Enable,
            BooleanSetting.Undefined,
            isConstructorParameterPath: false,
            useExactNameFromAttribute: false,
            out var match);

        found.Should().BeTrue();
        match.Should().NotBeNull();
        match.IsProperty.Should().BeTrue();
        match.Property.Should().NotBeNull();
        match.Property.Name.Should().Be("Value");
    }

    /// <summary>
    /// Matches a field when no property matches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MatchesFieldWhenNoPropertyMatches()
    {
        const string source = """
                              public class Sample
                              {
                                  public int Amount;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var type = compilation.GetTypeByMetadataName("Sample");
        type.Should().NotBeNull();

        var fields = type.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field.Name.Equals("Amount", StringComparison.Ordinal))
            .ToArray();

        var found = PropertyMapNameMatcher.TryFindSourceMember(
            sourceProperties: [],
            fields,
            "Amount",
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            isConstructorParameterPath: false,
            useExactNameFromAttribute: false,
            out var match);

        found.Should().BeTrue();
        match.Should().NotBeNull();
        match.IsField.Should().BeTrue();
        match.Field.Should().NotBeNull();
        match.Field.Name.Should().Be("Amount");
    }
}