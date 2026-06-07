// <copyright file="MappaIgnoreTargetPropertyAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaIgnoreTargetPropertyAttributeMapper"/>.
/// </summary>
public sealed class MappaIgnoreTargetPropertyAttributeMapperUnitTests
{
    private readonly MappaIgnoreTargetPropertyAttributeMapper mapper = new();

    /// <summary>
    /// Test <see cref="MappaIgnoreTargetPropertyAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWhileIgnoringATargetProperty()
    {
        // Arrange
        var source = new MappaIgnoreTargetPropertySourceModel
        {
            MappedProperty = 17,
        };

        // Act
        var actual = this.mapper.Map(source);

        // Assert
        actual.MappedProperty.Should().Be(source.MappedProperty.ToString(CultureInfo.InvariantCulture));
        actual.IgnoredProperty.Should().Be(0);
    }
}