// <copyright file="MappaUserSettingsMapFieldsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUserSettings.MapSourceFields"/> and <see cref="MappaUserSettings.MapTargetFields"/>.
/// </summary>
public sealed class MappaUserSettingsMapFieldsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test the settings default to <see cref="BooleanSetting.Undefined"/> and are inherited from <c>.editorconfig</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InheritsTheValuesFromGlobalOptions()
    {
        var undefinedSettings = new MappaUserSettings(CreateGlobalOptions("root = true"));
        undefinedSettings.MapSourceFields.Should().Be(BooleanSetting.Undefined);
        undefinedSettings.MapTargetFields.Should().Be(BooleanSetting.Undefined);

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.mapsourcefields = enable
                                    mappa.maptargetfields = disable
                                    """;
        var enabledSettings = new MappaUserSettings(CreateGlobalOptions(editorConfig));
        enabledSettings.MapSourceFields.Should().Be(BooleanSetting.Enable);
        enabledSettings.MapTargetFields.Should().Be(BooleanSetting.Disable);
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
                                    mappa.mapsourcefields = enable
                                    mappa.maptargetfields = enable
                                    """;
        var settings = new MappaUserSettings(CreateGlobalOptions(editorConfig));

        using (settings.Apply(new MappaSettingsAttribute
        {
            MapSourceFields = BooleanSetting.Disable,
            MapTargetFields = BooleanSetting.Disable,
        }))
        {
            settings.MapSourceFields.Should().Be(BooleanSetting.Disable);
            settings.MapTargetFields.Should().Be(BooleanSetting.Disable);

            using (settings.Apply(new MappaSettingsAttribute()))
            {
                settings.MapSourceFields.Should().Be(BooleanSetting.Disable);
                settings.MapTargetFields.Should().Be(BooleanSetting.Disable);
            }

            using (settings.Apply(new MappaSettingsAttribute
            {
                MapSourceFields = BooleanSetting.Enable,
                MapTargetFields = BooleanSetting.Enable,
            }))
            {
                settings.MapSourceFields.Should().Be(BooleanSetting.Enable);
                settings.MapTargetFields.Should().Be(BooleanSetting.Enable);
            }

            settings.MapSourceFields.Should().Be(BooleanSetting.Disable);
            settings.MapTargetFields.Should().Be(BooleanSetting.Disable);
        }

        settings.MapSourceFields.Should().Be(BooleanSetting.Enable);
        settings.MapTargetFields.Should().Be(BooleanSetting.Enable);
    }

    private static MappaGlobalOptions CreateGlobalOptions(string editorConfig)
        => new(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken));
}