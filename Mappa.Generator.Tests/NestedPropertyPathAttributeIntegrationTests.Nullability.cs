// <copyright file="NestedPropertyPathAttributeIntegrationTests.Nullability.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Nullability integration tests for nested property path source chains.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test chained source reads use conditional access when nullable reference types are enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithNullableEnableUsingConditionalAccess()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string? City { get; set; }
                                  }

                                  public class LocationDto
                                  {
                                      public AddressDto? Address { get; set; }
                                  }

                                  public class Source
                                  {
                                      public LocationDto? Location { get; set; }
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Location.Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("input.Location?.Address?.City");
    }

    /// <summary>
    /// Test chained source reads use conditional access for reference types when nullable reference types are disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithNullableDisableUsingDotAccess()
    {
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; }
                                  }

                                  public class LocationDto
                                  {
                                      public AddressDto Address { get; set; }
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; }
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Location.Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("input.Location?.Address?.City");
    }

    /// <summary>
    /// Test chained source reads use conditional access for nullable value type segments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathThroughNullableValueTypeSegment()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public int? Code { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Outer.Code", "Outer.Code")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("input.Outer?.Code");
    }

    /// <summary>
    /// Test chained source reads use plain member access for non-nullable value type segments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathThroughNonNullableValueTypeSegment()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public int Code { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Outer.Code", "Outer.Code")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("input.Outer.Code");
        generatedSource.Should().NotContain("input.Outer?.Code");
    }
}