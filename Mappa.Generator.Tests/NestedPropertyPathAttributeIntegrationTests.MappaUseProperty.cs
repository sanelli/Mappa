// <copyright file="NestedPropertyPathAttributeIntegrationTests.MappaUseProperty.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// <see cref="MappaUsePropertyAttribute"/> nested property path integration tests.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test mapping succeeds with a two-segment target and source property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithTwoSegmentTargetAndSourcePath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public string Value { get; set; } = null!;
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
                                      [MappaUseProperty("Outer.Value", "Outer.Value")]
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
    /// Test mapping succeeds with a three-segment target and source property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithThreeSegmentTargetAndSourcePath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
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
        generatedSource.Should().Contain("input.Location?.Address");
        generatedSource.Should().Contain(".City");
        generatedSource.Should().Contain("throw new System.NullReferenceException");
    }

    /// <summary>
    /// Test mapping succeeds with a three-segment nested target path (Location.Address.City).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithThreeSegmentNestedTargetPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class LocationDto
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Location.Address.City", "Location.Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("Location");
        generatedSource.Should().Contain("Address");
        generatedSource.Should().Contain("City");
    }

    /// <summary>
    /// Test multiple <see cref="MappaUsePropertyAttribute"/> declarations with different nested paths under the same root target member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentNestedPathsSharingRoot()
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
                                      [MappaUseProperty("Address.ZipCode", "Address.ZipCode")]
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
    /// Test multiple <see cref="MappaUsePropertyAttribute"/> declarations with different nested paths and different root target members.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentNestedPathsAndDifferentRoots()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class ContactDto
                                  {
                                      public string Name { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                      public ContactDto Contact { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                      public ContactDto Contact { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaUseProperty("Contact.Name", "Contact.Name")]
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
    /// Test multiple nested <see cref="MappaUsePropertyAttribute"/> paths that share a target root
    /// but use a different first source segment.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentSourceRootThanTargetRoot()
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

                                  public class LocationDto
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Location.Address.City")]
                                      [MappaUseProperty("Address.ZipCode", "Location.Address.ZipCode")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("input.Location");
        generatedSource.Should().Contain(".City");
        generatedSource.Should().Contain(".ZipCode");
    }

    /// <summary>
    /// Test mapping fails when multiple <see cref="MappaUsePropertyAttribute"/> declarations target the same exact nested path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapFailsWhenMultipleMappaUsePropertyAttributesTargetTheSameExactNestedPath()
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
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty, "Map", "Address");
    }

    /// <summary>
    /// Test mapping fails when multiple flat <see cref="MappaUsePropertyAttribute"/> declarations target the same property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapFailsWhenMultipleMappaUsePropertyAttributesTargetTheSameExactFlatPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int Foo { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.PropertyA), nameof(Source.Foo))]
                                      [MappaUseProperty(nameof(Target.PropertyA), nameof(Source.PropertyA))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty, "Map", "PropertyA");
    }

    /// <summary>
    /// Test swapped flat <see cref="MappaUsePropertyAttribute"/> mappings including identity int-to-int.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSwappedFlatMappaUsePropertyIncludingIntToIntIdentity()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum CountingValues { One, Two, Three }

                                  public class Source
                                  {
                                      public int ParamA { get; set; }
                                      public CountingValues ParamB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string ParamA { get; set; } = string.Empty;
                                      public int ParamB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.ParamA), nameof(Source.ParamB))]
                                      [MappaUseProperty(nameof(Target.ParamB), nameof(Source.ParamA))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        var generatedSource = GetGeneratedMapperSource(generatedResults);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
        generatedSource.Should().Contain("ParamB");
        generatedSource.Should().Contain("input.ParamA");
    }
}