// <copyright file="PolymorphicMethodMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="PolymorphicMethodMapStrategy"/>.
/// </summary>
// TODO [#49] Add tests to check we can pick up polymorphic method in a dependency class mapped with Mappa.
// TODO [#49] Add tests to check we can pick up polymorphic method in a dependency class NOT mapped with Mappa.
// TODO [#49] Add tests that we can pick up a user defined non-partial method tagged with the MappaTypeMapping attribute.
// TODO [#49] Add tests that we cna pick up a user defined non-partial method tagged with the MappaTypeMapping attribute in a dependency.
public sealed class PolymorphicMethodMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }

                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                            "__mappa_tmp_8",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingAttribute"/>.
    /// Nullability is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingAttributeWithNullabilityDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }

                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.None,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.None,
                2,
                NullableSetup.Disable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_11"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeIfStatementSyntax(
                            conditionAssertions => conditionAssertions.BeIsPatternExpressionSyntax(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                patternAssertions => patternAssertions.BeUnaryPatternSyntax(
                                    SyntaxKind.NotKeyword,
                                    expressionAssertions => expressionAssertions.BeConstantPatternSyntax(null))),
                            thenBlockStatementAssertions =>
                            {
                                thenBlockStatementAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                        "__mappa_tmp_12",
                                        initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("input")))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                                        "__mappa_tmp_13",
                                        initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_12.DependencyProperty")))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                        "__mappa_tmp_14",
                                        initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                            castExpression => castExpression.BeInvocationExpressionSyntax(
                                                "this.MapDependency",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_13")))))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        "__mappa_tmp_15",
                                        initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                            ("DependencyProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_14")))))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeAssignmentExpressionStatement(
                                        "__mappa_tmp_11",
                                        initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_15")));
                            },
                            elseBlockStatementAssertions =>
                            {
                                elseBlockStatementAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(elseStatementAssertions => elseStatementAssertions.BeAssignmentExpressionStatement(
                                        "__mappa_tmp_11",
                                        initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                            castExpressionAssertions => castExpressionAssertions.BeLiteralExpressionSyntax(null))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingAttribute"/>.
    /// The invoked method require a context parameter which is provided.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingAttributeAndBothMethodsHaveAContextParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using Mappa;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }

                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input, MappaContext context);
                                      
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
                2,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                            "__mappa_tmp_8",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7"),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("context")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")));
                });
    }

    /// <summary>
    /// Test a mapping will not use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingAttribute"/>
    /// because require a context parameter but the invoker does not
    /// have one.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithoutUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingAttributeAndMapDependencyRequireAContextParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;
                                  using Mappa;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }

                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input, MappaContext context);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(7)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(Guid).ToString(),
                            "__mappa_tmp_8",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_7.FirstProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_8.ToString")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_7.BaseProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                ("FirstProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")),
                                ("BaseProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_12",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_12")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingAttribute"/>.
    /// The invoked method does not require a context parameter but the invoker
    /// has a context parameter which is not being used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingAttributeAndOnlyMapMethodHaveAContextParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using Mappa;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }

                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
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
                2,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                            "__mappa_tmp_8",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Undefined"/> (and therefore it is not applied).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeUndefined()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.BaseProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                ("BaseProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_12",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_12")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Enable"/> but nullability do not match
    /// and therefore the method is not applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeEnabledButNullabilityDoNotMatch()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase? MapDependency(SourceBase? input);
                                      
                                      [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_13",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_14",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_13.BaseProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_15",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                ("BaseProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_14")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_16",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_15")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_16")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Enable"/> on method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeEnabledOnMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Enable"/> on method.
    /// Nullability is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeEnabledOnMethodAndNullabilityIsDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.None,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.None,
                2,
                NullableSetup.Disable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_15"))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeIfStatementSyntax(
                            conditionAssertions => conditionAssertions.BeIsPatternExpressionSyntax(
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                patternAssertions => patternAssertions.BeUnaryPatternSyntax(
                                    SyntaxKind.NotKeyword, notAssertions => notAssertions.BeConstantPatternSyntax(null))),
                            thenBlockAssertions =>
                            {
                                thenBlockAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(5)
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                        "__mappa_tmp_16",
                                        initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("input")))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                                        "__mappa_tmp_17",
                                        initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_16.DependencyProperty")))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                        "__mappa_tmp_18",
                                        initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                            expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                                "this.MapDependency",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_17")))))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeLocalDeclarationStatementSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        "__mappa_tmp_19",
                                        initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                            ("DependencyProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_18")))))
                                    .HasNextSyntaxNode(thenStatementAssertions => thenStatementAssertions.BeAssignmentExpressionStatement(
                                        "__mappa_tmp_15",
                                        rightSideExpression => rightSideExpression.BeIdentifierNameSyntax("__mappa_tmp_19")));
                            },
                            elseBlockAssertions =>
                            {
                                elseBlockAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(elseStatementAssertions => elseStatementAssertions.BeAssignmentExpressionStatement(
                                        "__mappa_tmp_15",
                                        rightExpressionAssertions => rightExpressionAssertions.BeCastExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                            castExpressionAssertions => castExpressionAssertions.BeLiteralExpressionSyntax(null))));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_15")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Enable"/> on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeEnabledOnClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping and
    /// <see cref="MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute"/> is
    /// <see cref="BooleanSetting.Disable"/> on method but <see cref="BooleanSetting.Enable"/> on
    /// method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyAndPolymorphicMapMethodWithMatchingDefaultAttributeDisabledOnClassEnabledOnClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  public class TargetDefault : TargetBase {  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetDefault DependencyProperty { get; set; } }

                                  [Mappa]
                                  [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      [MappaTypeMappingDefault(MappaTypeMappingDefaultBehavior.MapSourceType, typeof(TargetDefault))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      [MappaSettings(PolymorphicMapMethodWithMatchingDefaultAttribute = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                                expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                    "this.MapDependency",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_10")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")));
                });
    }

    /// <summary>
    /// Test a mapping can use a polymorphic mappa-generated
    /// method with <see cref="MethodMapStrategy"/> when source type
    /// and target type matched the method source and target types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingRequiresMethodTargetAndSourceTypes()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  
                                  public class Source { public SourceBase DependencyProperty { get; set; } }
                                  public class Target { public TargetBase DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase MapDependency(SourceBase input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBase",
                            "__mappa_tmp_8",
                            expressionAssertions => expressionAssertions.BeInvocationExpressionSyntax(
                                "this.MapDependency",
                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_7"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_8")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_9")));
                });
    }

    /// <summary>
    /// Test a mapping exists without using the polymorphic mappa-generated
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping but nullability do not match
    /// and therefore the method is not applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitlyButNullabilityDoNotMatch()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceBase { public int BaseProperty { get; set; }  }
                                  public class SourceFirst : SourceBase { public Guid FirstProperty { get; set; }  }
                                  
                                  public class TargetBase { public long BaseProperty { get; set; }  }
                                  public class TargetFirst : TargetBase { public string FirstProperty { get; set; }  }
                                  
                                  public class Source { public SourceFirst DependencyProperty { get; set; } }
                                  public class Target { public TargetFirst DependencyProperty { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetFirst), typeof(SourceFirst))]
                                      public partial TargetBase? MapDependency(SourceBase? input);
                                      
                                      public partial Target Map(Source input);
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
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(7)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceFirst",
                            "__mappa_tmp_9",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(Guid).ToString(),
                            "__mappa_tmp_10",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.FirstProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_11",
                            initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_10.ToString")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_12",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_9.BaseProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                            "__mappa_tmp_13",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetFirst",
                                ("FirstProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_11")),
                                ("BaseProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_12")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_14",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("DependencyProperty", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_13")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_14")));
                });
    }
}