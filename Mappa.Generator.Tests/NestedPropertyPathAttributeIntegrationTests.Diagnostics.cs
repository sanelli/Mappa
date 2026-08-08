// <copyright file="NestedPropertyPathAttributeIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for nested property paths.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test MP00033 is emitted when the first target path segment does not exist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenNestedTargetPathFirstSegmentDoesNotExist()
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
                                      public string PropertyA { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Missing.Value", "PropertyA")]
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
                nameof(MappaUsePropertyAttribute),
                "Missing.Value",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when an intermediate nested target path segment does not exist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenNestedTargetPathIntermediateSegmentDoesNotExist()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.Missing.City", "Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaUsePropertyAttribute),
                "Address.Missing.City",
                TargetTypeName)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath,
                "Map",
                nameof(MappaUsePropertyAttribute),
                "Address.City",
                "Address.Missing.City");
    }

    /// <summary>
    /// Test MP00043 is emitted when the source path has fewer segments than the target path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenSourcePropertyPathIsShorterThanTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City.Zip", "Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaUsePropertyAttribute),
                "Address.City.Zip",
                TargetTypeName)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath,
                "Map",
                nameof(MappaUsePropertyAttribute),
                "Address.City",
                "Address.City.Zip");
    }

    /// <summary>
    /// Test MP00044 is emitted when a nested source path segment does not exist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenNestedSourcePropertyPathSegmentDoesNotExist()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Address.Missing.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeSourcePropertyPathSegmentDoesNotExist,
                "Map",
                nameof(MappaUsePropertyAttribute),
                "Address.Missing.City",
                "Missing",
                SourceTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when a nested assign-to-context target path segment does not exist.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenNestedAssignToContextTargetPathSegmentDoesNotExist()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignToContext("caboom", "Address.Missing")]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible,
                "Map",
                "caboom",
                "Address.Missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test nested ignore under a shared nested attribute scope with a sibling <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanIgnoreOneNestedTargetPropertyWhileMappingSiblingUnderSameRoot()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                      public string ZipCode { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaIgnoreTargetProperty("Address.ZipCode")]
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
                block => block
                    .HasSyntaxNodesCount(5)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                        "__mappa_tmp_1",
                        value => value.BeMemberAccessExpressionSyntax("input.Address")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        typeof(string).ToString(),
                        "__mappa_tmp_2",
                        value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.City")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                        "__mappa_tmp_3",
                        value => value.BeObjectCreationExpressionSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        TargetTypeName,
                        "__mappa_tmp_4",
                        value => value.BeObjectCreationExpressionSyntax(
                            TargetTypeName,
                            ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4")));
    }
}