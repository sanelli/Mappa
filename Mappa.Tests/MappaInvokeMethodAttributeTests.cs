// <copyright file="MappaInvokeMethodAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInvokeMethodAttribute"/>.
/// </summary>
public sealed class MappaInvokeMethodAttributeTests
{
    /// <summary>
    /// Tests <see cref="MappaInvokeMethodAttribute.SourcePropertyName"/> is <c>null</c> when not set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyNameIsNullWhenNotSet()
    {
        // Act
        var attribute = new MappaInvokeMethodAttribute("TargetProperty", "MapMethod");

        // Assert
        attribute.SourcePropertyName.Should().BeNull();
    }

    /// <summary>
    /// Tests <see cref="MappaInvokeMethodAttribute.SourcePropertyName"/> returns the value set via object initializer
    /// on the <c>(targetPropertyName, methodName)</c> constructor overload.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyNameReturnsValueSetViaObjectInitializerOnTwoArgumentConstructor()
    {
        // Act
        var attribute = new MappaInvokeMethodAttribute("TargetProperty", "MapMethod")
        {
            SourcePropertyName = "SourceProperty",
        };

        // Assert
        attribute.SourcePropertyName.Should().Be("SourceProperty");
    }

    /// <summary>
    /// Tests <see cref="MappaInvokeMethodAttribute.SourcePropertyName"/> returns the value set via object initializer
    /// on the <c>(targetPropertyName, classType, methodName)</c> constructor overload.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyNameReturnsValueSetViaObjectInitializerOnClassTypeConstructor()
    {
        // Act
        var attribute = new MappaInvokeMethodAttribute("TargetProperty", typeof(MappaContext), "MapMethod")
        {
            SourcePropertyName = "SourceProperty",
        };

        // Assert
        attribute.SourcePropertyName.Should().Be("SourceProperty");
    }

    /// <summary>
    /// Tests <see cref="MappaInvokeMethodAttribute.SourcePropertyName"/> returns the value set via object initializer
    /// on the <c>(targetPropertyName, fieldName, methodName)</c> constructor overload.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyNameReturnsValueSetViaObjectInitializerOnFieldNameConstructor()
    {
        // Act
        var attribute = new MappaInvokeMethodAttribute("TargetProperty", "dependency", "MapMethod")
        {
            SourcePropertyName = "SourceProperty",
        };

        // Assert
        attribute.SourcePropertyName.Should().Be("SourceProperty");
    }
}