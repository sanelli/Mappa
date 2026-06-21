// <copyright file="MappaDiagnosticsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for diagnostics reported via <see cref="MappaDiagnostics"/>.
/// </summary>
public sealed class MappaDiagnosticsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaDiagnosticDescriptors.CannotDetectType"/> is emitted when
    /// <see cref="MappaInvokeMethodAttribute"/> references a nested type using a display name
    /// that cannot be resolved via metadata.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaInvokeMethodReferencesNestedTypeWithUnresolvedMetadataName()
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
                                      public string Property { get; set; }
                                  }

                                  public class HelperContainer
                                  {
                                      public static class NestedHelper
                                      {
                                          public static string MapProperty(int value) => value.ToString();
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethod(nameof(Target.Property), typeof(HelperContainer.NestedHelper), nameof(NestedHelper.MapProperty))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotDetectType,
                "Mappa.Generator.Tests.UnitTests.SourceCode.HelperContainer.NestedHelper");
    }

    /// <summary>
    /// Test <see cref="MappaDiagnosticDescriptors.CannotUseMappaAssignFromContextAttributeWithoutContextParameter"/>
    /// is emitted when <see cref="MappaAssignFromContextAttribute"/> is used without a
    /// <see cref="Mappa.MappaContext"/> parameter on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaAssignFromContextIsUsedWithoutMappaContextParameter()
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
                                      public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext(nameof(Target.Property), "Value")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotUseMappaAssignFromContextAttributeWithoutContextParameter,
                "Property");
    }
}