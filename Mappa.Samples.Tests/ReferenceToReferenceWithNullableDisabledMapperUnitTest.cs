// <copyright file="ReferenceToReferenceWithNullableDisabledMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ReferenceToReferenceWithNullableDisabledMapper"/>.
/// </summary>
public sealed class ReferenceToReferenceWithNullableDisabledMapperUnitTest
{
    private readonly ReferenceToReferenceWithNullableDisabledMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="ReferenceToReferenceWithNullableDisabledMapper.Map"/>
    /// when input is not null.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapWhenNotNull()
    {
        // Arrange
        SourceClassModel source = new() { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.Map(source);

        // Arrange
        target.Should().NotBeNull();
        target!.ParamA.Should().Be($"{source.ParamA}");
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="ReferenceToReferenceWithNullableDisabledMapper.Map"/>
    /// when input is null.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void CanMapWhenNull()
    {
        // Act
        var target = this.mapper.Map(null);

        // Arrange
        target.Should().BeNull();
    }
}