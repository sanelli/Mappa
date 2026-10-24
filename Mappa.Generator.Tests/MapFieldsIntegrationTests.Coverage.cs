// <copyright file="MapFieldsIntegrationTests.Coverage.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Coverage-oriented field mapping integration tests.
/// </summary>
public sealed partial class MapFieldsIntegrationTests
{
    /// <summary>
    /// Maps a target field from a source field via <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTargetFieldFromSourceFieldViaMappaUsePropertyAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int SourceValue;
                                  }

                                  public class Target
                                  {
                                      public int TargetValue;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.TargetValue), "SourceValue")]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.SourceValue")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("TargetValue", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Maps a target field via <see cref="Mappa.Attributes.MappaInvokeMethodAttribute"/> from a source property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTargetFieldViaMappaInvokeMethodFromSourceProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.Value), nameof(Increment), SourcePropertyName = nameof(Source.Value))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      private int Increment(int value) => value + 1;
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.Increment",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Assigns a constant to a target field via <see cref="Mappa.Attributes.MappaAssignFromConstantAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignConstantToTargetFieldViaMappaAssignFromConstantAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromConstant(nameof(Target.Value), 42)]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeLiteralExpressionSyntax(42)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Maps a nested source field path onto a flat target member via <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithNestedSourceFieldPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Address
                                  {
                                      public string City;
                                  }

                                  public class Source
                                  {
                                      public Address Location;
                                  }

                                  public class Target
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.City), "Location.City")]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Location.City")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("City", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Maps public fields onto a target produced by an empty-ctor-like object factory.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTargetFieldsUsingEmptyCtorLikeObjectFactory()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget() => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax("this.CreateTarget")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_1")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Reports that no strategy exists when source and target fields have incompatible types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenSourceAndTargetFieldsHaveIncompatibleTypes()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public System.Guid Value;
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "System.Guid", "int")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Does not map a private target field when <c>AllowFields</c> is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapPrivateTargetFieldWhenAllowFieldsIsDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers(AllowFields = false)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, SourceTypeName, TargetTypeName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Does not map a private source field when the allow-inaccessible whitelist excludes it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapPrivateSourceFieldWhenAllowInaccessibleWhitelistExcludesIt()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Value;
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers("Other")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, SourceTypeName, TargetTypeName)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaAllowInaccessibleSourceMembersAttribute),
                "Other",
                SourceTypeName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Maps a target field from a source property via <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTargetFieldFromSourcePropertyViaMappaUsePropertyAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Renamed { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.Value), nameof(Source.Renamed))]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Renamed")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Reports when multiple <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/> target the same field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenMultipleUsePropertyAttributesTargetTheSameField()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Foo { get; set; }
                                      public int Bar { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.Value), nameof(Source.Foo))]
                                      [MappaUseProperty(nameof(Target.Value), nameof(Source.Bar))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty, "Map", "Value")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Substitutes null for a target field using an explicit source field name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanNullSubstituteTargetFieldFromExplicitSourceFieldName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Other;
                                  }

                                  public class Target
                                  {
                                      public string Value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), SourcePropertyName = "Other", AssignValue = "Fallback")]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Other")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeBinaryExpressionSyntax(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    SyntaxKind.CoalesceExpression,
                                    right => right.BeLiteralExpressionSyntax("Fallback"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Maps a nested source field path onto a target field via <see cref="Mappa.Attributes.MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNestedSourceFieldPathOntoTargetFieldViaMappaUsePropertyAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Address
                                  {
                                      public string City;
                                  }

                                  public class Source
                                  {
                                      public Address Location;
                                  }

                                  public class Target
                                  {
                                      public string City;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.City), "Location.City")]
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Location.City")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("City", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Leaves an unmatched target field unmapped when no source member matches.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task LeavesUnmatchedTargetFieldUnmappedWhenNoSourceMemberMatches()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value;
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                      public int Extra;
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable, MapTargetFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
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
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Reports that no strategy exists when a source field cannot map to a target property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenSourceFieldCannotMapToTargetProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public System.Guid Value;
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(MapSourceFields = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "System.Guid", "int")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }
}