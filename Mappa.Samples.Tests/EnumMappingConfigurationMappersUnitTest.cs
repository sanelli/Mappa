// <copyright file="EnumMappingConfigurationMappersUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for enum mapping configuration sample mappers.
/// </summary>
public sealed class EnumMappingConfigurationMappersUnitTest
{
    private readonly EnumMemberIntMapper enumMemberIntMapper = new();
    private readonly EnumMemberStringMapper enumMemberStringMapper = new();
    private readonly EnumMemberTwoEnumMapper enumMemberTwoEnumMapper = new();
    private readonly EnumIgnoreMapper enumIgnoreMapper = new();
    private readonly EnumDefaultUseDefaultValueIntegralMapper enumDefaultUseDefaultValueIntegralMapper = new();
    private readonly EnumDefaultUseDefaultValueStringMapper enumDefaultUseDefaultValueStringMapper = new();
    private readonly EnumDefaultUseDefaultValueEnumMapper enumDefaultUseDefaultValueEnumMapper = new();
    private readonly EnumIgnoreAndDefaultMapper enumIgnoreAndDefaultMapper = new();
    private readonly EnumDefaultThrowMapper enumDefaultThrowMapper = new();
    private readonly EnumConfigClassPropertyMapper enumConfigClassPropertyMapper = new();
    private readonly EnumConfigMultiDefaultClassMapper enumConfigMultiDefaultClassMapper = new();

