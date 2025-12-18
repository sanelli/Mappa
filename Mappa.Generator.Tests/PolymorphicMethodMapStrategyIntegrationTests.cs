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
// TODO [#49] Add tests to check we can pick up polymorphic method when mapping when types are defined in the MappaTypeMappingDefault attribute implicitly.
// TODO [#49] Add tests to check we can pick up polymorphic when nullability is disabled.
// TODO [#49] Add tests to check we can pick up polymorphic when nullability is enabled and match but is not the same.
// TODO [#49] Add tests to check method is not picked up when nullability do not match.
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
    /// method where the types are defined by <see cref="MappaTypeMappingDefaultAttribute"/>
    /// using explicit target mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    // TODO [#49] Fix the test: the MapDependency method is not being picked up.
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomPolymorphicPartialMethodWhereMappingIsDefinedInTheMappingDefaultAttributeExplicitly()
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
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBase",
                            "__mappa_tmp_7",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.DependencyProperty")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
                            "__mappa_tmp_8",
                            initializerAssertions => initializerAssertions.BeCastExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetDefault",
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
}