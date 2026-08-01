// <copyright file="MappaAllowInaccessibleMembersIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for inaccessible-member attributes and generated <c>UnsafeAccessor</c> methods.
/// </summary>
public sealed class MappaAllowInaccessibleMembersIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string AccessorsClassName = "__MappaInaccessibleAccessors";

    /// <summary>
    /// Maps a private source getter to a public target property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPrivateSourceGetterToPublicTargetProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
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
                                        ("Property", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Property");
            });
    }

    /// <summary>
    /// Maps a public source property to a private target setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapPublicSourceToPrivateTargetSetter()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; private set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                $"{AccessorsClassName}.__mappa_tmp_3",
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_3", "Method", "set_Property");
            });
    }

    /// <summary>
    /// Maps using a private parameterless target constructor and a public property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPrivateParameterlessTargetConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      private Target()
                                      {
                                      }

                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_1"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeAssignmentExpressionStatement(
                                leftAssertions => leftAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Property"),
                                assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_1", "Constructor", null);
            });
    }

    /// <summary>
    /// Maps using a private target constructor that takes parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPrivateParameterizedTargetConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Name { get; set; }
                                      public int Age { get; set; }
                                  }

                                  public class Target
                                  {
                                      private Target(string name, int age)
                                      {
                                          this.Name = name;
                                          this.Age = age;
                                      }

                                      public string Name { get; }

                                      public int Age { get; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Name"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Age"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_4",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_3",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_3", "Constructor", null);
            });
    }

    /// <summary>
    /// Maps a protected source property when opted in.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapProtectedSourcePropertyWhenOptedIn()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      protected int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
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
                                        ("Property", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Property");
            });
    }

    /// <summary>
    /// Maps only the whitelisted inaccessible source property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapOnlyWhitelistedInaccessibleSourceProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Allowed { get; set; }
                                      private int Ignored { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Allowed { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers(nameof(Source.Allowed))]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
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
                                        ("Allowed", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Allowed");
            });
    }

    /// <summary>
    /// Fills a private get-only collection target property when opted in.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanFillPrivateGetOnlyCollectionTargetProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int[] Property { get; set; } = [];
                                  }

                                  public class Target
                                  {
                                      private ICollection<string> Property { get; } = new List<string>();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int[]).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                                conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                    .BeBlockStatement()
                                    .AsBlock()
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_4",
                                            initializationAssertions => initializationAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                            typeof(string).ToString(),
                                            "__mappa_tmp_5",
                                            initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                "__mappa_tmp_4.ToString"));
                                    })
                                    .HasNextSyntaxNode(forStatementAssertion =>
                                    {
                                        forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                            $"{AccessorsClassName}.__mappa_tmp_6(__mappa_tmp_1).Add",
                                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_6", "Method", "get_Property");
            });
    }

    /// <summary>
    /// Reports MP00067 when inaccessible attributes are used without UnsafeAccessor support.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenUnsafeAccessorIsNotSupported()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, LanguageVersion.CSharp11, CancellationToken.None)
            .ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.UnsafeAccessorNotSupported, "Map");
    }

    /// <summary>
    /// Reports MP00033 when the inaccessible whitelist names a missing property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenInaccessibleWhitelistNamesAMissingProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers("MissingProperty")]
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
                nameof(MappaAllowInaccessibleSourceMembersAttribute),
                "MissingProperty",
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Property", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Reports MP00068 when both allow flags are false.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenBothTargetAllowFlagsAreFalse()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers(AllowProperties = false, AllowConstructors = false)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.AllowInaccessibleTargetMembersDisabledAll, "Map");
    }

    /// <summary>
    /// Reports MP00069 when inaccessible attributes are used on a queryable projection.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsReportedWhenInaccessibleAttributesAreUsedOnProjection()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
                                      public partial IQueryable<Target> Map(IQueryable<Source> input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasAllowInaccessibleMembers, "Map")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Reuses a single getter accessor when the same private source property maps to two targets.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReuseGetterAccessorWhenSamePrivateSourceMapsToTwoTargets()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      private int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int First { get; set; }
                                      public int Second { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleSourceMembers]
                                      [MappaUseProperty(nameof(Target.First), nameof(Source.Value))]
                                      [MappaUseProperty(nameof(Target.Second), nameof(Source.Value))]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    $"{AccessorsClassName}.__mappa_tmp_2",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("First", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("Second", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_2", "Method", "get_Value");
            });
    }

    /// <summary>
    /// Does not write inaccessible target properties when <c>AllowProperties</c> is <c>false</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapPrivateTargetSetterWhenAllowPropertiesIsFalse()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PublicProperty { get; set; }
                                      public int PrivateProperty { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int PublicProperty { get; set; }
                                      public int PrivateProperty { get; private set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers(AllowProperties = false)]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PublicProperty"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("PublicProperty", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Does not invoke inaccessible constructors when <c>AllowConstructors</c> is <c>false</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapWhenPrivateConstructorIsDisallowed()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      private Target()
                                      {
                                      }

                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers(AllowConstructors = false)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, SourceTypeName, TargetTypeName);
    }

    /// <summary>
    /// Does not write a private target setter that is outside the property whitelist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotMapPrivateTargetSetterOutsideWhitelist()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Allowed { get; set; }
                                      public int Denied { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Allowed { get; private set; }
                                      public int Denied { get; private set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAllowInaccessibleTargetMembers(nameof(Target.Allowed))]
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
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetTypeName));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Allowed"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                $"{AccessorsClassName}.__mappa_tmp_3",
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                })
            .HaveClass(AccessorsClassName, classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.FileKeyword, SyntaxKind.StaticKeyword)
                    .HaveMethods(1)
                    .HaveExternUnsafeAccessorMethod("__mappa_tmp_3", "Method", "set_Allowed");
            });
    }
}