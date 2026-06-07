// <copyright file="MappingAttributeConflictWarningIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for mapping attribute conflict and missing-target warnings (MP00031–MP00033).
/// </summary>
public sealed class MappingAttributeConflictWarningIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";

    /// <summary>
    /// Test MP00031 is emitted when <see cref="MappaAssignFromContextAttribute"/> and
    /// <see cref="MappaUsePropertyAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromContextAndMappaUsePropertyTargetTheSameProperty()
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
                                      [MappaAssignFromContext(nameof(Target.Property), "Value")]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertySourcePropertyWillNotBeUsed,
                "Map",
                "Property",
                "Foo",
                nameof(MappaAssignFromContextAttribute));
    }

    /// <summary>
    /// Test MP00031 is emitted when <see cref="MappaAssignFromContextAttribute"/> and
    /// <see cref="MappaUsePropertyAttribute"/> target the same constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromContextAndMappaUsePropertyTargetTheSameConstructorParameter()
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
                                      [MappaAssignFromContext("value", "Value")]
                                      [MappaUseProperty("value", nameof(Source.Foo))]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertySourcePropertyWillNotBeUsed,
                "Map",
                "value",
                "Foo",
                nameof(MappaAssignFromContextAttribute));
    }

    /// <summary>
    /// Test MP00031 is emitted when <see cref="MappaAssignFromConstantAttribute"/> and
    /// <see cref="MappaUsePropertyAttribute"/> target the same property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromConstantAndMappaUsePropertyTargetTheSameProperty()
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
                                      public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromConstant(nameof(Target.Property), 42)]
                                      [MappaUseProperty(nameof(Target.Property), nameof(Source.Foo))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertySourcePropertyWillNotBeUsed,
                "Map",
                "Property",
                "Foo",
                nameof(MappaAssignFromConstantAttribute));
    }

    /// <summary>
    /// Test MP00031 is emitted when <see cref="MappaAssignFromConstantAttribute"/> and
    /// <see cref="MappaUsePropertyAttribute"/> target the same constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromConstantAndMappaUsePropertyTargetTheSameConstructorParameter()
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
                                      public Target(int value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromConstant("value", 42)]
                                      [MappaUseProperty("value", nameof(Source.Foo))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaUsePropertySourcePropertyWillNotBeUsed,
                "Map",
                "value",
                "Foo",
                nameof(MappaAssignFromConstantAttribute));
    }

    /// <summary>
    /// Test MP00032 is emitted when <see cref="MappaInvokeMethodAttribute"/> resolves to a
    /// parameterless method while <see cref="MappaUsePropertyAttribute"/> is also defined.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodHasNoParametersAndMappaUsePropertyTargetsTheSameProperty()
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
                                      public string GetValue() => "fixed";
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
    /// Test MP00032 is emitted when <see cref="MappaInvokeMethodAttribute"/> resolves to a
    /// single source-type parameter while <see cref="MappaUsePropertyAttribute"/> is also defined.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodAcceptsOnlySourceTypeAndMappaUsePropertyTargetsTheSameProperty()
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
                                      public string GetValue(Source source) => source.Foo.ToString();
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
    /// Test MP00032 is emitted when <see cref="MappaInvokeMethodAttribute"/> resolves to a
    /// parameterless method while <see cref="MappaUsePropertyAttribute"/> targets a constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodHasNoParametersAndMappaUsePropertyTargetsTheSameConstructorParameter()
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
                                      public Target(string value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod("value", nameof(GetValue))]
                                      [MappaUseProperty("value", nameof(Source.Foo))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string GetValue() => "fixed";
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
    /// Test MP00032 is emitted when <see cref="MappaInvokeMethodAttribute"/> resolves to a
    /// single source-type parameter while <see cref="MappaUsePropertyAttribute"/> targets a constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodAcceptsOnlySourceTypeAndMappaUsePropertyTargetsTheSameConstructorParameter()
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
                                      public Target(string value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod("value", nameof(GetValue))]
                                      [MappaUseProperty("value", nameof(Source.Foo))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string GetValue(Source source) => source.Foo.ToString();
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
    /// Test MP00033 is emitted when <see cref="MappaUsePropertyAttribute"/> targets a non-existent property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaUsePropertyTargetsANonExistentProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Missing", nameof(Source.Property))]
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
                "Missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaInvokeMethodAttribute"/> targets a non-existent property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaInvokeMethodTargetsANonExistentProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Other { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Other { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod("Missing", nameof(GetValue))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string GetValue() => "fixed";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaInvokeMethodAttribute),
                "Missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaAssignFromContextAttribute"/> targets a non-existent property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromContextTargetsANonExistentProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Property { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext("Missing", "Value")]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
                "Map",
                nameof(MappaAssignFromContextAttribute),
                "Missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaAssignFromConstantAttribute"/> targets a non-existent property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromConstantTargetsANonExistentProperty()
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
                                      [MappaAssignFromConstant("Missing", 42)]
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
                nameof(MappaAssignFromConstantAttribute),
                "Missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaUsePropertyAttribute"/> targets a non-existent constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaUsePropertyTargetsANonExistentConstructorParameter()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target(string value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("missing", nameof(Source.Value))]
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
                "missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test MP00033 is emitted when <see cref="MappaAssignFromConstantAttribute"/> targets a non-existent constructor parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AWarningIsEmittedWhenMappaAssignFromConstantTargetsANonExistentConstructorParameter()
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
                                      public Target(int value) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromConstant("missing", 42)]
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
                nameof(MappaAssignFromConstantAttribute),
                "missing",
                TargetTypeName);
    }

    /// <summary>
    /// Test no MP00032 warning is emitted when <see cref="MappaInvokeMethodAttribute"/> uses the
    /// source property via a two-parameter overload together with <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NoWarningIsEmittedWhenMappaInvokeMethodUsesTheSourcePropertyDefinedByMappaUseProperty()
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
                                      public string GetValue(Source source, int foo) => $"{source.Foo}:{foo}";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should().NotHaveDiagnostics();
    }
}