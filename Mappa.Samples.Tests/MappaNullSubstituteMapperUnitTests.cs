// <copyright file="MappaNullSubstituteMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MappaNullSubstituteMapper"/>.
/// </summary>
public sealed class MappaNullSubstituteMapperUnitTests
{
    private readonly MappaNullSubstituteMapper mapper = new();

    /// <summary>
    /// Test null source members are replaced by AssignValue and InvokeMethod substitutes.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapSubstitutesNullSourceMembers()
    {
        var source = new NullSubstituteSourceModel
        {
            Name = null,
            Alias = null,
        };

        var target = this.mapper.Map(source);

        target.Name.Should().Be("Anonymous");
        target.Alias.Should().Be("NoAlias");
    }

    /// <summary>
    /// Test non-null source members are preserved.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapKeepsNonNullSourceMembers()
    {
        var source = new NullSubstituteSourceModel
        {
            Name = "Ada",
            Alias = "Lovelace",
        };

        var target = this.mapper.Map(source);

        target.Name.Should().Be("Ada");
        target.Alias.Should().Be("Lovelace");
    }
}