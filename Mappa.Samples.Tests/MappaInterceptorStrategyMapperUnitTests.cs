// <copyright file="MappaInterceptorStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Runtime tests for <see cref="MappaInterceptorStrategyMapper"/>.
/// </summary>
public sealed class MappaInterceptorStrategyMapperUnitTests
{
    private readonly MappaInterceptorStrategyMapper mapper = new();

    /// <summary>
    /// Test nullable unwrap on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullable()
    {
        this.mapper.MapNullable(7).Should().Be(7L);
    }

    /// <summary>
    /// Test enum-to-enum mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapEnum()
    {
        this.mapper.MapEnum(InterceptorSourceStatus.Active).Should().Be(InterceptorTargetStatus.Active);
    }

    /// <summary>
    /// Test string-to-int parsing on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToInt()
    {
        this.mapper.MapStringToInt("42").Should().Be(42);
    }

    /// <summary>
    /// Test long-to-<see cref="DateTimeOffset"/> on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapLongToDateTimeOffset()
    {
        this.mapper.MapLongToDateTimeOffset(1_700_000_000L)
            .Should().Be(DateTimeOffset.FromUnixTimeSeconds(1_700_000_000L));
    }

    /// <summary>
    /// Test list mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapList()
    {
        this.mapper.MapList([1, 2, 3]).Should().Equal("1", "2", "3");
    }

    /// <summary>
    /// Test dictionary mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDictionary()
    {
        var actual = this.mapper.MapDictionary(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 });
        actual.Should().ContainKey("a").WhoseValue.Should().Be(1L);
        actual.Should().ContainKey("b").WhoseValue.Should().Be(2L);
    }

    /// <summary>
    /// Test tuple mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTuple()
    {
        var actual = this.mapper.MapTuple(Tuple.Create(9, "x"));
        actual.Item1.Should().Be("9");
        actual.Item2.Should().Be("x");
    }

    /// <summary>
    /// Test Guid strategy on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuid()
    {
        var input = Guid.NewGuid();
        new Guid(this.mapper.MapGuid(input)).Should().Be(input);
    }

    /// <summary>
    /// Test structured property mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapPerson()
    {
        var actual = this.mapper.MapPerson(new InterceptorPersonSource { Name = "Ada", Age = 36 });
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }
}