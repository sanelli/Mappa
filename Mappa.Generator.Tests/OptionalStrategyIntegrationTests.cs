// <copyright file="OptionalStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for source nd target optionals.
/// </summary>
public sealed class OptionalStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source but optional is not setup (default is disabled);
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDefaultSettingsTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
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
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is enabled on the method;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

     /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is enabled on the class;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnClassTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is disabled on class enabled on the method;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDisabledOnClassButEnabledOnMethodTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source but optional is not setup (default is disabled);
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDefaultSettingsTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.Increment",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnClassTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on method;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on method overriding class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodOverridingClassTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source but optional is not setup (default is disabled);
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDefaultSettingsTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int? PropertyA {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
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
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is enabled on the method;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

     /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is enabled on the class;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnClassTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is disabled on class enabled on the method;
    /// - when optional is present on the source but optional is enabled;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDisabledOnClassButEnabledOnMethodTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source but optional is not setup (default is disabled);
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalDefaultSettingsTargetingEmptyConstructorAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.Increment",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnClassTargetingEmptyConstructorAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on method;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodTargetingEmptyConstructorAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when source property is optional and:
    /// - when optional is present on the source and optional is enabled on method overriding class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceOptionalEnabledOnMethodOverridingClassTargetingEmptyConstructorAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int? Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int?).ToString(),
                                "__mappa_tmp_2");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                condition => condition.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatement => thenStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(statement =>
                                    {
                                        statement.BeLocalDeclarationStatementSyntax(
                                            typeof(int?).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "this.Increment",
                                                firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                    })
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3"))),
                                elseStatement => elseStatement
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax())));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target but optional is not setup (default is disabled);
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalDefaultSettingsTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "HasPropertyA")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
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
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
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
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target but optional is not setup (default is disabled);
    /// - when the mapping happens from source to property;
    /// - target property is required therefore no optional is applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledTargetingEmptyConstructorAndRequiredProperty()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public required int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
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
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target
    /// - when optional is enabled on class;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnClassTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    leftExpressionAssertions => leftExpressionAssertions.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                            leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target
    /// - when optional is enabled on method;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnMethodTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    leftExpressionAssertions => leftExpressionAssertions.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                            leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target
    /// - when optional is enabled on method overriding class;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnMethodOverridingClassTargetingEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    leftExpressionAssertions => leftExpressionAssertions.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                            leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target but optional is not setup (default is disabled);
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalDefaultSettingsTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int Increment(int x)
                                      {
                                         return x + 1;
                                      } 
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "HasPropertyA")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.Increment",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target;
    /// - optional is enabled on class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnClassTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("this.Increment", argument => argument.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement(
                                            leftExpression => leftExpression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target;
    /// - optional is enabled on method;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnMethodTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("this.Increment", argument => argument.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement(
                                            leftExpression => leftExpression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target;
    /// - optional is enabled on method overriding class;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/>;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnMethodOverridingClassTargetingConstructorWithParameterAndCustomMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int Increment(int x)
                                      {
                                         return x + 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_3",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("this.Increment", argument => argument.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement(
                                            leftExpression => leftExpression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the target;
    /// - optional is enabled on method;
    /// - when the mapping uses a custom mapping method via <see cref="MappaInvokeMethodAttribute"/> that does not require an input parameter;
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithTargetOptionalEnabledOnMethodTargetingConstructorWithParameterAndCustomMappingWithoutInputProperty()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      private int Increment()
                                      {
                                         return 1;
                                      } 
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_2",
                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("this.Increment")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.ExclamationEqualsToken,
                                    rightExpression => rightExpression.BeDefaultLiteralExpressionSyntax()),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)

                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement(
                                            leftExpression => leftExpression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when target property is optional and:
    /// - when optional is present on the source and on the target
    /// - when the mapping happens from source to property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithSourceAndTargetOptionalEnabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public bool HasPropertyA => /* fake value */ true;
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeMemberAccessExpressionSyntax("input.HasPropertyA"),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions => statementAssertions.BeAssignmentExpressionStatement(
                                            leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.PropertyA"),
                                            rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}