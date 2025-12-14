// <copyright file="PolymorphicMethodMapMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="Samples.PolymorphicMethodMapMapper"/>.
/// </summary>
public sealed class PolymorphicMethodMapMapperUnitTests
{
    private static readonly PolymorphicMethodMapMapper PolymorphicMethodMapMapper = new();

    /// <summary>
    /// Test <see cref="PolymorphicMethodMapMapper.Map(Models.Polymorphism.One.SourceWithDependency)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    private void PolymorphicMethodMapMapperCanMapCorrectlyPickingUpThePolymorphicMethod()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            ThirdClass = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456, GuidProperty = Guid.NewGuid(), Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapper.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.ThirdClass.Should().NotBeNull();
        target.ThirdClass.NumericProperty.Should().Be(456);
        target.ThirdClass.GuidProperty.Should().Be(source.ThirdClass.GuidProperty.ToString());
        target.ThirdClass.Numbers.Should().BeEquivalentTo([7, 8, 9]);
    }
}