// <copyright file="InvokeEmptyConstructorStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeEmptyConstructorStrategyMapper"/>.
/// </summary>
public sealed class InvokeEmptyConstructorStrategyMapperUnitTests
{
    private readonly InvokeEmptyConstructorStrategyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="InvokeEmptyConstructorStrategyMapper.Map(SourceClassModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToClassWithSingleEmptyConstructor()
    {
        // Arrange
        var source = new SourceClassModel { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.Map(source);

        // Arrange
        target.ParamA.Should().Be(source.ParamA.ToString(NumberFormatInfo.CurrentInfo));
        target.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="InvokeEmptyConstructorStrategyMapper.Map(SourceRecordModelWithEmptyConstructor)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToRecordWithSingleEmptyConstructor()
    {
        // Arrange
        var source = new SourceRecordModelWithEmptyConstructor { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.Map(source);

        // Arrange
        target.ParamA.Should().Be(source.ParamA.ToString(NumberFormatInfo.CurrentInfo));
        target.ParamB.Should().Be((int)source.ParamB);
    }
}