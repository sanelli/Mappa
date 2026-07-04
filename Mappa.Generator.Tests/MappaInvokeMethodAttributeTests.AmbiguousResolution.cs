// <copyright file="MappaInvokeMethodAttributeTests.AmbiguousResolution.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for ambiguous <see cref="Mappa.Attributes.MappaInvokeMethodAttribute"/> resolution.
/// </summary>
public sealed partial class MappaInvokeMethodAttributeTests
{
    /// <summary>
    /// Test MP00042 is emitted when <see cref="Mappa.Attributes.MappaInvokeMethodAttribute"/> resolution is ambiguous.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AnErrorIsEmittedWhenMappaInvokeMethodResolutionIsAmbiguous()
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
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);

                                      private static string CustomMapPropertyA(Source source)
                                      {
                                          return "static";
                                      }

                                      private string CustomMapPropertyA(Source source)
                                      {
                                          return "instance";
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainAmbiguousInvokeMethodResolutionDiagnostic("CustomMapPropertyA");
    }
}