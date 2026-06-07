// <copyright file="MappaIgnoreTargetPropertyAttributeIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaIgnoreTargetPropertyAttribute"/>.
/// </summary>
public sealed class MappaIgnoreTargetPropertyAttributeIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";

    /// <summary>
    /// Test a mapping via empty constructor succeeds when an unmapped target property is ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingEmptyConstructorWhenATargetPropertyIsIgnored()
    {
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
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyB))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
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
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(this.ToString)}"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test that ignoring a target property prevents MP00017 from being emitted.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoringTargetPropertyPreventsCannotMapNonRequiredPropertyDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { private get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyB))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
    }

    /// <summary>
    /// Test that <see cref="MappaIgnoreTargetPropertyAttribute"/> has no effect when
    /// parameterized constructor mapping is used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MappaIgnoreTargetPropertyHasNoEffectWhenParameterizedConstructorIsUsed()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public record Source(int PropertyA, int PropertyB);
                                  public record Target(string PropertyA, long PropertyB);

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyA))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
    }

    /// <summary>
    /// Test MP00007 is emitted when <see cref="MappaIgnoreTargetPropertyAttribute"/> and
    /// <see cref="MappaUsePropertyAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaIgnoreTargetPropertyConflictsWithMappaUseProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Foo { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.Property))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter, "Map", "Property");
    }

    /// <summary>
    /// Test MP00007 is emitted when <see cref="MappaIgnoreTargetPropertyAttribute"/> and
    /// <see cref="MappaAssignFromConstantAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaIgnoreTargetPropertyConflictsWithMappaAssignFromConstant()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source { }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.Property))]
                                      [MappaAssignFromConstant(nameof(Target.Property), 1)]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter, "Map", "Property");
    }

    /// <summary>
    /// Test MP00007 is emitted when <see cref="MappaIgnoreTargetPropertyAttribute"/> and
    /// <see cref="MappaAssignFromContextAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaIgnoreTargetPropertyConflictsWithMappaAssignFromContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source { }

                                  public class Target
                                  {
                                      public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.Property))]
                                      [MappaAssignFromContext(nameof(Target.Property), "Value")]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter, "Map", "Property");
    }

    /// <summary>
    /// Test MP00007 is emitted when <see cref="MappaIgnoreTargetPropertyAttribute"/> and
    /// <see cref="MappaInvokeMethodAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaIgnoreTargetPropertyConflictsWithMappaInvokeMethod()
    {
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
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyA))]
                                      [MappaInvokeMethod(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);

                                      public string CustomMapPropertyA(Source source) => source.PropertyA.ToString();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter, "Map", "PropertyA");
    }

    /// <summary>
    /// Test MP00034 is emitted when multiple <see cref="MappaIgnoreTargetPropertyAttribute"/>
    /// target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMultipleMappaIgnoreTargetPropertyTargetTheSameProperty()
    {
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
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyA))]
                                      [MappaIgnoreTargetProperty(nameof(Target.PropertyA))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty,
                "Map",
                "PropertyA");
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaIgnoreTargetPropertyAttribute"/>
    /// targets a non-existent property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaIgnoreTargetPropertyTargetsANonExistentProperty()
    {
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
                                      public string PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty("MissingProperty")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaIgnoreTargetPropertyAttribute),
                "MissingProperty",
                TargetTypeName);
    }
}