// <copyright file="MappaInvokeMethodAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInvokeMethodAttribute"/> usage.
/// </summary>
// TODO [#190] MappaInvokeMethod with type targeting non-static method fail locating the method (when map-method is static).
// TODO [#190] MappaInvokeMethod with type targeting non-static method fail locating the method (when map-method is not-static).
public sealed class MappaInvokeMethodAttributeTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with one parameter
    /// with type source class type when the target class does
    /// not have a zero-parameter constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithOneParameterForConstructorWithParametersMapping()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source(int PropertyA, int PropertyB);
                                  public record Target(string PropertyA, long PropertyB);

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                     public partial Target Map(Source input);
                                  
                                     [MappaIgnore]
                                     public string CustomMapPropertyA(Source source)
                                     {
                                        return $"{source.PropertyA} - {source.PropertyB}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with one parameter
    /// with type source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithOneParameterOfTypeSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with one parameter
    /// with type source class type when multiple methods with
    /// lower priorities are available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCorrectNonStaticLocalMethodWithOneParametersdWhenMultipleAvailable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                      
                                      public string CustomMapPropertyA()
                                      {
                                         return "constant";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local <c>static</c> method with one parameter
    /// with type source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticLocalMethodWithOneParameterSameOfTypeSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public static string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionUsingIdentifierNameSyntax(
                                    "CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local <c>static</c> method with no parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithNoParameters()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public static string CustomMapPropertyA()
                                      {
                                         return "This is a constant value";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionUsingIdentifierNameSyntax("CustomMapPropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with one parameter
    /// with type source parameter type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithOneParameterOfTypeSourceParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public string CustomMapPropertyA(int input)
                                      {
                                         return $"Value is - {input}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with two parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithTwoParameterOfExactExpectedType()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source input, int propertyA)
                                      {
                                         return $"Value is {input.PropertyB} + {propertyA}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with one parameter
    /// with type implicitly convertible from source parameter type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithOneParameterOfTypeImplicitlyConvertibleFromSourceParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public string CustomMapPropertyA(long input)
                                      {
                                         return $"Value is {input} and is actually an integer";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local <c>static</c> method with one parameter
    /// with type implicitly convertible from source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticLocalMethodWithOneParameterWithTypeImplicitlyConvertiblefromSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public interface ISource {
                                      int PropertyA { get; }
                                      int PropertyB { get; }
                                  }

                                  public class Source : ISource
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      [MappaIgnore]
                                      public static string CustomMapPropertyA(ISource source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionUsingIdentifierNameSyntax(
                                    "CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> local method with two parameters
    /// of implicit convertible types.
    /// </summary>
    /// <param name="sourceType">The type of the source parameter in the method to invoke.</param>
    /// <param name="propertyType">The type of the property parameter in the method to invoke.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [IntegrationTest]
    [InlineData("Source", "long")]
    [InlineData("ISource", "int")]
    [InlineData("ISource", "long")]
    public async Task CanMapUsingNonStaticLocalMethodWithTwoParameterOfImplicitConvertibleType(
        string sourceType,
        string propertyType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyType);

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           public interface ISource 
                           {
                              int PropertyB { get; }
                           }

                           public class Source
                             : ISource
                           {
                               public int PropertyA { get; set; }
                               public int PropertyB { get; set; }
                           }

                           public class Target
                           {
                               public string PropertyA { get; set; }
                               public long PropertyB { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                               public partial Target Map(Source input);
                           
                               [MappaIgnore]
                               public string CustomMapPropertyA({{sourceType}} input, {{propertyType}} propertyA)
                               {
                                  return $"Value is {input.PropertyB} + {propertyA}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a <c>static</c> method on the type specified in the attribute
    /// with one parameter with type source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMethodOnSpecifiedTypeWithOneParameterOfTypeSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public sealed class Helper
                                  {
                                      public static string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), typeof(Helper), nameof(Helper.CustomMapPropertyA))]
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Helper.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a <c>static</c> method on the type specified in the attribute
    /// with two parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMethodOnSpecifiedTypeWithTwoParameters()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), typeof(Helper), nameof(Helper.CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                  }
                                  
                                  public sealed class Helper
                                  {
                                      public static string CustomMapPropertyA(Source source, int property)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB} - {property}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Helper.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a <c>static</c> method on the property specified in the attribute
    /// with one parameter with type source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMethodOnSpecifiedPropertyWithOneParameterOfTypeSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private Helper Dependency => new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(Dependency), nameof(Helper.CustomMapPropertyA))]
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "Dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> method on a non-static field specified in the attribute
    /// with one parameter with type source class type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticMethodOnNonStaticFieldWithOneParameterOfTypeSource()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private Helper dependency = new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(dependency), nameof(Helper.CustomMapPropertyA))]
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> does not apply
    /// to inner classes.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MappaInvokeMethodAttributeDoesNotApplyToInnerClasses()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public interface ISource
                                  {
                                    int PropertyA { get; }
                                  }
                                  
                                  public class InnerSource
                                    : ISource
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }
                                  
                                  public class InnerTarget
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Source
                                    : ISource
                                  {
                                      public int PropertyA { get; set; }
                                      public InnerSource PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public InnerTarget PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);
                                      
                                      public string CustomMapPropertyA(ISource source)
                                      {
                                        return $"This should not be applied to to the inner property {source.PropertyA}";
                                      }
                                  }}
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
                        .HasSyntaxNodesCount(9)
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string",
                                "__mappa_tmp_5",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_6",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                "__mappa_tmp_7",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_8",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_7")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_8");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> when targeting
    /// internal property setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingInvokeMethodAttributeWhenTargetSetterIsInternal()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                        public int PropertyA {get;set;}
                                  }
                                  
                                  public class Target
                                  {
                                        public string PropertyA {get; internal set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                     public partial Target Map(Source input);
                                  
                                     public string CustomMapPropertyA(Source source)
                                     {
                                        return $"#{source.PropertyA}#";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
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
                                        ("PropertyA", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> when targeting
    /// private property setter and therefore no mapping can happen.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapGenerateWarningsWhenUsingInvokeMethodAttributeButTargetPropertyIsPrivate()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                        public int PropertyA {get;set;}
                                        public int PropertyB {get;set;}
                                  }
                                  
                                  public class Target
                                  {
                                        public string PropertyA {get; set;}
                                        public string PropertyB {get; private set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyB), nameof(CustomMapPropertyB))]
                                     public partial Target Map(Source input);
                                  
                                     public string CustomMapPropertyA(Source source)
                                     {
                                        return $"#{source.PropertyA}#";
                                     }
                                     
                                     public string CustomMapPropertyB(Source source)
                                     {
                                         return $"#{source.PropertyB}#";
                                     }
                                  }
                                  
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveOnlyWarnings("MP00014")
            .HaveDiagnostic(MappaDiagnosticDescriptors.PropertySetterIsNotAccessible, "Mappa.Generator.Tests.UnitTests.SourceCode.Target.PropertyB")
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
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
                                        ("PropertyA", propertyInitializerAssertions => propertyInitializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test diagnostic is returned when trying to use a non-static field
    /// from a static mapper method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTryingToUseNonStaticFieldFromStaticMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private Helper dependency = new();

                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(dependency), nameof(Helper.CustomMapPropertyA))]
                                      public static partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic, "dependency")
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "PropertyA");
    }

    /// <summary>
    /// Test diagnostic is returned when trying to use a non-static property
    /// from a static mapper method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTryingToUseNonStaticPropertyFromStaticMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private Helper Dependency {get;} = new Helper();

                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(Dependency), nameof(Helper.CustomMapPropertyA))]
                                      public static partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic, "Dependency")
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "PropertyA");
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> method on the <c>static</c> field specified in the attribute
    /// to map a <c>static</c> method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticMethodOnStaticFieldForStaticMapMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private static Helper dependency = new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(dependency), nameof(Helper.CustomMapPropertyA))]
                                      public static partial Target Map(Source input);
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> method on the <c>static</c> property specified in the attribute
    /// to map a <c>static</c> method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticMethodOnStaticPropertyForStaticMapMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private static Helper Dependency {get;} = new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(Dependency), nameof(Helper.CustomMapPropertyA))]
                                      public static partial Target Map(Source input);
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "Dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test diagnostic is returned when targeting non-static method from a static method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestDiagnosticIsReturnedWhenTargetingNonStaticMethodFromStaticMapMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                        return $"{source.PropertyA} - {source.PropertyB}";
                                      }

                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public static partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotDetectSuitableMethodToInvokeForParameter, "CustomMapPropertyA", "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper", "PropertyA")
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "PropertyA");
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a <c>static</c> local method with one parameter
    /// with type source class type when the target class does
    /// not have a zero-parameter constructor and map method is
    /// <c>static</c> as well.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStaticMethodByInvokingAStaticMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source(int PropertyA, int PropertyB);
                                  public record Target(string PropertyA, long PropertyB);

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                     public static partial Target Map(Source input);
                                  
                                     public static string CustomMapPropertyA(Source source)
                                     {
                                        return $"{source.PropertyA} - {source.PropertyB}";
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionUsingIdentifierNameSyntax(
                                    "CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a <c>static</c> local method with one parameter
    /// with type source class type when the target class does
    /// not have a zero-parameter constructor and map method is
    /// non-<c>static</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonStaticMethodByInvokingAStaticMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source(int PropertyA, int PropertyB);
                                  public record Target(string PropertyA, long PropertyB);

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                     [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                     public partial Target Map(Source input);
                                  
                                     [MappaIgnore]
                                     public static string CustomMapPropertyA(Source source)
                                     {
                                        return $"{source.PropertyA} - {source.PropertyB}";
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionUsingIdentifierNameSyntax(
                                    "CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> method on a <c>static</c> field specified in the attribute
    /// with one parameter with type source class type and the method being mapped
    /// is non-<c>static</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeNonStaticMethodFromStaticFieldFromAndTheMethodBeingMappedIsNotStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private static Helper dependency = new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(dependency), nameof(Helper.CustomMapPropertyA))]
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting
    /// a non-<c>static</c> method on a <c>static</c> property specified in the attribute
    /// with one parameter with type source class type and the method being mapped
    /// is non-<c>static</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeNonStaticMethodFromStaticPropertyFromAndTheMethodBeingMappedIsNotStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }
                                  
                                  public class Helper
                                  {
                                      public string CustomMapPropertyA(Source source)
                                      {
                                         return $"{source.PropertyA} - {source.PropertyB}";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private static Helper Dependency {get;} = new();
                                  
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(Dependency), nameof(Helper.CustomMapPropertyA))]
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
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "Dependency.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
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
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }
}