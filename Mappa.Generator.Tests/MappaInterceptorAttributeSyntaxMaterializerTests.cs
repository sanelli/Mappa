// <copyright file="MappaInterceptorAttributeSyntaxMaterializerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorAttributeSyntaxMaterializer"/> via call-site analysis.
/// </summary>
public sealed class MappaInterceptorAttributeSyntaxMaterializerTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Materializes the supported method-attribute constructors and initializers from <c>With</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesSupportedMethodAttributeVariants()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaMustMapTargetPropertyAttribute("Required"),
                                                  new MappaAssignToContextAttribute("key", "Value"),
                                                  new MappaNullSubstituteAttribute("Name")
                                                  {
                                                      SourcePropertyName = "Alt",
                                                      InvokeMethod = "Fallback",
                                                  },
                                                  new MappaInvokeMethodAttribute("Value", "Compute")
                                                  {
                                                      SourcePropertyName = "SourceValue",
                                                  },
                                                  new MappaInvokeMethodAttribute("Other", "helper", "ComputeOther"),
                                                  new MappaBeforeMapAttribute("Before"),
                                                  new MappaBeforeMapAttribute("hooks", "BeforeOnHooks"),
                                                  new MappaAfterMapAttribute("After"),
                                                  new MappaAfterMapAttribute("hooks", "AfterOnHooks"),
                                                  new MappaApplyTransformerAttribute("Transform"),
                                                  new MappaIgnoreAttribute(),
                                                  new MappaTransformerAttribute(),
                                                  new MappaAllowInaccessibleSourceMembersAttribute("PrivateA", "PrivateB"),
                                                  new MappaAssignFromContextAttribute("FromContext", "ctx"),
                                                  new MappaAssignFromConstantAttribute("Constant", 42),
                                                  new MappaNullSubstituteAttribute("Literal")
                                                  {
                                                      AssignValue = "n/a",
                                                  },
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        var attributes = callSites[0].MethodAttributes;
        attributes.OfType<MappaMustMapTargetPropertyAttribute>().Should().ContainSingle(attribute =>
            attribute.TargetPropertyNames.SequenceEqual(new[] { "Required" }));
        attributes.OfType<MappaAssignToContextAttribute>().Should().ContainSingle(attribute =>
            attribute.ContextKey == "key" && attribute.TargetPropertyName == "Value");
        attributes.OfType<MappaAssignFromContextAttribute>().Should().ContainSingle(attribute =>
            attribute.TargetPropertyName == "FromContext" && attribute.ItemName == "ctx");
        attributes.OfType<MappaAssignFromConstantAttribute>().Should().ContainSingle(attribute =>
            attribute.TargetPropertyName == "Constant" && Equals(attribute.Value, 42));
        attributes.OfType<MappaNullSubstituteAttribute>().Should().Contain(attribute =>
            attribute.TargetPropertyName == "Name"
            && attribute.SourcePropertyName == "Alt"
            && attribute.InvokeMethod == "Fallback"
            && !attribute.HasAssignValue);
        attributes.OfType<MappaNullSubstituteAttribute>().Should().Contain(attribute =>
            attribute.TargetPropertyName == "Literal"
            && attribute.HasAssignValue
            && Equals(attribute.AssignValue, "n/a"));
        attributes.OfType<MappaInvokeMethodAttribute>().Should().Contain(attribute =>
            attribute.TargetPropertyName == "Value"
            && attribute.MethodName == "Compute"
            && attribute.SourcePropertyName == "SourceValue"
            && attribute.FieldName == null);
        attributes.OfType<MappaInvokeMethodAttribute>().Should().Contain(attribute =>
            attribute.TargetPropertyName == "Other"
            && attribute.MethodName == "ComputeOther"
            && attribute.FieldName == "helper");
        attributes.OfType<MappaBeforeMapAttribute>().Should().HaveCount(2);
        attributes.OfType<MappaAfterMapAttribute>().Should().HaveCount(2);
        attributes.OfType<MappaApplyTransformerAttribute>().Should().ContainSingle();
        attributes.OfType<MappaIgnoreAttribute>().Should().ContainSingle();
        attributes.OfType<MappaTransformerAttribute>().Should().ContainSingle();
        attributes.OfType<MappaAllowInaccessibleSourceMembersAttribute>().Should().ContainSingle(attribute =>
            attribute.MemberNames.SequenceEqual(new[] { "PrivateA", "PrivateB" }));
    }

    /// <summary>
    /// Materializes inaccessible-target allow attributes with and without member names.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesAllowInaccessibleTargetMembersVariants()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaAllowInaccessibleTargetMembersAttribute()
                                                  {
                                                      AllowFields = false,
                                                      AllowProperties = true,
                                                      AllowConstructors = false,
                                                  },
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.OfType<MappaAllowInaccessibleTargetMembersAttribute>()
            .Should().ContainSingle(attribute =>
                attribute.MemberNames.Length == 0
                && !attribute.AllowFields
                && attribute.AllowProperties
                && !attribute.AllowConstructors);
    }

    /// <summary>
    /// Non-constant inaccessible-target allow flags are ignored; constant flags still apply.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesAllowInaccessibleTargetMembersIgnoringNonConstantFlags()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  private static readonly bool DisableConstructors = false;

                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaAllowInaccessibleTargetMembersAttribute()
                                                  {
                                                      AllowFields = true,
                                                      AllowProperties = false,
                                                      AllowConstructors = DisableConstructors,
                                                  },
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();

        // AllowConstructors defaults to true; the non-constant assignment is ignored.
        callSites[0].MethodAttributes.OfType<MappaAllowInaccessibleTargetMembersAttribute>()
            .Should().ContainSingle(attribute =>
                attribute.AllowFields
                && !attribute.AllowProperties
                && attribute.AllowConstructors);
    }

    /// <summary>
    /// Materializes named inaccessible-target members with field writes enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesAllowInaccessibleTargetMembersWithNames()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaAllowInaccessibleTargetMembersAttribute("TargetPrivate")
                                                  {
                                                      AllowFields = true,
                                                  },
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.OfType<MappaAllowInaccessibleTargetMembersAttribute>()
            .Should().ContainSingle(attribute =>
                attribute.MemberNames.SequenceEqual(new[] { "TargetPrivate" })
                && attribute.AllowFields);
    }

    /// <summary>
    /// Materializes the parameterless inaccessible-source allow attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesAllowInaccessibleSourceMembersWithoutNames()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaAllowInaccessibleSourceMembersAttribute(),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.OfType<MappaAllowInaccessibleSourceMembersAttribute>()
            .Should().ContainSingle(attribute => attribute.MemberNames.Length == 0);
    }

    /// <summary>
    /// Materializes settings properties that use enum and style constants.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesRichSettingsInitializer()
    {
        const string source = """
                              using System.Globalization;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(new MappaSettingsAttribute
                                          {
                                              MapSourceFields = BooleanSetting.Enable,
                                              MapTargetFields = BooleanSetting.Disable,
                                              DateTimeFormat = "O",
                                              DateTimeStyle = DateTimeStyles.RoundtripKind,
                                              IntStyle = NumberStyles.AllowLeadingSign,
                                              GlobalNumberStyle = NumberStyles.Integer,
                                              CultureName = "en-US",
                                              CultureInfoSetting = CultureInfoSetting.UserDefined,
                                              MaxRuntimeDepth = (short)3,
                                              ReferenceReusing = BooleanSetting.Enable,
                                          });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        var settings = callSites[0].Settings.Should().ContainSingle().Subject;
        settings.MapSourceFields.Should().Be(BooleanSetting.Enable);
        settings.MapTargetFields.Should().Be(BooleanSetting.Disable);
        settings.DateTimeFormat.Should().Be("O");
        settings.DateTimeStyle.Should().Be(DateTimeStyles.RoundtripKind);
        settings.IntStyle.Should().Be(NumberStyles.AllowLeadingSign);
        settings.GlobalNumberStyle.Should().Be(NumberStyles.Integer);
        settings.CultureName.Should().Be("en-US");
        settings.CultureInfoSetting.Should().Be(CultureInfoSetting.UserDefined);
        settings.MaxRuntimeDepth.Should().Be(3);
        settings.ReferenceReusing.Should().Be(BooleanSetting.Enable);
    }

    /// <summary>
    /// Materializes the parameterless must-map attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesParameterlessMustMapTargetPropertyAttribute()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaMustMapTargetPropertyAttribute(),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.OfType<MappaMustMapTargetPropertyAttribute>()
            .Should().ContainSingle(attribute => attribute.TargetPropertyNames.Length == 0);
    }

    /// <summary>
    /// Empty settings object creation still materializes a default settings instance.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesEmptySettingsObjectCreation()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(new MappaSettingsAttribute());
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle().Which.Settings.Should().ContainSingle();
    }

    /// <summary>
    /// Collection expressions are accepted for method-attribute arrays.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeAcceptsCollectionExpressionForMethodAttributes()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes:
                                              [
                                                  new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                                  new MappaUsePropertyAttribute("Target", "Source"),
                                              ]);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.Should().HaveCount(2);
    }

    /// <summary>
    /// Implicit arrays are accepted for static-dependency arrays.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeAcceptsImplicitArrayForStaticDependencies()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaStaticDependencies: new[]
                                              {
                                                  new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].StaticDependencies.Should().ContainSingle()
            .Which.Name.Should().Be("HelperMapper");
    }

    /// <summary>
    /// Non-creation elements inside a collection expression report a diagnostic.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsNonCreationInsideCollectionExpression()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      var ignore = new MappaIgnoreTargetPropertyAttribute("Ignored");
                                      return this.MappaMap<string>(value)
                                          .With(mappaMethodAttributes: [ignore]);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// Unsupported materializable attribute types leave the method-attributes list empty
    /// while still recording the attribute type for validation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeSkipsUnsupportedMethodAttributeMaterialization()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaUseConstructorAttribute(ConstructorType.NoParameters),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeFalse();
        callSites[0].MethodAttributes.Should().BeEmpty();
    }

    /// <summary>
    /// Non-constant constructor arguments prevent method-attribute materialization.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeSkipsMethodAttributesWithNonConstantConstructorArguments()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  private const string TargetName = "Value";
                                  private static readonly string SourceName = "Alt";

                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaUsePropertyAttribute(TargetName, SourceName),
                                                  new MappaIgnoreTargetPropertyAttribute(SourceName),
                                                  new MappaAssignFromContextAttribute(SourceName, TargetName),
                                                  new MappaAssignFromConstantAttribute(SourceName, 1),
                                                  new MappaAssignToContextAttribute(TargetName, SourceName),
                                                  new MappaNullSubstituteAttribute(SourceName),
                                                  new MappaInvokeMethodAttribute(TargetName, SourceName),
                                                  new MappaInvokeMethodAttribute(TargetName, SourceName, TargetName),
                                                  new MappaBeforeMapAttribute(SourceName),
                                                  new MappaBeforeMapAttribute(SourceName, TargetName),
                                                  new MappaAfterMapAttribute(SourceName),
                                                  new MappaAfterMapAttribute(SourceName, TargetName),
                                                  new MappaApplyTransformerAttribute(SourceName),
                                                  new MappaMustMapTargetPropertyAttribute(SourceName),
                                                  new MappaAllowInaccessibleSourceMembersAttribute(SourceName),
                                                  new MappaAllowInaccessibleTargetMembersAttribute(SourceName),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.Should().BeEmpty();
    }

    /// <summary>
    /// Null-substitute and inaccessible-target attributes materialize without object initializers.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesAttributesWithoutInitializers()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaNullSubstituteAttribute("Name"),
                                                  new MappaAllowInaccessibleTargetMembersAttribute(),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.OfType<MappaNullSubstituteAttribute>()
            .Should().ContainSingle(attribute =>
                attribute.TargetPropertyName == "Name" && !attribute.HasAssignValue);
        callSites[0].MethodAttributes.OfType<MappaAllowInaccessibleTargetMembersAttribute>()
            .Should().ContainSingle();
    }

    /// <summary>
    /// Unknown or non-constant settings assignments are ignored during materialization.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeIgnoresInvalidSettingsAssignments()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  private static string Format() => "O";

                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(new MappaSettingsAttribute
                                          {
                                              DateTimeFormat = Format(),
                                              NotARealSetting = BooleanSetting.Enable,
                                          });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].Settings.Should().ContainSingle()
            .Which.DateTimeFormat.Should().BeNull();
    }

    /// <summary>
    /// Static-dependency materialization rejects non-<c>typeof</c> constructor arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsStaticDependencyWithoutTypeOf()
    {
        const string source = """
                              using System;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  private static Type HelperType => typeof(HelperMapper);

                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaStaticDependencies: new[]
                                              {
                                                  new MappaStaticDependencyAttribute(HelperType),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// Static-dependency materialization rejects object creations that are not a single <c>typeof</c> argument.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryMaterializeStaticDependencyTypeRejectsWrongArgumentShape()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Holder
                              {
                                  public object Create() => new MappaIgnoreAttribute();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var creation = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes())
            .OfType<ObjectCreationExpressionSyntax>()
            .Single();
        var semanticModel = compilation.GetSemanticModel(creation.SyntaxTree);

        MappaInterceptorAttributeSyntaxMaterializer.TryMaterializeStaticDependencyType(
                creation,
                semanticModel,
                TestContext.Current.CancellationToken)
            .Should().BeNull();
    }

    /// <summary>
    /// <see cref="MappaInterceptorAttributeSyntaxMaterializer.StripGlobalPrefix"/> removes only a leading global alias.
    /// </summary>
    [Fact]
    [UnitTest]
    public void StripGlobalPrefixHandlesPrefixedAndBareNames()
    {
        MappaInterceptorAttributeSyntaxMaterializer.StripGlobalPrefix("global::Mappa.Attributes.MappaIgnoreAttribute")
            .Should().Be("Mappa.Attributes.MappaIgnoreAttribute");
        MappaInterceptorAttributeSyntaxMaterializer.StripGlobalPrefix("Mappa.Attributes.MappaIgnoreAttribute")
            .Should().Be("Mappa.Attributes.MappaIgnoreAttribute");
    }

    /// <summary>
    /// <see cref="MappaInterceptorAttributeSyntaxMaterializer.AllowsMultiple"/> returns false when AttributeUsage is absent.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllowsMultipleReturnsFalseWhenAttributeUsageIsAbsent()
    {
        const string source = """
                              using System;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class NoUsageAttribute : Attribute
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var attributeType = compilation.GetTypeByMetadataName(
            "Mappa.Generator.Tests.UnitTests.SourceCode.NoUsageAttribute");
        attributeType.Should().NotBeNull();
        MappaInterceptorAttributeSyntaxMaterializer.AllowsMultiple(attributeType).Should().BeFalse();
    }

    /// <summary>
    /// Nullable value-type detection used by settings conversion is covered directly.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsNullableValueTypeDetectsNullableValueTypes()
    {
        var method = typeof(MappaInterceptorAttributeSyntaxMaterializer)
            .GetMethod("IsNullableValueType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.Should().NotBeNull();
        if (method is null)
        {
            return;
        }

        method.Invoke(null, [typeof(int?)]).Should().Be(true);
        method.Invoke(null, [typeof(int)]).Should().Be(false);
        method.Invoke(null, [typeof(string)]).Should().Be(false);
    }

    /// <summary>
    /// Settings property application swallows conversion failures for incompatible constants.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ApplySettingsPropertySwallowsIncompatibleEnumConstants()
    {
        const string source = """
                              class C
                              {
                                  void M()
                                  {
                                      var x = "Enable";
                                  }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var literal = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes())
            .OfType<LiteralExpressionSyntax>()
            .Single();
        var semanticModel = compilation.GetSemanticModel(literal.SyntaxTree);
        var apply = typeof(MappaInterceptorAttributeSyntaxMaterializer)
            .GetMethod("ApplySettingsProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        apply.Should().NotBeNull();
        if (apply is null)
        {
            return;
        }

        var settings = new MappaSettingsAttribute();
        var act = () => apply.Invoke(
            null,
            [settings, nameof(MappaSettingsAttribute.MapSourceFields), literal, semanticModel, TestContext.Current.CancellationToken]);
        act.Should().NotThrow();
        settings.MapSourceFields.Should().Be(BooleanSetting.Undefined);
    }

    /// <summary>
    /// Settings constant conversion covers primitive branches that settings properties do not exercise.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConvertConstantToPropertyTypeCoversPrimitiveBranches()
    {
        var method = typeof(MappaInterceptorAttributeSyntaxMaterializer)
            .GetMethod("ConvertConstantToPropertyType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.Should().NotBeNull();
        if (method is null)
        {
            return;
        }

        method.Invoke(null, [true, typeof(bool)]).Should().Be(true);
        method.Invoke(null, [42, typeof(int)]).Should().Be(42);
        method.Invoke(null, [3L, typeof(long)]).Should().Be(3L);
    }

    /// <summary>
    /// Projection attributes without an initializer still materialize and are rejected as unsupported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesProjectionAttributeWithoutInitializer()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaProjectionAttribute(),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorUnsupportedFeature.Id);
    }

    private static (System.Collections.Immutable.ImmutableArray<MappaInterceptorCallSite> CallSites, List<Diagnostic> Diagnostics)
        Analyze(string source)
    {
        var compilation = BuildCompilation(source);
        var classDeclaration = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == "InlineMapper");
        var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
        var classSymbol = semanticModel.GetDeclaredSymbol(
            classDeclaration,
            TestContext.Current.CancellationToken);
        classSymbol.Should().NotBeNull();

        var diagnostics = new List<Diagnostic>();
        var callSites = MappaInterceptorCallSiteAnalyzer.Analyze(
            classDeclaration,
            classSymbol,
            "MappaMap",
            compilation,
            diagnostics.Add,
            TestContext.Current.CancellationToken);
        return (callSites, diagnostics);
    }
}