// <copyright file="EmptyToEmptyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="EmptyToEmptyMapper"/>.
/// </summary>
public sealed class EmptyToEmptyMapperUnitTests
{
    private readonly EmptyToEmptyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="EmptyToEmptyMapper.Map(EmptySourceModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapEmptySourceToEmptyTarget()
    {
        var source = new EmptySourceModel();

        var target = this.mapper.Map(source);

        target.Should().NotBeNull();
        target.Should().BeOfType<EmptyTargetModel>();
    }

    /// <summary>
    /// Unit test for <see cref="EmptyToEmptyMapper.Map(SourceWithEmptyNestedModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapSourceWithNestedEmptyType()
    {
        var source = new SourceWithEmptyNestedModel
        {
            Nested = new NestedEmptySourceModel(),
        };

        var target = this.mapper.Map(source);

        target.Should().NotBeNull();
        target.Nested.Should().NotBeNull();
        target.Nested.Should().BeOfType<NestedEmptyTargetModel>();
    }
}