// <copyright file="AttributeDataExtensionsGetMappaNullSubstituteAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="AttributeDataExtensions.GetMappaNullSubstituteAttributes"/>.
/// </summary>
public sealed class AttributeDataExtensionsGetMappaNullSubstituteAttributeTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that AssignValue mode is detected from the named argument, including a null value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaNullSubstituteAttributesReadsAssignValueIncludingNull()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public string? Value { get; set; }
                              }

                              public class Target
                              {
                                  public string Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaNullSubstitute(nameof(Target.Value), SourcePropertyName = nameof(Source.Value), AssignValue = "Other")]
                                  [MappaNullSubstitute("Other", SourcePropertyName = "Other", AssignValue = null)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var nullSubstitutes = attributes.GetMappaNullSubstituteAttributes(compilation);

        nullSubstitutes.Should().HaveCount(2);
        nullSubstitutes[0].TargetPropertyName.Should().Be("Value");
        nullSubstitutes[0].SourcePropertyName.Should().Be("Value");
        nullSubstitutes[0].HasAssignValue.Should().BeTrue();
        nullSubstitutes[0].AssignValue.Should().BeOfType<TypedConstant>();
        ((TypedConstant)nullSubstitutes[0].AssignValue!).Value.Should().Be("Other");
        nullSubstitutes[0].InvokeMethod.Should().BeNull();

        nullSubstitutes[1].HasAssignValue.Should().BeTrue();
        nullSubstitutes[1].AssignValue.Should().BeOfType<TypedConstant>();
        ((TypedConstant)nullSubstitutes[1].AssignValue!).Value.Should().BeNull();
        nullSubstitutes[1].InvokeMethod.Should().BeNull();
    }

    /// <summary>
    /// Test that InvokeMethod mode and locators are read from named arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaNullSubstituteAttributesReadsInvokeMethodAndLocators()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class Helpers
                              {
                                  public static string Substitute(string? value) => value ?? string.Empty;
                              }

                              public sealed class Helper
                              {
                                  public string Substitute(string? value) => value ?? string.Empty;
                              }

                              public class Source
                              {
                                  public string? Value { get; set; }
                              }

                              public class Target
                              {
                                  public string Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  private readonly Helper helper = new();

                                  [MappaNullSubstitute(nameof(Target.Value), SourcePropertyName = nameof(Source.Value), InvokeMethod = nameof(Substitute))]
                                  [MappaNullSubstitute("Typed", SourcePropertyName = "Typed", InvokeMethod = "Substitute", InvokeMethodType = typeof(Helpers))]
                                  [MappaNullSubstitute("Sourced", SourcePropertyName = "Sourced", InvokeMethod = "Substitute", InvokeMethodSource = "helper")]
                                  public partial Target Map(Source input);

                                  private static string Substitute(string? value) => value ?? string.Empty;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var nullSubstitutes = attributes.GetMappaNullSubstituteAttributes(compilation);

        nullSubstitutes.Should().HaveCount(3);
        nullSubstitutes[0].HasAssignValue.Should().BeFalse();
        nullSubstitutes[0].InvokeMethod.Should().Be("Substitute");
        nullSubstitutes[0].InvokeMethodType.Should().BeNull();
        nullSubstitutes[0].InvokeMethodSource.Should().BeNull();

        nullSubstitutes[1].InvokeMethod.Should().Be("Substitute");
        nullSubstitutes[1].InvokeMethodType.Should().NotBeNull();
        nullSubstitutes[1].InvokeMethodType!.FullName.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Helpers");
        nullSubstitutes[1].InvokeMethodSource.Should().BeNull();

        nullSubstitutes[2].InvokeMethod.Should().Be("Substitute");
        nullSubstitutes[2].InvokeMethodType.Should().BeNull();
        nullSubstitutes[2].InvokeMethodSource.Should().Be("helper");
    }

    /// <summary>
    /// Test that absence of named mode arguments leaves HasAssignValue false and InvokeMethod null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaNullSubstituteAttributesLeavesModeUnsetWhenNamedArgumentsAbsent()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public string? Value { get; set; }
                              }

                              public class Target
                              {
                                  public string Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaNullSubstitute(nameof(Target.Value), SourcePropertyName = nameof(Source.Value))]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var nullSubstitutes = attributes.GetMappaNullSubstituteAttributes(compilation);

        nullSubstitutes.Should().ContainSingle();
        nullSubstitutes[0].HasAssignValue.Should().BeFalse();
        nullSubstitutes[0].AssignValue.Should().BeNull();
        nullSubstitutes[0].InvokeMethod.Should().BeNull();
    }

    /// <summary>
    /// Test that the single-argument constructor leaves <c>SourcePropertyName</c> unset,
    /// and that a named <c>SourcePropertyName</c> argument is applied when present.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaNullSubstituteAttributesSupportsOptionalSourcePropertyName()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public string? Value { get; set; }
                                  public string? Other { get; set; }
                              }

                              public class Target
                              {
                                  public string Value { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaNullSubstitute(nameof(Target.Value), AssignValue = "Default")]
                                  [MappaNullSubstitute("Named", SourcePropertyName = "Other", AssignValue = "NamedDefault")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            AttributeDataExtensionsTestHelper.MapperMetadataName,
            "Map");

        var nullSubstitutes = attributes.GetMappaNullSubstituteAttributes(compilation);

        nullSubstitutes.Should().HaveCount(2);
        nullSubstitutes[0].TargetPropertyName.Should().Be("Value");
        nullSubstitutes[0].SourcePropertyName.Should().BeNull();
        nullSubstitutes[0].HasAssignValue.Should().BeTrue();

        nullSubstitutes[1].TargetPropertyName.Should().Be("Named");
        nullSubstitutes[1].SourcePropertyName.Should().Be("Other");
        nullSubstitutes[1].HasAssignValue.Should().BeTrue();
    }
}