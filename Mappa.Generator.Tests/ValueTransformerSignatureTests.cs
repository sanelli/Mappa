// <copyright file="ValueTransformerSignatureTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ValueTransformerSignature"/>.
/// </summary>
public sealed class ValueTransformerSignatureTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Namespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.TryValidate"/> accepts well-formed transformer signatures.
    /// </summary>
    /// <param name="methodDeclaration">The transformer method declaration.</param>
    [Theory]
    [UnitTest]
    [InlineData("string Transform(string value) => value;")]
    [InlineData("string Transform(in string value) => value;")]
    [InlineData("string Transform(string value, MappaContext context) => value;")]
    [InlineData("string Transform(in string value, in MappaContext context) => value;")]
    [InlineData("string? Transform(string? value) => value;")]
    [InlineData("T Transform<T>(T value) => value;")]
    [InlineData("T[] Transform<T>(T[] value) => value;")]
    [InlineData("List<T> Transform<T>(List<T> value) => value;")]
    [InlineData("KeyValuePair<TKey, TValue> Transform<TKey, TValue>(KeyValuePair<TKey, TValue> value) => value;")]
    public void TryValidateAcceptsValidSignatures(string methodDeclaration)
    {
        var (method, compilation, _) = BuildTransformer(methodDeclaration);

        var isValid = ValueTransformerSignature.TryValidate(method, compilation, out var reason);

        isValid.Should().BeTrue();
        reason.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.TryValidate"/> rejects malformed transformer signatures with a reason.
    /// </summary>
    /// <param name="methodDeclaration">The transformer method declaration.</param>
    /// <param name="expectedReason">The expected reason.</param>
    [Theory]
    [UnitTest]
    [InlineData("async Task<string> Transform(Task<string> value) => await value;", "asynchronous transformers are not supported")]
    [InlineData("Task Transform(Task value) => value;", "asynchronous transformers are not supported")]
    [InlineData("void Transform(string value) { }", "it must return a value")]
    [InlineData("ref string Transform(string value) => ref stored;", "it must not return by reference")]
    [InlineData("ref readonly string Transform(string value) => ref stored;", "it must not return by reference")]
    [InlineData("string Transform() => string.Empty;", "it must have one parameter, or one parameter and a MappaContext")]
    [InlineData("string Transform(string value, MappaContext context, int extra) => value;", "it must have one parameter, or one parameter and a MappaContext")]
    [InlineData("string Transform(ref string value) => value;", "the value parameter must not be 'ref' or 'out'")]
    [InlineData("string Transform(out string value) { value = string.Empty; return value; }", "the value parameter must not be 'ref' or 'out'")]
    [InlineData("string Transform(string value, int other) => value;", "the second parameter must be a MappaContext passed by value")]
    [InlineData("string Transform(string value, ref MappaContext context) => value;", "the second parameter must be a MappaContext passed by value")]
    [InlineData("string Transform(string value, out MappaContext context) { context = null; return value; }", "the second parameter must be a MappaContext passed by value")]
    [InlineData("string Transform(long value) => string.Empty;", "the return type must be the same as the value parameter type, nullability included")]
    [InlineData("string? Transform(string value) => value;", "the return type must be the same as the value parameter type, nullability included")]
    [InlineData("string Transform<T>(string value) => value;", "every generic type parameter must be inferable from the value parameter")]
    [InlineData("T Transform<T, U>(T value) => value;", "every generic type parameter must be inferable from the value parameter")]
    public void TryValidateRejectsInvalidSignatures(string methodDeclaration, string expectedReason)
    {
        var (method, compilation, _) = BuildTransformer(methodDeclaration);

        var isValid = ValueTransformerSignature.TryValidate(method, compilation, out var reason);

        isValid.Should().BeFalse();
        reason.Should().Be(expectedReason);
    }

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.TryValidate"/> rejects methods that are not ordinary methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryValidateRejectsOperators()
    {
        var (method, compilation, _) = Build(
            "public static Holder operator +(Holder value) => value;",
            "op_UnaryPlus",
            "enable",
            "int");

        var isValid = ValueTransformerSignature.TryValidate(method, compilation, out var reason);

        isValid.Should().BeFalse();
        reason.Should().Be("it is not an ordinary method");
    }

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.IsMatch"/> binds the type parameters of open generic transformers
    /// and verifies their generic constraints and nullable annotations.
    /// </summary>
    /// <param name="methodDeclaration">The transformer method declaration.</param>
    /// <param name="siteType">The type of the mapping site.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [UnitTest]
    [InlineData("string Transform(string value) => value;", "string", true)]
    [InlineData("string Transform(string value) => value;", "int", false)]
    [InlineData("T Transform<T>(T value) => value;", "int", true)]
    [InlineData("T Transform<T>(T value) => value;", "string", true)]
    [InlineData("T Transform<T>(T value) => value;", "List<int>", true)]
    [InlineData("List<T> Transform<T>(List<T> value) => value;", "List<int>", true)]
    [InlineData("List<T> Transform<T>(List<T> value) => value;", "HashSet<int>", false)]
    [InlineData("List<T> Transform<T>(List<T> value) => value;", "int[]", false)]
    [InlineData("List<T> Transform<T>(List<T> value) => value;", "string", false)]
    [InlineData("T[] Transform<T>(T[] value) => value;", "int[]", true)]
    [InlineData("T[] Transform<T>(T[] value) => value;", "List<int>[]", true)]
    [InlineData("T[] Transform<T>(T[] value) => value;", "int", false)]
    [InlineData("T[] Transform<T>(T[] value) => value;", "int[,]", false)]
    [InlineData("List<T>? Transform<T>(List<T>? value) => value;", "List<int>?", true)]
    [InlineData("List<T>? Transform<T>(List<T>? value) => value;", "List<int>", false)]
    [InlineData("T[]? Transform<T>(T[]? value) => value;", "int[]?", true)]
    [InlineData("T[]? Transform<T>(T[]? value) => value;", "int[]", false)]
    [InlineData("KeyValuePair<T, T> Transform<T>(KeyValuePair<T, T> value) => value;", "KeyValuePair<int, int>", true)]
    [InlineData("KeyValuePair<T, T> Transform<T>(KeyValuePair<T, T> value) => value;", "KeyValuePair<int, string>", false)]
    [InlineData("Dictionary<TKey, TValue> Transform<TKey, TValue>(Dictionary<TKey, TValue> value) => value;", "Dictionary<int, string>", true)]
    [InlineData("string Transform(string value) => value;", "string[]", false)]
    [InlineData("KeyValuePair<string, T> Transform<T>(KeyValuePair<string, T> value) => value;", "KeyValuePair<string, int>", true)]
    [InlineData("KeyValuePair<string, T> Transform<T>(KeyValuePair<string, T> value) => value;", "KeyValuePair<int, int>", false)]
    [InlineData("T Transform<T>(T value) where T : class => value;", "string", true)]
    [InlineData("T Transform<T>(T value) where T : class => value;", "int", false)]
    [InlineData("T Transform<T>(T value) where T : struct => value;", "int", true)]
    [InlineData("T Transform<T>(T value) where T : struct => value;", "string", false)]
    [InlineData("T Transform<T>(T value) where T : struct => value;", "int?", false)]
    [InlineData("T Transform<T>(T value) where T : unmanaged => value;", "Plain", true)]
    [InlineData("T Transform<T>(T value) where T : unmanaged => value;", "WithReference", false)]
    [InlineData("T Transform<T>(T value) where T : new() => value;", "Marked", true)]
    [InlineData("T Transform<T>(T value) where T : new() => value;", "NoPublicCtor", false)]
    [InlineData("T Transform<T>(T value) where T : new() => value;", "AbstractType", false)]
    [InlineData("T Transform<T>(T value) where T : new() => value;", "int", true)]
    [InlineData("T Transform<T>(T value) where T : new() => value;", "Plain", true)]
    [InlineData("T Transform<T>(T value) where T : IMarker => value;", "Marked", true)]
    [InlineData("T Transform<T>(T value) where T : IMarker => value;", "string", false)]
    [InlineData("T Transform<T>(T value) where T : class, IMarker, new() => value;", "Marked", true)]
    [InlineData("T Transform<T>(T value) where T : class, IMarker, new() => value;", "NoPublicCtor", false)]
    [InlineData("KeyValuePair<T, U> Transform<T, U>(KeyValuePair<T, U> value) where T : IEquatable<U> => value;", "KeyValuePair<int, int>", false)]
    [InlineData("KeyValuePair<T, U> Transform<T, U>(KeyValuePair<T, U> value) where T : class where U : struct => value;", "KeyValuePair<string, int>", true)]
    [InlineData("KeyValuePair<T, U> Transform<T, U>(KeyValuePair<T, U> value) where T : class where U : struct => value;", "KeyValuePair<int, int>", false)]
    [InlineData("T? Transform<T>(T? value) where T : class => value;", "string?", true)]
    [InlineData("T? Transform<T>(T? value) where T : class => value;", "string", false)]
    [InlineData("T? Transform<T>(T? value) => value;", "string?", true)]
    [InlineData("T? Transform<T>(T? value) => value;", "string", false)]
    [InlineData("T? Transform<T>(T? value) => value;", "int", true)]
    [InlineData("KeyValuePair<T?, T> Transform<T>(KeyValuePair<T?, T> value) where T : class => value;", "KeyValuePair<string?, string>", true)]
    [InlineData("KeyValuePair<T?, T> Transform<T>(KeyValuePair<T?, T> value) where T : class => value;", "KeyValuePair<string?, Marked>", false)]
    [InlineData("KeyValuePair<T?, T> Transform<T>(KeyValuePair<T?, T> value) where T : class => value;", "KeyValuePair<string, string>", false)]
    public void IsMatchBindsAndVerifiesOpenGenericTransformers(string methodDeclaration, string siteType, bool expected)
    {
        var (method, compilation, site) = BuildTransformer(methodDeclaration, siteType);

        ValueTransformerSignature.IsMatch(new ValueTransformer(method, null, null, null, null), site, true, compilation)
            .Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.IsMatch"/> ignores annotations when nullable is not enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsMatchIgnoresAnnotationsWhenNullableIsNotEnabled()
    {
        var (method, compilation, site) = BuildTransformer("T[]? Transform<T>(T[]? value) => value;", "int[]");
        var transformer = new ValueTransformer(method, null, null, null, null);

        ValueTransformerSignature.IsMatch(transformer, site, false, compilation).Should().BeTrue();
        ValueTransformerSignature.IsMatch(transformer, site, true, compilation).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="ValueTransformerSignature.IsMatch"/> matches oblivious annotations on either side.
    /// </summary>
    /// <param name="transformerContext">The nullable context of the transformer.</param>
    /// <param name="siteContext">The nullable context of the site.</param>
    [Theory]
    [UnitTest]
    [InlineData("disable", "enable")]
    [InlineData("enable", "disable")]
    public void IsMatchMatchesObliviousAnnotations(string transformerContext, string siteContext)
    {
        var (method, compilation, site) = BuildTransformer(
            "T[] Transform<T>(T[] value) => value;",
            "int[]",
            transformerContext,
            siteContext);

        ValueTransformerSignature.IsMatch(new ValueTransformer(method, null, null, null, null), site, true, compilation)
            .Should().BeTrue();
    }

    private static (IMethodSymbol Method, CSharpCompilation Compilation, ITypeSymbol SiteType) BuildTransformer(
        string methodDeclaration,
        string siteType = "int",
        string transformerContext = "enable",
        string siteContext = "enable")
        => Build($"public static {methodDeclaration}", "Transform", transformerContext, siteType, siteContext);

    private static (IMethodSymbol Method, CSharpCompilation Compilation, ITypeSymbol SiteType) Build(
        string declaration,
        string methodName,
        string transformerContext,
        string siteType,
        string siteContext = "enable")
    {
        var source = $$"""
                       #nullable enable
                       using System;
                       using System.Collections.Generic;
                       using System.Threading.Tasks;
                       using Mappa;
                       using Mappa.Attributes;

                       namespace {{Namespace}};

                       public interface IMarker { }

                       public class Marked : IMarker { }

                       public class NoPublicCtor { public NoPublicCtor(int value) { } }

                       public abstract class AbstractType { }

                       public struct Plain { public int Value; }

                       public struct WithReference { public string Value; }

                       #nullable {{transformerContext}}
                       public class Holder
                       {
                           private static string stored = string.Empty;

                           [MappaTransformer]
                           {{declaration}}
                       }

                       #nullable {{siteContext}}
                       public static class Site
                       {
                           public static {{siteType}} Value = default!;
                       }
                       """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName($"{Namespace}.Holder");
        var site = compilation.GetTypeByMetadataName($"{Namespace}.Site");
        if (holder is null || site is null)
        {
            throw new InvalidOperationException("The test types were not found in the compilation.");
        }

        var method = holder.GetMembers(methodName).OfType<IMethodSymbol>().Single();
        var siteField = site.GetMembers("Value").OfType<IFieldSymbol>().Single();
        return (method, compilation, siteField.Type);
    }
}