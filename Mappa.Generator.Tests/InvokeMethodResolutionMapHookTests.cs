// <copyright file="InvokeMethodResolutionMapHookTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for map-hook invoke-method resolution.
/// </summary>
public sealed class InvokeMethodResolutionMapHookTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Test supported signature tiers select the expected candidate.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="providesContext">Whether the map provides context.</param>
    /// <param name="expectedParameterCount">The expected selected parameter count.</param>
    [Theory]
    [InlineData("RefContext", true, 2)]
    [InlineData("RefOnly", true, 1)]
    [InlineData("RefOnly", false, 1)]
    [InlineData("ContextOnly", true, 1)]
    [InlineData("Parameterless", true, 0)]
    [InlineData("Parameterless", false, 0)]
    [UnitTest]
    public void SupportedSignatureTierSelectsExpectedCandidate(
        string methodName,
        bool providesContext,
        int expectedParameterCount)
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");
        var mappedValueType = compilation.GetSpecialType(SpecialType.System_Int32);

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, methodName),
            methodName,
            mappedValueType,
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            providesContext,
            out var method,
            out var ambiguityDetails);

        result.Should().Be(InvokeMethodResolutionResult.Success);
        method.Should().NotBeNull();
        method?.Parameters.Should().HaveCount(expectedParameterCount);
        ambiguityDetails.Should().BeEmpty();
    }

    /// <summary>
    /// Test same-name overloads select the highest supported tier.
    /// </summary>
    /// <param name="providesContext">Whether the map provides context.</param>
    /// <param name="expectedParameterCount">The selected parameter count.</param>
    [Theory]
    [InlineData(true, 2)]
    [InlineData(false, 1)]
    [UnitTest]
    public void SameNameOverloadsSelectHighestSupportedTier(
        bool providesContext,
        int expectedParameterCount)
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, "Priority"),
            "Priority",
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            providesContext,
            out var method,
            out _);

        result.Should().Be(InvokeMethodResolutionResult.Success);
        method.Should().NotBeNull();
        if (method is null)
        {
            throw new InvalidOperationException("Expected a hook method to be selected.");
        }

        method.Parameters.Should().HaveCount(expectedParameterCount);
        method.Parameters[0].RefKind.Should().Be(RefKind.Ref);
    }

    /// <summary>
    /// Test a context-only hook cannot be selected without a map context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ContextOnlyHookIsNotSelectedWithoutContext()
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, "ContextOnly"),
            "ContextOnly",
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            mapMethodProvidesContext: false,
            out var method,
            out var ambiguityDetails);

        result.Should().Be(InvokeMethodResolutionResult.NotFound);
        method.Should().BeNull();
        ambiguityDetails.Should().BeEmpty();
    }

    /// <summary>
    /// Test invalid signatures are rejected by resolution.
    /// </summary>
    /// <param name="methodName">The invalid method name.</param>
    [Theory]
    [InlineData("NonVoid")]
    [InlineData("MissingRef")]
    [InlineData("WrongType")]
    [InlineData("WrongCount")]
    [InlineData("WrongOrder")]
    [UnitTest]
    public void InvalidSignatureIsRejected(string methodName)
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, methodName),
            methodName,
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            mapMethodProvidesContext: true,
            out var method,
            out var ambiguityDetails);

        result.Should().Be(InvokeMethodResolutionResult.NotFound);
        method.Should().BeNull();
        ambiguityDetails.Should().BeEmpty();
    }

    /// <summary>
    /// Test static requirements filter otherwise valid hook candidates.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="requiresStatic">Whether the candidate must be static.</param>
    /// <param name="expectedSuccess">Whether resolution should succeed.</param>
    [Theory]
    [InlineData("StaticHook", true, true)]
    [InlineData("StaticHook", false, false)]
    [InlineData("InstanceHook", false, true)]
    [InlineData("InstanceHook", true, false)]
    [UnitTest]
    public void StaticRequirementFiltersCandidates(
        string methodName,
        bool requiresStatic,
        bool expectedSuccess)
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");
        var staticRequirement = requiresStatic
            ? InvokeMethodStaticRequirement.Static
            : InvokeMethodStaticRequirement.NotStatic;
        var expectedResult = expectedSuccess
            ? InvokeMethodResolutionResult.Success
            : InvokeMethodResolutionResult.NotFound;

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, methodName),
            methodName,
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            staticRequirement,
            mapMethodProvidesContext: false,
            out _,
            out _);

        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// Test inaccessible hook candidates are rejected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InaccessibleHookIsRejected()
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, "Inaccessible"),
            "Inaccessible",
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            mapMethodProvidesContext: false,
            out var method,
            out _);

        result.Should().Be(InvokeMethodResolutionResult.NotFound);
        method.Should().BeNull();
    }

    /// <summary>
    /// Test multiple candidates in a winning tier are reported as ambiguous.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MultipleWinningTierCandidatesAreAmbiguous()
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, "Ambiguous"),
            "Ambiguous",
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            mapMethodProvidesContext: false,
            out var method,
            out var ambiguityDetails);

        result.Should().Be(InvokeMethodResolutionResult.Ambiguous);
        method.Should().BeNull();
        ambiguityDetails.Should().Contain("multiple methods named 'Ambiguous'");
    }

    /// <summary>
    /// Test the most-derived candidate wins within a signature tier.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MostDerivedCandidateWinsWithinTier()
    {
        var compilation = BuildHookCompilation();
        var mapClass = GetRequiredType(compilation, "Mapper");
        var hookType = GetRequiredType(compilation, "Hooks");

        var result = InvokeMethodResolution.TryResolveMapHook(
            compilation,
            mapClass,
            hookType,
            GetMethods(hookType, "Inherited"),
            "Inherited",
            compilation.GetSpecialType(SpecialType.System_Int32),
            nullableEnabled: true,
            InvokeMethodStaticRequirement.StaticOrNotStatic,
            mapMethodProvidesContext: false,
            out var method,
            out _);

        result.Should().Be(InvokeMethodResolutionResult.Success);
        method.Should().NotBeNull();
        method?.ContainingType.Should().Be(hookType);
    }

    private static CSharpCompilation BuildHookCompilation()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Mapper
                                  {
                                  }

                                  public class BaseHooks
                                  {
                                      public void Inherited(ref int input) { }
                                  }

                                  public sealed class Hooks : BaseHooks
                                  {
                                      public void RefContext(ref int input, MappaContext context) { }
                                      public void RefOnly(ref int input) { }
                                      public void ContextOnly(MappaContext context) { }
                                      public void Parameterless() { }
                                      public void Priority(ref int input, MappaContext context) { }
                                      public void Priority(ref int input) { }
                                      public void Priority(MappaContext context) { }
                                      public void Priority() { }
                                      public int NonVoid() => 0;
                                      public void MissingRef(int input) { }
                                      public void WrongType(ref long input) { }
                                      public void WrongCount(ref int input, MappaContext context, int extra) { }
                                      public void WrongOrder(MappaContext context, ref int input) { }
                                      public static void StaticHook() { }
                                      public void InstanceHook() { }
                                      private void Inaccessible() { }
                                      public void Ambiguous(ref int input) { }
                                      public void Ambiguous<T>(ref int input) { }
                                      public new void Inherited(ref int input) { }
                                  }
                                  """;

        return BuildCompilation(sourceCode);
    }

    private static INamedTypeSymbol GetRequiredType(CSharpCompilation compilation, string typeName)
        => compilation.GetTypeByMetadataName($"{SourceNamespace}.{typeName}")
           ?? throw new InvalidOperationException($"Cannot locate type '{typeName}'.");

    private static IMethodSymbol[] GetMethods(INamedTypeSymbol type, string methodName)
        => type.LocateMethods(methodName);
}