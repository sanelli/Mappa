// <copyright file="InaccessibleMembersCoverageTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests covering inaccessible-member options, registry reuse, and support-gate branches.
/// </summary>
public sealed class InaccessibleMembersCoverageTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test target options allow-all, whitelist, and <c>AllowProperties = false</c> branches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetOptionsIsPropertyAllowedCoversAllowFlagsAndWhitelist()
    {
        var allowAll = InaccessibleTargetMemberOptions.FromAttribute(new MappaAllowInaccessibleTargetMembersAttribute());
        allowAll.Should().NotBeNull();
        allowAll.IsPropertyAllowed("Any").Should().BeTrue();

        var whitelist = InaccessibleTargetMemberOptions.FromAttribute(
            new MappaAllowInaccessibleTargetMembersAttribute("Allowed"));
        whitelist.Should().NotBeNull();
        whitelist.IsPropertyAllowed("Allowed").Should().BeTrue();
        whitelist.IsPropertyAllowed("Denied").Should().BeFalse();

        var propertiesDisabled = InaccessibleTargetMemberOptions.FromAttribute(
            new MappaAllowInaccessibleTargetMembersAttribute { AllowProperties = false });
        propertiesDisabled.Should().NotBeNull();
        propertiesDisabled.IsPropertyAllowed("Any").Should().BeFalse();

        InaccessibleTargetMemberOptions.FromAttribute(null).Should().BeNull();
    }

    /// <summary>
    /// Test source options allow-all and whitelist branches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourceOptionsIsMemberAllowedCoversAllowAllAndWhitelist()
    {
        var allowAll = InaccessibleSourceMemberOptions.FromAttribute(new MappaAllowInaccessibleSourceMembersAttribute());
        allowAll.Should().NotBeNull();
        allowAll.IsMemberAllowed("Any").Should().BeTrue();

        var whitelist = InaccessibleSourceMemberOptions.FromAttribute(
            new MappaAllowInaccessibleSourceMembersAttribute("Allowed"));
        whitelist.Should().NotBeNull();
        whitelist.IsMemberAllowed("Allowed").Should().BeTrue();
        whitelist.IsMemberAllowed("Denied").Should().BeFalse();

        InaccessibleSourceMemberOptions.FromAttribute(null).Should().BeNull();
    }

    /// <summary>
    /// Test attribute constructors accept a <c>null</c> member-name array.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeConstructorsAcceptNullMemberNames()
    {
        new MappaAllowInaccessibleSourceMembersAttribute(null).MemberNames.Should().BeEmpty();
        new MappaAllowInaccessibleTargetMembersAttribute(null).MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Test eligibility rejects indexers and get-only target properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetPropertyEligibilityRejectsIndexersAndGetOnlyProperties()
    {
        const string source = """
                              namespace Coverage;
                              public class Source
                              {
                                  public int Value { get; set; }
                              }

                              public class Target
                              {
                                  public int this[int index]
                                  {
                                      get => 0;
                                      set { }
                                  }

                                  public int GetOnly { get; } = 0;
                              }

                              public sealed class Mapper
                              {
                                  public Target Map(Source input) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var tree = compilation.SyntaxTrees.Single();
        var model = compilation.GetSemanticModel(tree);
        var mapMethodSyntax = tree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Single(method => method.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            mapMethodSyntax,
            model,
            nullableEnabled: true,
            TestContext.Current.CancellationToken);

        var targetType = compilation.GetTypeByMetadataName("Coverage.Target");
        targetType.Should().NotBeNull();

        var indexer = targetType.GetMembers().OfType<IPropertySymbol>().Single(property => property.IsIndexer);
        var getOnly = targetType.GetMembers("GetOnly").OfType<IPropertySymbol>().Single();
        var options = InaccessibleTargetMemberOptions.FromAttribute(new MappaAllowInaccessibleTargetMembersAttribute());

        InaccessibleMemberEligibility.TryIsTargetPropertyWritable(
                indexer,
                compilation,
                mapMethod,
                options,
                out var indexerRequiresUnsafe)
            .Should().BeFalse();
        indexerRequiresUnsafe.Should().BeFalse();

        InaccessibleMemberEligibility.TryIsTargetPropertyWritable(
                getOnly,
                compilation,
                mapMethod,
                options,
                out var getOnlyRequiresUnsafe)
            .Should().BeFalse();
        getOnlyRequiresUnsafe.Should().BeFalse();
    }

    /// <summary>
    /// Test target eligibility returns <c>false</c> when options are present but UnsafeAccessor is unsupported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetPropertyEligibilityRejectsWhenUnsafeAccessorIsUnsupported()
    {
        const string source = """
                              namespace Coverage;
                              public class Source
                              {
                                  public int Value { get; set; }
                              }

                              public class Target
                              {
                                  public int Value { get; private set; }
                              }

                              public sealed class Mapper
                              {
                                  public Target Map(Source input) => new Target();
                              }
                              """;

        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp11);
        var compilation = BuildCompilation(source, parseOptions);
        var tree = compilation.SyntaxTrees.Single();
        var model = compilation.GetSemanticModel(tree);
        var mapMethodSyntax = tree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Single(method => method.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            mapMethodSyntax,
            model,
            nullableEnabled: true,
            TestContext.Current.CancellationToken);

        var targetProperty = compilation.GetTypeByMetadataName("Coverage.Target")!
            .GetMembers("Value")
            .OfType<IPropertySymbol>()
            .Single();
        var options = InaccessibleTargetMemberOptions.FromAttribute(new MappaAllowInaccessibleTargetMembersAttribute());

        compilation.IsUnsafeAccessorSupported().Should().BeFalse();
        InaccessibleMemberEligibility.TryIsTargetPropertyWritable(
                targetProperty,
                compilation,
                mapMethod,
                options,
                out var requiresUnsafeAccessor)
            .Should().BeFalse();
        requiresUnsafeAccessor.Should().BeFalse();
    }

    /// <summary>
    /// Test the registry reuses accessors, exposes <see cref="InaccessibleAccessorRegistry.Accessors"/>,
    /// and covers <c>AccessorKey</c> equality including the boxed <see cref="object.Equals(object?)"/> path.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegistryReusesAccessorsAndCoversAccessorKeyEquality()
    {
        const string source = """
                              namespace Coverage;
                              public class Source
                              {
                                  private int Value { get; set; }
                              }

                              public class Target
                              {
                                  private Target()
                                  {
                                  }

                                  public int Value { get; private set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Coverage.Source");
        var targetType = compilation.GetTypeByMetadataName("Coverage.Target");
        sourceType.Should().NotBeNull();
        targetType.Should().NotBeNull();

        var sourceProperty = sourceType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Value").OfType<IPropertySymbol>().Single();
        var constructor = targetType.InstanceConstructors.Single(method => method.Parameters.Length == 0);

        var context = new MappaBuilderContext(compilation);
        var registry = new InaccessibleAccessorRegistry();

        var getterFirst = registry.GetOrAddPropertyGetter(sourceType, sourceProperty, context);
        var getterSecond = registry.GetOrAddPropertyGetter(sourceType, sourceProperty, context);
        getterSecond.Should().Be(getterFirst);

        var setterFirst = registry.GetOrAddPropertySetter(targetType, targetProperty, context);
        var setterSecond = registry.GetOrAddPropertySetter(targetType, targetProperty, context);
        setterSecond.Should().Be(setterFirst);

        var ctorFirst = registry.GetOrAddConstructor(constructor, context);
        var ctorSecond = registry.GetOrAddConstructor(constructor, context);
        ctorSecond.Should().Be(ctorFirst);

        registry.Accessors.Should().HaveCount(3);
        registry.BuildSource().Should().NotBeNullOrWhiteSpace();
        new InaccessibleAccessorRegistry().BuildSource().Should().BeEmpty();

        var getterKey = new InaccessibleAccessorRegistry.AccessorKey(
            sourceType,
            InaccessibleAccessorUnsafeKind.Method,
            "get_Value");
        var getterKeyCopy = new InaccessibleAccessorRegistry.AccessorKey(
            sourceType,
            InaccessibleAccessorUnsafeKind.Method,
            "get_Value");
        var otherKey = new InaccessibleAccessorRegistry.AccessorKey(
            sourceType,
            InaccessibleAccessorUnsafeKind.Method,
            "get_Other");

        getterKey.Equals(getterKeyCopy).Should().BeTrue();
        getterKey.Equals(otherKey).Should().BeFalse();
        getterKey.Equals((object)getterKeyCopy).Should().BeTrue();
        getterKey.Equals((object)"not-a-key").Should().BeFalse();
        getterKey.GetHashCode().Should().Be(getterKeyCopy.GetHashCode());
    }

    /// <summary>
    /// Test <see cref="CompilationExtensions.IsUnsafeAccessorSupported"/> returns <c>false</c>
    /// when the attribute type is missing from the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsUnsafeAccessorSupportedReturnsFalseWhenAttributeTypeIsMissing()
    {
        var compilation = CSharpCompilation.Create("NoUnsafeAccessorMetadata");

        compilation.IsUnsafeAccessorSupported().Should().BeFalse();
    }
}