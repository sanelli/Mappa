// <copyright file="MappaUserSettingsOneOfWellKnownTypesTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUserSettings.OneOfWellKnownTypes"/>.
/// </summary>
public sealed class MappaUserSettingsOneOfWellKnownTypesTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test the setting defaults to <see cref="BooleanSetting.Undefined"/> and is inherited from <c>.editorconfig</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InheritsTheValueFromGlobalOptions()
    {
        var undefinedSettings = new MappaUserSettings(CreateGlobalOptions("root = true"));
        undefinedSettings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Undefined);

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.oneofwellknowntypes = enable
                                    """;
        var enabledSettings = new MappaUserSettings(CreateGlobalOptions(editorConfig));
        enabledSettings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Enable);
    }

    /// <summary>
    /// Test attribute values override the global value, undefined values keep it, and values are restored on dispose.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AppliesAndRestoresAttributeOverrides()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.oneofwellknowntypes = enable
                                    """;
        var settings = new MappaUserSettings(CreateGlobalOptions(editorConfig));

        using (settings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Disable }))
        {
            settings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Disable);

            using (settings.Apply(new MappaSettingsAttribute()))
            {
                settings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Disable);
            }

            using (settings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
            {
                settings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Enable);
            }

            settings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Disable);
        }

        settings.OneOfWellKnownTypes.Should().Be(BooleanSetting.Enable);
    }

    private static MappaGlobalOptions CreateGlobalOptions(string editorConfig)
        => new(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken));
}