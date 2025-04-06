// <copyright file="PragmaWarningSettingsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests around the <see cref="MappaSettingsAttribute.PragmaWarning"/>.
/// </summary>
public sealed class PragmaWarningSettingsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Tests that we can set <see cref="PragmaWarningSetting.NoBlock"/>
    /// on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PragmaWarningIsSetOnTheClassAsNoBlock()
    {
        // TODO [#11] PragmaWarning set on the class as NoBlock.
        await Task.Delay(0);
        true.Should().BeFalse();
    }

    // TODO [#11] PragmaWarning set on the class as Disable.
    // TODO [#11] PragmaWarning set on the method as NoBlock.
    // TODO [#11] PragmaWarning set on the method as Disable.
    // TODO [#11] PragmaWarning set on the class as NoBlock overridden by method as Disable.
    // TODO [#11] PragmaWarning set on the class as Disable overridden by method as NoBlock.
}