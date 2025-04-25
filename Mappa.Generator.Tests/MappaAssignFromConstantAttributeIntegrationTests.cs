// <copyright file="MappaAssignFromConstantAttributeIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
// TODO [#53] An error is emitted when multiple attributes target the same property.
public sealed class MappaAssignFromConstantAttributeIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a scalar value can be set using <see cref="MappaAssignFromConstantAttribute"/> on property.
    /// </summary>
    /// <param name="type">The type being investigated.</param>
    /// <param name="attributeValue">The value to be stored in the attribute.</param>
    /// <param name="expectedValue">The value expected when asserting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("string", "\"This is the value\"", "This is the value")]
    [InlineData("byte", "17", 17)]
    [InlineData("sbyte", "17", 17)]
    [InlineData("ushort", "17", 17)]
    [InlineData("short", "17", 17)]
    [InlineData("uint", "17", 17)]
    [InlineData("int", "17", 17)]
    [InlineData("ulong", "17", 17)]
    [InlineData("long", "17", 17)]
    [InlineData("float", "17.00f", 17.00f)]
    [InlineData("double", "17.00", 17.00)]
    [InlineData("char", "'X'", 'X')]
    [InlineData("bool", "true", true)]
    [InlineData("bool", "false", false)]
    [InlineData("string?", "null", null)]
    [InlineData("System.Type", "typeof(string)", typeof(string))]
    [InlineData("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", "MyEnum.Two", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two")]
    public async Task CanAssignScalarValueViaMappaAssignFromConstantAttributeToProperty(
        string type,
        string attributeValue,
        object? expectedValue)
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public enum MyEnum
                           {
                             One,
                             Two,
                           }

                           public class Source { }
                           public class Target {
                              public {{type}} Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), {{attributeValue}})]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                type,
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    if (type.Equals("System.Type", StringComparison.Ordinal)
                                        && expectedValue is Type expectedValueAsType)
                                    {
                                        initializationAssertions.BeTypeOfExpressionSyntax(expectedValueAsType.ToString());
                                    }
                                    else if (type.Equals("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", StringComparison.Ordinal) && expectedValue is string s)
                                    {
                                        initializationAssertions.BeMemberAccessExpressionSyntax(s);
                                    }
                                    else
                                    {
                                        initializationAssertions.BeLiteralExpressionSyntax(expectedValue);
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a scalar value can be set using <see cref="MappaAssignFromConstantAttribute"/>
    /// on constructor parameter.
    /// </summary>
    /// <param name="type">The type being investigated.</param>
    /// <param name="attributeValue">The value to be stored in the attribute.</param>
    /// <param name="expectedValue">The value expected when asserting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("string", "\"This is the value\"", "This is the value")]
    [InlineData("byte", "17", 17)]
    [InlineData("sbyte", "17", 17)]
    [InlineData("ushort", "17", 17)]
    [InlineData("short", "17", 17)]
    [InlineData("uint", "17", 17)]
    [InlineData("int", "17", 17)]
    [InlineData("ulong", "17", 17)]
    [InlineData("long", "17", 17)]
    [InlineData("float", "17.00f", 17.00f)]
    [InlineData("double", "17.00", 17.00)]
    [InlineData("char", "'X'", 'X')]
    [InlineData("bool", "true", true)]
    [InlineData("bool", "false", false)]
    [InlineData("string?", "null", null)]
    [InlineData("System.Type", "typeof(string)", typeof(string))]
    [InlineData("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", "MyEnum.Two", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two")]
    public async Task CanAssignScalarValueViaMappaAssignFromConstantAttributeToContructorParameter(
        string type,
        string attributeValue,
        object? expectedValue)
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public enum MyEnum
                           {
                             One,
                             Two,
                           }

                           public class Source { }
                           public class Target {
                              public Target({{type}} value) { }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant("value", {{attributeValue}})]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                type,
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    if (type.Equals("System.Type", StringComparison.Ordinal)
                                        && expectedValue is Type expectedValueAsType)
                                    {
                                        initializationAssertions.BeTypeOfExpressionSyntax(expectedValueAsType.ToString());
                                    }
                                    else if (type.Equals("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", StringComparison.Ordinal) && expectedValue is string s)
                                    {
                                        initializationAssertions.BeMemberAccessExpressionSyntax(s);
                                    }
                                    else
                                    {
                                        initializationAssertions.BeLiteralExpressionSyntax(expectedValue);
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a scalar value can be set using <see cref="MappaAssignFromConstantAttribute"/> on property.
    /// </summary>
    /// <param name="type">The type being investigated.</param>
    /// <param name="attributeValue">The value to be stored in the attribute.</param>
    /// <param name="expectedValues">The value expected when asserting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("sbyte", "1, 2, 3", new sbyte[] { 1, 2, 3 })]
    [InlineData("short", "1, 2, 3", new short[] { 1, 2, 3 })]
    [InlineData("int", "1, 2, 3", new[] { 1, 2, 3 })]
    [InlineData("long", "1, 2, 3", new long[] { 1, 2, 3 })]
    [InlineData("byte", "1, 2, 3", new byte[] { 1, 2, 3 })]
    [InlineData("ushort", "1, 2, 3", new ushort[] { 1, 2, 3 })]
    [InlineData("uint", "1, 2, 3", new uint[] { 1, 2, 3 })]
    [InlineData("ulong", "1, 2, 3", new ulong[] { 1, 2, 3 })]
    [InlineData("float", "1, 2, 3", new[] { 1.00f, 2.00f, 3.00f })]
    [InlineData("double", "1, 2, 3", new[] { 1.00, 2.00, 3.00 })]
    [InlineData("string", "\"hello\", \"world\", \"!\"", new[] { "hello", "world", "!" })]
    [InlineData("string?", "\"hello\", null, \"!\"", new[] { "hello", null, "!" })]
    [InlineData("char", "'a', 'b', 'c'", new[] { 'a', 'b', 'c' })]
    [InlineData("bool", "true, false, true", new[] { true, false, true })]
    [InlineData("System.Type", "typeof(string), typeof(int), typeof(float)", new[] { typeof(string), typeof(int), typeof(float) })]
    [InlineData("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", "MyEnum.Two, MyEnum.One, MyEnum.Two", new[] { "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.One", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two" })]
    public async Task CanAssignArrayValueViaMappaAssignFromConstantAttributeToProperty(
        string type,
        string attributeValue,
        ICollection expectedValues)
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public enum MyEnum
                           {
                             One,
                             Two,
                           }

                           public class Source { }
                           public class Target {
                              public {{type}}[] Property { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant(nameof(Target.Property), new {{type}}[] { {{attributeValue}} })]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                $"{type}[]",
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    List<object> arrayOfValues = new List<object>();
                                    foreach (object o in expectedValues)
                                    {
                                        arrayOfValues.Add(o);
                                    }

                                    initializationAssertions.BeArrayCreationExpressionSyntax(
                                        type,
                                        sizeAssertions => sizeAssertions.BeOmittedSizeExpressionSyntax(),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[0]),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[1]),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[2]));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a scalar value can be set using <see cref="MappaAssignFromConstantAttribute"/>
    /// on constructor parameter.
    /// </summary>
    /// <param name="type">The type being investigated.</param>
    /// <param name="attributeValue">The value to be stored in the attribute.</param>
    /// <param name="expectedValues">The value expected when asserting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("sbyte", "1, 2, 3", new sbyte[] { 1, 2, 3 })]
    [InlineData("short", "1, 2, 3", new short[] { 1, 2, 3 })]
    [InlineData("int", "1, 2, 3", new[] { 1, 2, 3 })]
    [InlineData("long", "1, 2, 3", new long[] { 1, 2, 3 })]
    [InlineData("byte", "1, 2, 3", new byte[] { 1, 2, 3 })]
    [InlineData("ushort", "1, 2, 3", new ushort[] { 1, 2, 3 })]
    [InlineData("uint", "1, 2, 3", new uint[] { 1, 2, 3 })]
    [InlineData("ulong", "1, 2, 3", new ulong[] { 1, 2, 3 })]
    [InlineData("float", "1, 2, 3", new[] { 1.00f, 2.00f, 3.00f })]
    [InlineData("double", "1, 2, 3", new[] { 1.00, 2.00, 3.00 })]
    [InlineData("string", "\"hello\", \"world\", \"!\"", new[] { "hello", "world", "!" })]
    [InlineData("string?", "\"hello\", null, \"!\"", new[] { "hello", null, "!" })]
    [InlineData("char", "'a', 'b', 'c'", new[] { 'a', 'b', 'c' })]
    [InlineData("bool", "true, false, true", new[] { true, false, true })]
    [InlineData("System.Type", "typeof(string), typeof(int), typeof(float)", new[] { typeof(string), typeof(int), typeof(float) })]
    [InlineData("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", "MyEnum.Two, MyEnum.One, MyEnum.Two", new[] { "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.One", "Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum.Two" })]
    public async Task CanAssignArrayValueViaMappaAssignFromConstantAttributeToConstructorParameter(
        string type,
        string attributeValue,
        ICollection expectedValues)
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public enum MyEnum
                           {
                             One,
                             Two,
                           }

                           public class Source { }
                           public class Target {
                                public Target({{type}}[] value) { }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaAssignFromConstant("value", new {{type}}[] { {{attributeValue}} })]
                               public partial Target Map(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                $"{type}[]",
                                "__mappa_tmp_1",
                                initializationAssertions =>
                                {
                                    List<object> arrayOfValues = new List<object>();
                                    foreach (object o in expectedValues)
                                    {
                                        arrayOfValues.Add(o);
                                    }

                                    initializationAssertions.BeArrayCreationExpressionSyntax(
                                        type,
                                        sizeAssertions => sizeAssertions.BeOmittedSizeExpressionSyntax(),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[0]),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[1]),
                                        arrayValueAssertions => AssertInitArrayValue(arrayValueAssertions, type, arrayOfValues[2]));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    private static void AssertInitArrayValue(ExpressionSyntaxAssertions initializationAssertions, string type, object? value)
    {
        if (type.Equals("System.Type", StringComparison.Ordinal)
            && value is Type expectedValueAsType)
        {
            initializationAssertions.BeTypeOfExpressionSyntax(expectedValueAsType.ToString());
        }
        else if (type.Equals("Mappa.Generator.Tests.UnitTests.SourceCode.MyEnum", StringComparison.Ordinal) && value is string s)
        {
            initializationAssertions.BeMemberAccessExpressionSyntax(s);
        }
        else
        {
            initializationAssertions.BeLiteralExpressionSyntax(value);
        }
    }
}