// <copyright file="AttributeDataExtensionsValueTransformerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for the value transformer related members of <see cref="AttributeDataExtensions"/>.
/// </summary>
public sealed class AttributeDataExtensionsValueTransformerTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads
    /// <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> set to <see cref="BooleanSetting.Enable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsMappaTransformerFromDependencyEnable()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var settings = attributes.GetMappaSettingsAttribute(compilation);

        settings.Should().NotBeNull();
        settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads
    /// <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> set to <see cref="BooleanSetting.Disable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsMappaTransformerFromDependencyDisable()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Disable)]
                              public sealed partial class TestMapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetTypeAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName);

        var settings = attributes.GetMappaSettingsAttribute(compilation);

        settings.Should().NotBeNull();
        settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Disable);
    }

    /// <summary>
    /// Test <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> stays
    /// <see cref="BooleanSetting.Undefined"/> when it is not specified.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeLeavesMappaTransformerFromDependencyUndefinedWhenNotSpecified()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Enable)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var settings = attributes.GetMappaSettingsAttribute(compilation);

        settings.Should().NotBeNull();
        settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Undefined);
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.HasMappaTransformerAttribute"/> detects the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void HasMappaTransformerAttributeDetectsTransformerMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  public partial Target Map(Source input);

                                  [MappaTransformer]
                                  private string Transformer(string value) => value;

                                  private string NotTransformer(string value) => value;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var transformerAttributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Transformer");
        var notTransformerAttributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "NotTransformer");

        transformerAttributes.HasMappaTransformerAttribute(compilation).Should().BeTrue();
        notTransformerAttributes.HasMappaTransformerAttribute(compilation).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaApplyTransformerAttributes"/> reads all constructors and <c>FieldName</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaApplyTransformerAttributesReadsAllConstructorsAndFieldName()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public static class Normalizers
                              {
                                  public static string Normalize(string value) => value;
                              }

                              public class InstanceNormalizer
                              {
                                  public string Normalize(string value) => value;
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  private readonly InstanceNormalizer normalizer = new();

                                  [MappaApplyTransformer(nameof(LocalNormalize))]
                                  [MappaApplyTransformer(typeof(Normalizers), nameof(Normalizers.Normalize))]
                                  [MappaApplyTransformer("Child.Name", nameof(LocalNormalize))]
                                  [MappaApplyTransformer("Name", typeof(Normalizers), nameof(Normalizers.Normalize))]
                                  [MappaApplyTransformer(nameof(InstanceNormalizer.Normalize), FieldName = nameof(normalizer))]
                                  [MappaApplyTransformer(42, "InvalidFirstArgument")]
                                  public partial Target Map(Source input);

                                  private string LocalNormalize(string value) => value;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var applyTransformers = attributes.GetMappaApplyTransformerAttributes(compilation);

        applyTransformers.Should().HaveCount(5);

        applyTransformers[0].MethodName.Should().Be("LocalNormalize");
        applyTransformers[0].ClassType.Should().BeNull();
        applyTransformers[0].SourcePropertyName.Should().BeNull();
        applyTransformers[0].FieldName.Should().BeNull();
        applyTransformers[0].Location.Should().NotBeNull();

        applyTransformers[1].MethodName.Should().Be("Normalize");
        applyTransformers[1].ClassType.Should().NotBeNull();
        applyTransformers[1].ClassType!.Name.Should().Be("Normalizers");
        applyTransformers[1].SourcePropertyName.Should().BeNull();
        applyTransformers[1].FieldName.Should().BeNull();

        applyTransformers[2].MethodName.Should().Be("LocalNormalize");
        applyTransformers[2].ClassType.Should().BeNull();
        applyTransformers[2].SourcePropertyName.Should().Be("Child.Name");
        applyTransformers[2].FieldName.Should().BeNull();

        applyTransformers[3].MethodName.Should().Be("Normalize");
        applyTransformers[3].ClassType.Should().NotBeNull();
        applyTransformers[3].ClassType!.Name.Should().Be("Normalizers");
        applyTransformers[3].SourcePropertyName.Should().Be("Name");
        applyTransformers[3].FieldName.Should().BeNull();

        applyTransformers[4].MethodName.Should().Be("Normalize");
        applyTransformers[4].ClassType.Should().BeNull();
        applyTransformers[4].SourcePropertyName.Should().BeNull();
        applyTransformers[4].FieldName.Should().Be("normalizer");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaApplyTransformerAttributes"/> returns nothing when the attribute is absent.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaApplyTransformerAttributesReturnsEmptyWhenAttributeIsAbsent()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        attributes.GetMappaApplyTransformerAttributes(compilation).Should().BeEmpty();
    }
}