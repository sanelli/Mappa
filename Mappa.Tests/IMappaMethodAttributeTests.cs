// <copyright file="IMappaMethodAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="IMappaMethodAttribute"/> marker implementation.
/// </summary>
public sealed class IMappaMethodAttributeTests
{
    /// <summary>
    /// Tests that representative method-applicable attributes implement <see cref="IMappaMethodAttribute"/>.
    /// </summary>
    /// <param name="attributeType">The attribute type under test.</param>
    [Theory]
    [UnitTest]
    [InlineData(typeof(MappaUsePropertyAttribute))]
    [InlineData(typeof(MappaIgnoreTargetPropertyAttribute))]
    [InlineData(typeof(MappaMustMapTargetPropertyAttribute))]
    [InlineData(typeof(MappaAssignFromContextAttribute))]
    [InlineData(typeof(MappaAssignFromConstantAttribute))]
    [InlineData(typeof(MappaAssignToContextAttribute))]
    [InlineData(typeof(MappaInvokeMethodAttribute))]
    [InlineData(typeof(MappaNullSubstituteAttribute))]
    [InlineData(typeof(MappaBeforeMapAttribute))]
    [InlineData(typeof(MappaAfterMapAttribute))]
    [InlineData(typeof(MappaObjectFactoryAttribute))]
    [InlineData(typeof(MappaTransformerAttribute))]
    [InlineData(typeof(MappaApplyTransformerAttribute))]
    [InlineData(typeof(MappaProjectionAttribute))]
    [InlineData(typeof(MappaIgnoreAttribute))]
    [InlineData(typeof(MappaAllowInaccessibleSourceMembersAttribute))]
    [InlineData(typeof(MappaAllowInaccessibleTargetMembersAttribute))]
    [InlineData(typeof(MappaUseConstructorAttribute))]
    [InlineData(typeof(MappaTypeMappingAttribute))]
    [InlineData(typeof(MappaTypeMappingDefaultAttribute))]
    [InlineData(typeof(MappaSourcePreferredInterfaceImplementationAttribute))]
    [InlineData(typeof(MappaTargetPreferredInterfaceImplementationAttribute))]
    [InlineData(typeof(MappaProtobufTypeMappingAttribute))]
    [InlineData(typeof(MappaProtobufTypeMappingDefaultAttribute))]
    [InlineData(typeof(MappaProtobufPolymorphismSetupAttribute))]
    public void MethodApplicableAttributesImplementIMappaMethodAttribute(Type attributeType)
    {
        // Assert
        typeof(IMappaMethodAttribute).IsAssignableFrom(attributeType).Should().BeTrue();
    }

    /// <summary>
    /// Tests that generic enum configuration attributes implement <see cref="IMappaMethodAttribute"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumConfigurationAttributesImplementIMappaMethodAttribute()
    {
        // Assert
        typeof(IMappaMethodAttribute).IsAssignableFrom(typeof(MappaMapEnumMemberAttribute<DayOfWeek>)).Should().BeTrue();
        typeof(IMappaMethodAttribute).IsAssignableFrom(typeof(MappaMapEnumMemberAttribute<DayOfWeek, ConsoleColor>)).Should().BeTrue();
        typeof(IMappaMethodAttribute).IsAssignableFrom(typeof(MappaMapEnumIgnoreAttribute<DayOfWeek>)).Should().BeTrue();
        typeof(IMappaMethodAttribute).IsAssignableFrom(typeof(MappaMapEnumDefaultAttribute<DayOfWeek>)).Should().BeTrue();
    }

    /// <summary>
    /// Tests that settings and class-only attributes do not implement <see cref="IMappaMethodAttribute"/>.
    /// </summary>
    /// <param name="attributeType">The attribute type under test.</param>
    [Theory]
    [UnitTest]
    [InlineData(typeof(MappaSettingsAttribute))]
    [InlineData(typeof(MappaAttribute))]
    [InlineData(typeof(MappaInterceptorAttribute))]
    [InlineData(typeof(MappaInterceptorMethodDependencyAttribute))]
    [InlineData(typeof(MappaStaticDependencyAttribute))]
    [InlineData(typeof(MappaDependencyAttribute))]
    [InlineData(typeof(MappaDependencyInjectionAttribute))]
    public void SettingsAndClassOnlyAttributesDoNotImplementIMappaMethodAttribute(Type attributeType)
    {
        // Assert
        typeof(IMappaMethodAttribute).IsAssignableFrom(attributeType).Should().BeFalse();
    }
}