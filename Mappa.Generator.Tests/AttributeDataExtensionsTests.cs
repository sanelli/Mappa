// <copyright file="AttributeDataExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="AttributeDataExtensions"/>.
/// </summary>
public sealed class AttributeDataExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> reads a method-name constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReadsMethodNameConstructor()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault("DefaultMap")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaTypeMappingDefaultAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute!.MethodName.Should().Be("DefaultMap");
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.InvokeMethod);
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> reads a behavior-only constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReadsBehaviorConstructor()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.Null)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaTypeMappingDefaultAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute!.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.Null);
        attribute.MethodName.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> reads type and method constructors.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReadsTypeAndMethodConstructors()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public static class Dependency
                              {
                                  public static Target DefaultMap(Source input) => new();
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault(typeof(Dependency), "DefaultMap")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaTypeMappingDefaultAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute!.MethodName.Should().Be("DefaultMap");
        attribute.Type.Should().NotBeNull();
        attribute.Type!.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> reads behavior and type constructors.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReadsBehaviorAndTypeConstructors()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(Target))]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaTypeMappingDefaultAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute!.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.MapSourceType);
        attribute.Type.Should().NotBeNull();
        attribute.Type!.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetInvokeMethodAttributes"/> reads valid attribute constructors.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetInvokeMethodAttributesReadsValidConstructors()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Property { get; set; }
                              }

                              public class Target
                              {
                                  public string Property { get; set; }
                              }

                              public static class ExternalHelper
                              {
                                  public static string MapProperty(int value) => value.ToString();
                              }

                              public class InstanceHelper
                              {
                                  public string MapProperty(int value) => value.ToString();
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  private readonly InstanceHelper helper = new();

                                  [MappaInvokeMethod(nameof(Target.Property), "LocalMap")]
                                  [MappaInvokeMethod(nameof(Target.Property), typeof(ExternalHelper), "MapProperty")]
                                  [MappaInvokeMethod(nameof(Target.Property), nameof(helper), "MapProperty")]
                                  public partial Target Map(Source input);

                                  private string LocalMap(int value) => value.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var invokeAttributes = attributes.GetInvokeMethodAttributes(compilation);

        invokeAttributes.Should().HaveCount(3);
        invokeAttributes[0].TargetPropertyName.Should().Be("Property");
        invokeAttributes[0].MethodName.Should().Be("LocalMap");
        invokeAttributes[0].ClassType.Should().BeNull();
        invokeAttributes[0].FieldName.Should().BeNull();

        invokeAttributes[1].ClassType.Should().NotBeNull();
        invokeAttributes[1].ClassType!.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.ExternalHelper");
        invokeAttributes[1].FieldName.Should().BeNull();

        invokeAttributes[2].FieldName.Should().Be("helper");
        invokeAttributes[2].ClassType.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaStaticDependencies"/> reads multiple valid attributes.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaStaticDependenciesReadsMultipleValidAttributes()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public static class FirstDependency { }

                              public static class SecondDependency { }

                              [Mappa]
                              [MappaStaticDependency(typeof(FirstDependency))]
                              [MappaStaticDependency(typeof(SecondDependency))]
                              public sealed partial class TestMapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetTypeAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName);

        var dependencies = attributes.GetMappaStaticDependencies(compilation);

        dependencies.Should().HaveCount(2);
        dependencies[0].ToDisplayString().Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.FirstDependency");
        dependencies[1].ToDisplayString().Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.SecondDependency");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaDependencyInjectionAttributeData"/> reads ctor and named arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaDependencyInjectionAttributeDataReadsCtorAndNamedArguments()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public interface IIgnored
                              {
                              }

                              [MappaDependencyInjection(
                                  "RegisterFromCtor",
                                  MethodName = "RegisterFromProperty",
                                  ExtensionMethod = false,
                                  Accessibility = MappaDependencyInjectionMethodAccessibility.Internal,
                                  ServiceLifetime = MappaDependencyInjectionServiceLifetime.Scoped,
                                  InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceAndClass,
                                  IgnoreType = new[] { typeof(IIgnored) })]
                              public static partial class TestRegistrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetTypeAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.NamespaceName + ".TestRegistrar");

        var attribute = attributes.GetMappaDependencyInjectionAttributeData(compilation);

        attribute.Should().NotBeNull();
        attribute!.ConstructorMethodName.Should().Be("RegisterFromCtor");
        attribute.MethodName.Should().Be("RegisterFromProperty");
        attribute.ExtensionMethod.Should().BeFalse();
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Internal);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Scoped);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.InterfaceAndClass);
        attribute.IgnoreTypes.Should().HaveCount(1);
        attribute.IgnoreTypes[0].ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".IIgnored");
        attribute.ResolveMethodName("TestRegistrar").Should().Be("RegisterFromProperty");
    }
}