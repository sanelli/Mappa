// <copyright file="InvokeParseStringWithFormatMapStrategyIntegrationTests.InvalidDateTimeStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Invalid date/time style integration tests for string parse strategies.
/// </summary>
public sealed partial class InvokeParseStringWithFormatMapStrategyIntegrationTests
{
    /// <summary>
    /// Test MP00038 is emitted and code generation continues when an invalid integer
    /// <see cref="System.Globalization.DateTimeStyles"/> value is set on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningAndGeneratesCodeWhenInvalidDateTimeStyleIsDefinedOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(DateTimeStyle = (DateTimeStyles)999)]
                                      public partial DateTime Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveOnlyWarnings("MP00038")
            .HaveGeneratedSourceCode();
    }

    /// <summary>
    /// Test MP00038 is emitted when an invalid global date/time style is defined on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenInvalidGlobalDateTimeStyleIsDefinedOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaSettings(GlobalDateTimeStyle = (DateTimeStyles)999)]
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateTime Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveOnlyWarnings("MP00038")
            .HaveGeneratedSourceCode();
    }
}