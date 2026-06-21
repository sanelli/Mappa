// <copyright file="AttributeDataExtensionsGetMappaSettingsAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/>.
/// </summary>
public sealed class AttributeDataExtensionsGetMappaSettingsAttributeTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> returns <c>null</c>
    /// when no <see cref="MappaSettingsAttribute"/> is applied.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReturnsNullWhenAttributeIsAbsent()
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

        attributes.GetMappaSettingsAttribute(compilation).Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads rarely used format properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsRareFormatProperties()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(
                                      SByteFormat = "sb",
                                      UShortFormat = "us",
                                      UIntFormat = "ui",
                                      ULongFormat = "ul")]
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
        settings!.SByteFormat.Should().Be("sb");
        settings.UShortFormat.Should().Be("us");
        settings.UIntFormat.Should().Be("ui");
        settings.ULongFormat.Should().Be("ul");
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads style sentinel values.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsStyleSentinelValues()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(
                                      DateTimeStyle = MappaSettingsAttribute.UndefinedDateTimeStyle,
                                      IntStyle = MappaSettingsAttribute.UndefinedNumberStyle)]
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
        settings!.DateTimeStyle.Should().Be(MappaSettingsAttribute.UndefinedDateTimeStyle);
        settings.IntStyle.Should().Be(MappaSettingsAttribute.UndefinedNumberStyle);
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads combined number style flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsCombinedNumberStyleFlags()
    {
        const string source = """
                              using System.Globalization;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(IntStyle = NumberStyles.AllowThousands | NumberStyles.AllowParentheses)]
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
        settings!.IntStyle.Should().Be(NumberStyles.AllowThousands | NumberStyles.AllowParentheses);
    }

    /// <summary>
    /// Test <see cref="AttributeDataExtensions.GetMappaSettingsAttribute"/> reads invalid integer style casts.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaSettingsAttributeReadsInvalidIntegerStyleCasts()
    {
        const string source = """
                              using System.Globalization;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaSettings(
                                      DateTimeStyle = (DateTimeStyles)999,
                                      IntStyle = (NumberStyles)1048576)]
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
        settings!.DateTimeStyle.Should().Be((DateTimeStyles)999);
        settings.IntStyle.Should().Be((NumberStyles)1048576);
    }
}