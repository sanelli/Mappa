// <copyright file="InvokeConstructorStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeConstructorStrategyMapper"/>.
/// </summary>
public sealed class InvokeConstructorStrategyMapperUnitTests
{
    private readonly InvokeConstructorStrategyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="InvokeConstructorStrategyMapper.Map(SourceRecordModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToClassWithConstructorUsingParameters()
    {
        // Arrange
        var source = new SourceRecordModel(123, CountingValues.Three);

        // Act
        var target = this.mapper.Map(source);

        // Arrange
        target.ParamA.Should().Be(source.ParamA.ToString(NumberFormatInfo.CurrentInfo));
        target.ParamB.Should().Be((int)source.ParamB);
    }
}