// <copyright file="MappaNullSubstituteAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaNullSubstituteAttribute"/>.
/// </summary>
public sealed class MappaNullSubstituteAttributeTests
{
    /// <summary>
    /// Tests the constructor initializes the target and leaves the source unset.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorInitializesTargetPropertyNameWithOptionalSourceUnset()
    {
        // Act
        var attribute = new MappaNullSubstituteAttribute("Target.Value");

        // Assert
        attribute.TargetPropertyName.Should().Be("Target.Value");
        attribute.SourcePropertyName.Should().BeNull();
        attribute.AssignValue.Should().BeNull();
        attribute.HasAssignValue.Should().BeFalse();
        attribute.InvokeMethod.Should().BeNull();
        attribute.InvokeMethodType.Should().BeNull();
        attribute.InvokeMethodSource.Should().BeNull();
    }

    /// <summary>
    /// Tests <see cref="MappaNullSubstituteAttribute.SourcePropertyName"/> can be set via object initializer.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyNameCanBeSetViaObjectInitializer()
    {
        // Act
        var attribute = new MappaNullSubstituteAttribute("Target.Value")
        {
            SourcePropertyName = "Source.Value",
        };

        // Assert
        attribute.TargetPropertyName.Should().Be("Target.Value");
        attribute.SourcePropertyName.Should().Be("Source.Value");
    }

    /// <summary>
    /// Tests <see cref="MappaNullSubstituteAttribute.AssignValue"/> can be set via object initializer.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AssignValueCanBeSetViaObjectInitializer()
    {
        // Act
        var attribute = new MappaNullSubstituteAttribute("Value")
        {
            SourcePropertyName = "Value",
            AssignValue = "Other Value",
            HasAssignValue = true,
        };

        // Assert
        attribute.AssignValue.Should().Be("Other Value");
        attribute.HasAssignValue.Should().BeTrue();
        attribute.InvokeMethod.Should().BeNull();
    }

    /// <summary>
    /// Tests <see cref="MappaNullSubstituteAttribute.InvokeMethod"/> and related locators can be set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InvokeMethodAndLocatorsCanBeSetViaObjectInitializer()
    {
        // Act
        var attribute = new MappaNullSubstituteAttribute("Value")
        {
            SourcePropertyName = "Value",
            InvokeMethod = "Substitute",
            InvokeMethodType = typeof(string),
            InvokeMethodSource = "helper",
        };

        // Assert
        attribute.InvokeMethod.Should().Be("Substitute");
        attribute.InvokeMethodType.Should().Be<string>();
        attribute.InvokeMethodSource.Should().Be("helper");
        attribute.AssignValue.Should().BeNull();
    }

    /// <summary>
    /// Tests the attribute implements <see cref="IMappaTargetPropertyNameAttribute"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ImplementsIMappaTargetPropertyNameAttribute()
    {
        // Act
        var attribute = new MappaNullSubstituteAttribute("Target")
        {
            SourcePropertyName = "Source",
        };

        // Assert
        attribute.Should().BeAssignableTo<IMappaTargetPropertyNameAttribute>();
        attribute.TargetPropertyName.Should().Be("Target");
    }

    /// <summary>
    /// Tests the attribute targets methods and allows multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaNullSubstituteAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}