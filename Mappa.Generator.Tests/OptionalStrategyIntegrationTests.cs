// <copyright file="OptionalStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
                                      public bool HasPropertyA { get; set; }
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
                                      public bool HasPropertyA { get; set; }
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
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                  [MappaSettings(Optional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                  [MappaSettings(Optional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      public bool HasPropertyA { get; set; }
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
                                  [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                  [MappaSettings(Optional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                        leftExpression => leftExpression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_1"))),
                                elseStatement => elseStatement
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int? PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(Increment))]
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                    .IsBlockStatement()
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
                                    .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                        .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                        .IsBlockStatement()
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
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(Optional = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(Optional = BooleanSetting.Enable)]
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
                                        .IsBlockStatement()
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

    // TODO [#48] Test with optional disabled targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting optional property with mapping user defined via attribute (method invokation does not have any input parameter in order to test the missing source).
    // TODO [#48] Test with optional enabled on method targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled and source is optional and target is optional.
    // TODO [#48] Test that target optional is not generated when target property is required.
    // TODO [#48] Test with nested struct/classes.
}