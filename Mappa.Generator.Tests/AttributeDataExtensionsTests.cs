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
        attribute.MethodName.Should().Be("DefaultMap");
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
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.Null);
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
        attribute.MethodName.Should().Be("DefaultMap");
        attribute.Type.Should().NotBeNull();
        attribute.Type.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
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
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.MapSourceType);
        attribute.Type.Should().NotBeNull();
        attribute.Type.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetTypeMappingAttributes"/> reads the generic type-mapping attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetTypeMappingAttributesReadsGenericAttribute()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public class SourceDerived : Source { }

                              public class TargetDerived : Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMapping<TargetDerived, SourceDerived>]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var typeMappings = attributes.GetTypeMappingAttributes(compilation);

        typeMappings.Should().HaveCount(1);
        typeMappings[0].TargetType.Should().NotBeNull();
        typeMappings[0].TargetType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.TargetDerived");
        typeMappings[0].SourceType.Should().NotBeNull();
        typeMappings[0].SourceType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.SourceDerived");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetTypeMappingAttributes"/> reads non-generic and generic attributes together.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetTypeMappingAttributesReadsNonGenericAndGenericAttributesTogether()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public class SourceFirst : Source { }

                              public class TargetFirst : Target { }

                              public class SourceSecond : Source { }

                              public class TargetSecond : Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                  [MappaTypeMapping<TargetSecond, SourceSecond>]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var typeMappings = attributes.GetTypeMappingAttributes(compilation);

        typeMappings.Should().HaveCount(2);
        typeMappings[0].TargetType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst");
        typeMappings[0].SourceType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst");
        typeMappings[1].TargetType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.TargetSecond");
        typeMappings[1].SourceType.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.SourceSecond");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetTypeMappingAttributes"/> ignores type-mapping attributes whose
    /// constructor type arguments are not named types (for example arrays).
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetTypeMappingAttributesIgnoresNonNamedConstructorTypeArguments()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMapping(typeof(string[]), typeof(Source))]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var typeMappings = attributes.GetTypeMappingAttributes(compilation);

        typeMappings.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> reads the generic default attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReadsGenericAttribute()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public class DefaultTarget : Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault<DefaultTarget>]
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
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.MapSourceType);
        attribute.Type.Should().NotBeNull();
        attribute.Type.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.DefaultTarget");
        attribute.MethodName.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetTypeMappingAttributes"/> ignores generic attributes whose
    /// type arguments are not named types (for example arrays).
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetTypeMappingAttributesIgnoresGenericAttributesWithNonNamedTypeArguments()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMapping<string[], Source>]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var typeMappings = attributes.GetTypeMappingAttributes(compilation);

        typeMappings.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> returns <see langword="null"/>
    /// when the type argument is not a named type (for example an array).
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReturnsNullForNonNamedTypeArgument()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(int[]))]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaTypeMappingDefaultAttribute(compilation);

        attribute.Should().BeNull();
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
                                  IgnoreType = new[] { typeof(IIgnored) },
                                  InjectFromAssemblies = new[] { typeof(IIgnored) })]
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
        attribute.ConstructorMethodName.Should().Be("RegisterFromCtor");
        attribute.MethodName.Should().Be("RegisterFromProperty");
        attribute.ExtensionMethod.Should().BeFalse();
        attribute.Accessibility.Should().Be(MappaDependencyInjectionMethodAccessibility.Internal);
        attribute.ServiceLifetime.Should().Be(MappaDependencyInjectionServiceLifetime.Scoped);
        attribute.InjectInterfaces.Should().Be(MappaDependencyInjectionInjectInterfaces.InterfaceAndClass);
        attribute.IgnoreTypes.Should().HaveCount(1);
        attribute.IgnoreTypes[0].ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".IIgnored");
        attribute.InjectFromAssemblies.Should().HaveCount(1);
        attribute.InjectFromAssemblies[0].ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".IIgnored");
        attribute.ResolveMethodName("TestRegistrar").Should().Be("RegisterFromProperty");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaAllowInaccessibleSourceMembersAttribute"/>
    /// reads params member names as primitive constructor arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaAllowInaccessibleSourceMembersAttributeReadsPrimitiveParamsMemberNames()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  private int First { get; set; }

                                  private int Second { get; set; }
                              }

                              public class Target
                              {
                                  public int First { get; set; }

                                  public int Second { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaAllowInaccessibleSourceMembers("First", "", "Second")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaAllowInaccessibleSourceMembersAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute.MemberNames.Should().Equal("First", "Second");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaAllowInaccessibleTargetMembersAttribute"/>
    /// reads a single primitive params member name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaAllowInaccessibleTargetMembersAttributeReadsSinglePrimitiveParamsMemberName()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Allowed { get; set; }
                              }

                              public class Target
                              {
                                  private int Allowed { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaAllowInaccessibleTargetMembers("Allowed")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaAllowInaccessibleTargetMembersAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute.MemberNames.Should().Equal("Allowed");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaAllowInaccessibleSourceMembersAttribute"/>
    /// with the parameterless constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaAllowInaccessibleSourceMembersAttributeReadsParameterlessConstructor()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  private int Value { get; set; }
                              }

                              public class Target
                              {
                                  public int Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaAllowInaccessibleSourceMembers]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaAllowInaccessibleSourceMembersAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaDependencyInjectionAttributeData"/>
    /// when IgnoreType and InjectFromAssemblies are empty arrays.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaDependencyInjectionAttributeDataReadsEmptyTypeArrays()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaDependencyInjection(
                                  IgnoreType = new System.Type[0],
                                  InjectFromAssemblies = new System.Type[0])]
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
        attribute.IgnoreTypes.Should().BeEmpty();
        attribute.InjectFromAssemblies.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaBeforeMapAttributes"/> reads valid constructors
    /// and ignores invalid location argument types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaBeforeMapAttributesReadsValidConstructorsAndIgnoresInvalidLocation()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public static class StaticHookHelpers
                              {
                                  public static void Before(Source source) { }
                              }

                              public class InstanceHookHelpers
                              {
                                  public void InstanceBefore(Source source) { }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  private readonly InstanceHookHelpers helper = new();

                                  [MappaBeforeMap(nameof(LocalBefore))]
                                  [MappaBeforeMap(typeof(StaticHookHelpers), nameof(StaticHookHelpers.Before))]
                                  [MappaBeforeMap(nameof(helper), "InstanceBefore")]
                                  [MappaBeforeMap(42, "InvalidLocation")]
                                  public partial Target Map(Source input);

                                  private void LocalBefore(Source source) { }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var beforeMaps = attributes.GetMappaBeforeMapAttributes(compilation);

        beforeMaps.Should().HaveCount(3);
        beforeMaps[0].MethodName.Should().Be("LocalBefore");
        beforeMaps[0].ClassType.Should().BeNull();
        beforeMaps[0].FieldName.Should().BeNull();
        beforeMaps[1].ClassType.Should().NotBeNull();
        beforeMaps[1].MethodName.Should().Be("Before");
        beforeMaps[2].FieldName.Should().Be("helper");
        beforeMaps[2].MethodName.Should().Be("InstanceBefore");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetInvokeMethodAttributes"/> ignores invalid constructors.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetInvokeMethodAttributesIgnoresInvalidConstructors()
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

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaInvokeMethod(nameof(Target.Property), 42, "LocalMap")]
                                  [MappaInvokeMethod(nameof(Target.Property), "LocalMap")]
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

        invokeAttributes.Should().HaveCount(1);
        invokeAttributes[0].MethodName.Should().Be("LocalMap");
        invokeAttributes[0].FieldName.Should().BeNull();
        invokeAttributes[0].ClassType.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaObjectFactoryAttributes"/> ignores invalid middle arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaObjectFactoryAttributesIgnoresInvalidMiddleArgument()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaObjectFactory(typeof(Target), 42, "CreateTarget")]
                                  [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                  public partial Target Map(Source input);

                                  private Target CreateTarget() => new();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var factories = attributes.GetMappaObjectFactoryAttributes(compilation);

        factories.Should().HaveCount(1);
        factories[0].MethodName.Should().Be("CreateTarget");
        factories[0].FieldName.Should().BeNull();
        factories[0].ClassType.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaTypeMappingDefaultAttribute"/> returns <see langword="null"/>
    /// when constructor arguments cannot be interpreted.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaTypeMappingDefaultAttributeReturnsNullForUninterpretableConstructorArguments()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaTypeMappingDefault(42.5)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        attributes.GetMappaTypeMappingDefaultAttribute(compilation).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaDependencyInjectionAttributeData"/>
    /// ignores non-named types in IgnoreType / InjectFromAssemblies arrays.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaDependencyInjectionAttributeDataIgnoresNonNamedTypesInTypeArrays()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public interface IIgnored
                              {
                              }

                              [MappaDependencyInjection(
                                  IgnoreType = new System.Type[] { typeof(int[]), typeof(IIgnored) },
                                  InjectFromAssemblies = new System.Type[] { typeof(string[]), typeof(IIgnored) })]
                              public static partial class TestRegistrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetTypeAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.NamespaceName + ".TestRegistrar");

        var attribute = attributes.GetMappaDependencyInjectionAttributeData(compilation);
        if (attribute is null)
        {
            throw new InvalidOperationException("Expected MappaDependencyInjection attribute data.");
        }

        attribute.IgnoreTypes.Should().HaveCount(1);
        attribute.IgnoreTypes[0].ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".IIgnored");
        attribute.InjectFromAssemblies.Should().HaveCount(1);
        attribute.InjectFromAssemblies[0].ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".IIgnored");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaDependencyInjectionAttributeData"/>
    /// returns empty type arrays when only non-named types are provided.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaDependencyInjectionAttributeDataReturnsEmptyWhenTypeArraysContainOnlyNonNamedTypes()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaDependencyInjection(
                                  IgnoreType = new System.Type[] { typeof(int[]), typeof(byte[]) },
                                  InjectFromAssemblies = new System.Type[] { typeof(string[]) })]
                              public static partial class TestRegistrar
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetTypeAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.NamespaceName + ".TestRegistrar");

        var attribute = attributes.GetMappaDependencyInjectionAttributeData(compilation);
        if (attribute is null)
        {
            throw new InvalidOperationException("Expected MappaDependencyInjection attribute data.");
        }

        attribute.IgnoreTypes.Should().BeEmpty();
        attribute.InjectFromAssemblies.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaAllowInaccessibleTargetMembersAttribute"/>
    /// reads <c>AllowProperties = false</c> and <c>AllowConstructors = false</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaAllowInaccessibleTargetMembersAttributeReadsAllowPropertiesAndAllowConstructorsFalse()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Value { get; set; }
                              }

                              public class Target
                              {
                                  private int Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaAllowInaccessibleTargetMembers(AllowProperties = false, AllowConstructors = false)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaAllowInaccessibleTargetMembersAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute.AllowProperties.Should().BeFalse();
        attribute.AllowConstructors.Should().BeFalse();
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaAllowInaccessibleTargetMembersAttribute"/>
    /// filters empty primitive params member names.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaAllowInaccessibleTargetMembersAttributeFiltersEmptyPrimitiveMemberNames()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Allowed { get; set; }
                              }

                              public class Target
                              {
                                  private int Allowed { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaAllowInaccessibleTargetMembers("", "Allowed", "")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var attribute = attributes.GetMappaAllowInaccessibleTargetMembersAttribute(compilation);

        attribute.Should().NotBeNull();
        attribute.MemberNames.Should().Equal("Allowed");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaStaticDependencies"/> ignores constructor
    /// arguments that are not <see cref="Microsoft.CodeAnalysis.INamedTypeSymbol"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaStaticDependenciesIgnoresNonNamedTypeConstructorArguments()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public static class ValidDependency { }

                              [Mappa]
                              [MappaStaticDependency(typeof(int[]))]
                              [MappaStaticDependency(typeof(ValidDependency))]
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

        dependencies.Should().HaveCount(1);
        dependencies[0].ToDisplayString().Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.ValidDependency");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetEnumMapIgnoreAttributes"/> keeps defined enum members
    /// and skips constants that do not match any enum member.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetEnumMapIgnoreAttributesSkipsUndefinedEnumMemberConstants()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SampleEnum
                              {
                                  Alpha = 1,
                                  Beta = 2,
                              }

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaMapEnumIgnore<SampleEnum>(SampleEnum.Alpha)]
                                  [MappaMapEnumIgnore<SampleEnum>((SampleEnum)999)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var ignores = attributes.GetEnumMapIgnoreAttributes(compilation);

        ignores.Should().HaveCount(1);
        ignores[0].EnumMemberName.Should().Be("Alpha");
        ignores[0].EnumType.ToDisplayString().Should().Be(AttributeDataExtensionsTestHelper.NamespaceName + ".SampleEnum");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetEnumMapMemberAttributes"/> ignores null string
    /// second constructor arguments and keeps a valid pairing.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetEnumMapMemberAttributesIgnoresNullStringSecondArgument()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SampleEnum
                              {
                                  Alpha = 1,
                                  Beta = 2,
                              }

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaMapEnumMember<SampleEnum>(SampleEnum.Alpha, null)]
                                  [MappaMapEnumMember<SampleEnum>(SampleEnum.Beta, "beta")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var members = attributes.GetEnumMapMemberAttributes(compilation);

        members.Should().HaveCount(1);
        members[0].EnumMemberName.Should().Be("Beta");
        members[0].StringValue.Should().Be("beta");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetEnumMapDefaultAttributes"/> ignores null string
    /// second constructor arguments and keeps a valid default.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetEnumMapDefaultAttributesIgnoresNullStringSecondArgument()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SampleEnum
                              {
                                  Alpha = 1,
                                  Beta = 2,
                              }

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaMapEnumDefault<SampleEnum>(MappaMapEnumDefaultBehavior.UseDefaultValue, null)]
                                  [MappaMapEnumDefault<SampleEnum>(MappaMapEnumDefaultBehavior.UseDefaultValue, "fallback")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var defaults = attributes.GetEnumMapDefaultAttributes(compilation);

        defaults.Should().HaveCount(1);
        defaults[0].Behavior.Should().Be(MappaMapEnumDefaultBehavior.UseDefaultValue);
        defaults[0].StringDefaultValue.Should().Be("fallback");
    }
}