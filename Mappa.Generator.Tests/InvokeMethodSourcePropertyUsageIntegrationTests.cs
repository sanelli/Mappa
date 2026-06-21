// <copyright file="InvokeMethodSourcePropertyUsageIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="Extensions.InvokeMethodSourcePropertyUsage"/> and MP00032 scenarios.
/// </summary>
public sealed class InvokeMethodSourcePropertyUsageIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test no MP00032 warning when the invoke method accepts only the source property type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoWarningIsEmittedWhenMappaInvokeMethodAcceptsOnlyPropertyTypeAndMappaUsePropertyTargetsTheSameProperty()
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string GetValue(int foo) => foo.ToString();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should().NotHaveDiagnostics();
    }

    /// <summary>
    /// Test no MP00032 warning when the invoke method accepts a type implicitly convertible from the source property type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoWarningIsEmittedWhenMappaInvokeMethodAcceptsImplicitPropertyTypeAndMappaUsePropertyTargetsTheSameProperty()
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string GetValue(long foo) => foo.ToString();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should().NotHaveDiagnostics();
    }

    /// <summary>
    /// Test MP00032 is emitted when the invoke method accepts only <see cref="Mappa.MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodAcceptsOnlyMappaContextAndMappaUsePropertyTargetsTheSameProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string GetValue(MappaContext ctx) => "fixed";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertyNotUsedByInvokeMethod,
                "Map",
                "Property",
                "Foo",
                "GetValue");
    }

    /// <summary>
    /// Test no MP00032 warning when the invoke method accepts the source property and <see cref="Mappa.MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoWarningIsEmittedWhenMappaInvokeMethodAcceptsPropertyAndMappaContextAndMappaUsePropertyTargetsTheSameProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string GetValue(int foo, MappaContext ctx) => $"{foo}:{ctx.Keys.Count}";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should().NotHaveDiagnostics();
    }

    /// <summary>
    /// Test MP00032 is emitted when the invoke method accepts the source type and <see cref="Mappa.MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodAcceptsSourceAndMappaContextAndMappaUsePropertyTargetsTheSameProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string GetValue(Source source, MappaContext ctx) => source.Foo.ToString();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertyNotUsedByInvokeMethod,
                "Map",
                "Property",
                "Foo",
                "GetValue");
    }

    /// <summary>
    /// Test MP00032 is emitted when the invoke method accepts the source type and <see cref="Mappa.MappaContext"/>
    /// and <see cref="MappaUsePropertyAttribute"/> targets a constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodAcceptsSourceAndMappaContextAndMappaUsePropertyTargetsTheSameConstructorParameter()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Foo { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target(string value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod("value", nameof(GetValue))]
                                      [MappaUseProperty("value", nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string GetValue(Source source, MappaContext ctx) => source.Foo.ToString();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertyNotUsedByInvokeMethod,
                "Map",
                "value",
                "Foo",
                "GetValue");
    }

    /// <summary>
    /// Test no MP00032 warning when the invoke method has three parameters including <see cref="Mappa.MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoWarningIsEmittedWhenMappaInvokeMethodHasThreeParametersAndMappaUsePropertyTargetsTheSameProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaInvokeMethod(nameof(Target.Property), nameof(GetValue))]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string GetValue(Source source, int foo, MappaContext ctx) => $"{source.Foo}:{foo}";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should().NotHaveDiagnostics();
    }
}