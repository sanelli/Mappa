// <copyright file="MappaAssignFromConstantAttributeIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
// TODO [#53] An error is emitted when multiple attributes target the same property.
// TODO [#53] An error is emitted when multiple attributes target the same constructor parameter.
// TODO [#53] Can assign an enumeration to a value for property.
// TODO [#53] Can assign an enumeration to a value for ctor parameter.
// TODO [#53] Can assign an array of numeric values for property.
// TODO [#53] Can assign an array of numeric values for ctor parameter.
// TODO [#53] Can assign an array of strings for property.
// TODO [#53] Can assign an array of strings for ctor parameter.
// TODO [#53] Can assign an array of char values for property.
// TODO [#53] Can assign an array of char values for ctor parameter.
// TODO [#53] Can assign an array of boolean values for property.
// TODO [#53] Can assign an array of boolean values for ctor parameter.
// TODO [#53] Can assign an array of types values for property.
// TODO [#53] Can assign an array of types values for ctor parameter.
// TODO [#53] Can assign an array of enumerations values for property.
// TODO [#53] Can assign an array of enumerations values for ctor parameter.
// TODO [#53] Can assign an array of objects values for property.
// TODO [#53] Can assign an array of objects values for ctor parameter.
public sealed class MappaAssignFromConstantAttributeIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a property can be set using <see cref="MappaAssignFromConstantAttribute"/> on property.
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
    public async Task CanAssignPropertyValueViaMappaAssignFromConstantAttribute(
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
    /// Test a property can be set using <see cref="MappaAssignFromConstantAttribute"/>
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
    public async Task CanAssignConstructorParameterValueViaMappaAssignFromConstantAttribute(
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
                                        parameterAssertion => parameterAssertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }
}