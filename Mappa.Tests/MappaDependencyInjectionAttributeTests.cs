// <copyright file="MappaDependencyInjectionAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDependencyInjectionAttribute"/>.
/// </summary>
public sealed class MappaDependencyInjectionAttributeTests
{
    /// <summary>
    /// Tests the parameterless constructor initializes empty ignore types and default tunables.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParameterlessConstructorInitializesDefaults()
    {
        // Act
        var attribute = new MappaDependencyInjectionAttribute();

        // Assert
        attribute.ConstructorMethodName.Should().BeNull();
        attribute.MethodName.Should().BeNull();
        attribute.ExtensionMethod.Should().BeTrue();
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Public);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Singleton);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.ClassOnly);
        attribute.IgnoreType.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the constructor method-name overload stores the name and keeps default tunables.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorMethodNameInitializesAttribute()
    {
        // Act
        var attribute = new MappaDependencyInjectionAttribute("RegisterMappers");

        // Assert
        attribute.ConstructorMethodName.Should().Be("RegisterMappers");
        attribute.MethodName.Should().BeNull();
        attribute.ExtensionMethod.Should().BeTrue();
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Public);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Singleton);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.ClassOnly);
        attribute.IgnoreType.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that all tunables can be assigned after construction.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TunablesCanBeAssigned()
    {
        // Act
        var attribute = new MappaDependencyInjectionAttribute
        {
            ExtensionMethod = false,
            MethodName = "RegisterAll",
            Accessibility = MappaDependencyInjectionMethodAccessibility.Internal,
            ServiceLifetime = MappaDependencyInjectionServiceLifetime.Scoped,
            InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceAndClass,
            IgnoreType = [typeof(string), typeof(MappaContext)],
        };

        // Assert
        attribute.ExtensionMethod.Should().BeFalse();
        attribute.MethodName.Should().Be("RegisterAll");
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Internal);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Scoped);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.InterfaceAndClass);
        attribute.IgnoreType.Should().Equal(typeof(string), typeof(MappaContext));
    }

    /// <summary>
    /// Tests that remaining enum values can be assigned on the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RemainingEnumValuesCanBeAssigned()
    {
        // Act
        var attribute = new MappaDependencyInjectionAttribute
        {
            Accessibility = MappaDependencyInjectionMethodAccessibility.ProtectedInternal,
            ServiceLifetime = MappaDependencyInjectionServiceLifetime.Transient,
            InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceOnly,
        };

        // Assert
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.ProtectedInternal);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Transient);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.InterfaceOnly);

        // Act
        attribute.Accessibility = MappaDependencyInjectionMethodAccessibility.Private;
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Private);

        // Act
        attribute.Accessibility = MappaDependencyInjectionMethodAccessibility.Protected;
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Protected);
    }

    /// <summary>
    /// Tests the attribute targets classes only and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsClassesAndDisallowsMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaDependencyInjectionAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Class);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}