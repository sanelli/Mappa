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
    public async Task CanMapWithOptionalDefaultSettingsTargetingConstructorWithParameter()
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
    public async Task CanMapWithOptionalEnabledOnMethodTargetingConstructorWithParameter()
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
    public async Task CanMapWithOptionalEnabledOnClassTargetingConstructorWithParameter()
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
    public async Task CanMapWithOptionalDisabledOnClassButEnabledOnMethodTargetingConstructorWithParameter()
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
    public async Task CanMapWithOptionalDefaultSettingsTargetingConstructorWithParameterAndCustomMapping()
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
    public async Task CanMapWithOptionalEnabledOnClassTargetingConstructorWithParameterAndCustomMapping()
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
    public async Task CanMapWithOptionalEnabledOnMethodTargetingConstructorWithParameterAndCustomMapping()
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
    public async Task CanMapWithOptionalEnabledOnMethodOverridingClassTargetingConstructorWithParameterAndCustomMapping()
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

    // TODO [#48] Test with optional disabled targeting non-optional property.
    // TODO [#48] Test with optional enabled on method targeting non-optional property.
    // TODO [#48] Test with optional enabled on class targeting non-optional property.
    // TODO [#48] Test with optional enabled on method overriding on class targeting non-optional property.
    // TODO [#48] Test with optional disabled targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional disabled targeting optional property.
    // TODO [#48] Test with optional enabled on class targeting optional property.
    // TODO [#48] Test with optional enabled on method targeting optional property.
    // TODO [#48] Test with optional enabled on method overriding on class targeting optional property.
    // TODO [#48] Test with optional disabled targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with nested struct/classes.
}