    /// <summary>
    /// Verifies integral member overrides are applied.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected integral code.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigStatus.Active, 0)]
    [InlineData(ConfigStatus.Inactive, 99)]
    [InlineData(ConfigStatus.Deprecated, 2)]
    public void EnumMemberIntMapperCanMapStatusToIntegral(ConfigStatus value, int expected)
    {
        // Act
        var actual = this.enumMemberIntMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies string member overrides are applied.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected string.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigStatus.Active, "Active")]
    [InlineData(ConfigStatus.Inactive, "disabled")]
    [InlineData(ConfigStatus.Deprecated, "Deprecated")]
    public void EnumMemberStringMapperCanMapStatusToString(ConfigStatus value, string expected)
    {
        // Act
        var actual = this.enumMemberStringMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies two-enum member overrides are applied.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected target status.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigSourceStatus.Online, ConfigTargetStatus.Online)]
    [InlineData(ConfigSourceStatus.Offline, ConfigTargetStatus.Standby)]
    public void EnumMemberTwoEnumMapperCanMapSourceStatusToTargetStatus(ConfigSourceStatus value, ConfigTargetStatus expected)
    {
        // Act
        var actual = this.enumMemberTwoEnumMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies unmapped source members throw when no default is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumMemberTwoEnumMapperThrowsWhenSourceStatusCannotBeMapped()
    {
        // Arrange
        var act = () => this.enumMemberTwoEnumMapper.Map(ConfigSourceStatus.Legacy);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Verifies mapped members are converted normally when another member is ignored.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected integral code.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigStatus.Active, 0)]
    [InlineData(ConfigStatus.Inactive, 1)]
    public void EnumIgnoreMapperCanMapNonIgnoredStatusToIntegral(ConfigStatus value, int expected)
    {
        // Act
        var actual = this.enumIgnoreMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies ignored members throw when no default fallback is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumIgnoreMapperThrowsWhenIgnoredStatusIsMapped()
    {
        // Arrange
        var act = () => this.enumIgnoreMapper.Map(ConfigStatus.Deprecated);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Verifies known members map to their integral values when a default is configured.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected integral code.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigStatus.Active, 0)]
    [InlineData(ConfigStatus.Inactive, 1)]
    public void EnumDefaultUseDefaultValueIntegralMapperCanMapKnownStatusToIntegral(ConfigStatus value, int expected)
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueIntegralMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies unmapped integral enum values return the configured default.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultUseDefaultValueIntegralMapperReturnsDefaultIntegralValueForUnmappedStatus()
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueIntegralMapper.Map((ConfigStatus)99);

        // Assert
        actual.Should().Be(42);
    }

    /// <summary>
    /// Verifies known members map to their string names when a default is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultUseDefaultValueStringMapperCanMapKnownStatusToString()
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueStringMapper.Map(ConfigStatus.Active);

        // Assert
        actual.Should().Be("Active");
    }

    /// <summary>
    /// Verifies unmapped enum values return the configured default string.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultUseDefaultValueStringMapperReturnsDefaultStringValueForUnmappedStatus()
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueStringMapper.Map((ConfigStatus)99);

        // Assert
        actual.Should().Be("unknown");
    }

    /// <summary>
    /// Verifies members with matching target names map normally when an enum default is configured.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected target status.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigSourceStatus.Online, ConfigTargetStatus.Online)]
    [InlineData(ConfigSourceStatus.Offline, ConfigTargetStatus.Offline)]
    public void EnumDefaultUseDefaultValueEnumMapperCanMapKnownSourceStatusToTargetStatus(ConfigSourceStatus value, ConfigTargetStatus expected)
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueEnumMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies unmapped source members return the configured enum default.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultUseDefaultValueEnumMapperReturnsDefaultEnumValueForUnmappedSourceStatus()
    {
        // Act
        var actual = this.enumDefaultUseDefaultValueEnumMapper.Map(ConfigSourceStatus.Legacy);

        // Assert
        actual.Should().Be(ConfigTargetStatus.Offline);
    }

    /// <summary>
    /// Verifies non-ignored members map normally when ignore and default are combined.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected integral code.</param>
    [Theory]
    [UnitTest]
    [InlineData(ConfigStatus.Active, 0)]
    [InlineData(ConfigStatus.Deprecated, 2)]
    public void EnumIgnoreAndDefaultMapperCanMapNonIgnoredStatusToIntegral(ConfigStatus value, int expected)
    {
        // Act
        var actual = this.enumIgnoreAndDefaultMapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Verifies ignored members use the configured default value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumIgnoreAndDefaultMapperReturnsDefaultIntegralValueForIgnoredStatus()
    {
        // Act
        var actual = this.enumIgnoreAndDefaultMapper.Map(ConfigStatus.Inactive);

        // Assert
        actual.Should().Be(42);
    }

    /// <summary>
    /// Verifies known members map to their integral values when throw is configured.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultThrowMapperCanMapKnownStatusToIntegral()
    {
        // Act
        var actual = this.enumDefaultThrowMapper.Map(ConfigStatus.Active);

        // Assert
        actual.Should().Be(0);
    }

    /// <summary>
    /// Verifies unmapped values throw when the default behaviour is throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultThrowMapperThrowsWhenStatusCannotBeMapped()
    {
        // Arrange
        var act = () => this.enumDefaultThrowMapper.Map((ConfigStatus)99);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Verifies nested enum properties honour member overrides on the root map method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumConfigClassPropertyMapperCanMapClassWithNestedEnumMemberOverride()
    {
        // Arrange
        var source = new EnumConfigSourceModel
        {
            Status = ConfigStatus.Inactive,
            Priority = ConfigPriority.High,
        };

        // Act
        var actual = this.enumConfigClassPropertyMapper.Map(source);

        // Assert
        actual.Status.Should().Be(99);
        actual.Priority.Should().Be(2);
    }

    /// <summary>
    /// Verifies nested enums map normally on the happy path with per-enum defaults.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumConfigMultiDefaultClassMapperCanMapClassWithMultipleEnumDefaults()
    {
        // Arrange
        var source = new EnumConfigMultiDefaultSourceModel
        {
            Status = ConfigStatus.Active,
            Priority = ConfigPriority.Normal,
        };

        // Act
        var actual = this.enumConfigMultiDefaultClassMapper.Map(source);

        // Assert
        actual.Status.Should().Be(0);
        actual.Priority.Should().Be(1);
    }

    /// <summary>
    /// Verifies the priority enum uses its configured default fallback.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumConfigMultiDefaultClassMapperReturnsDefaultPriorityCodeForUnmappedPriority()
    {
        // Arrange
        var source = new EnumConfigMultiDefaultSourceModel
        {
            Status = ConfigStatus.Active,
            Priority = (ConfigPriority)99,
        };

        // Act
        var actual = this.enumConfigMultiDefaultClassMapper.Map(source);

        // Assert
        actual.Status.Should().Be(0);
        actual.Priority.Should().Be(0);
    }

    /// <summary>
    /// Verifies the status enum throws when its configured default behaviour is throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumConfigMultiDefaultClassMapperThrowsWhenStatusCannotBeMappedOnClassMap()
    {
        // Arrange
        var source = new EnumConfigMultiDefaultSourceModel
        {
            Status = (ConfigStatus)99,
            Priority = ConfigPriority.Low,
        };
        var act = () => this.enumConfigMultiDefaultClassMapper.Map(source);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}