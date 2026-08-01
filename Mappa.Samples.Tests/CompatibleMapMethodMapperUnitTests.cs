// <copyright file="CompatibleMapMethodMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="CompatibleMapMethodMapper"/>.
/// </summary>
public sealed class CompatibleMapMethodMapperUnitTests
{
    private readonly CompatibleMapMethodMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="CompatibleMapMethodMapper.Map(CompatibleMapMethodSource)"/>
    /// proving the nested property uses the compatible hand-written method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingCompatibleMapMethodForNestedProperty()
    {
        // Arrange
        var source = new CompatibleMapMethodSource
        {
            Property = new CompatibleMapMethodDerivedSource
            {
                Value = 42,
            },
        };

        // Act
        var target = this.mapper.Map(source);

        // Assert — MapInner adds 100 and sets Label; constructor mapping would copy Value unchanged.
        target.Property.Should().BeOfType<CompatibleMapMethodDerivedTarget>();
        target.Property.Value.Should().Be(142);
        ((CompatibleMapMethodDerivedTarget)target.Property).Label.Should().Be("mapped");
    }
}