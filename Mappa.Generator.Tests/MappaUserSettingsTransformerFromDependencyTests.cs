// <copyright file="MappaUserSettingsTransformerFromDependencyTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUserSettings.MappaTransformerFromDependency"/>.
/// </summary>
public sealed class MappaUserSettingsTransformerFromDependencyTests
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
        undefinedSettings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Undefined);

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.transformerfromdependency = enable
                                    """;
        var enabledSettings = new MappaUserSettings(CreateGlobalOptions(editorConfig));
        enabledSettings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
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
                                    mappa.transformerfromdependency = enable
                                    """;
        var settings = new MappaUserSettings(CreateGlobalOptions(editorConfig));

        using (settings.Apply(new MappaSettingsAttribute { MappaTransformerFromDependency = BooleanSetting.Disable }))
        {
            settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Disable);

            using (settings.Apply(new MappaSettingsAttribute()))
            {
                settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Disable);
            }

            using (settings.Apply(new MappaSettingsAttribute { MappaTransformerFromDependency = BooleanSetting.Enable }))
            {
                settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
            }

            settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Disable);
        }

        settings.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
    }

    private static MappaGlobalOptions CreateGlobalOptions(string editorConfig)
        => new(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken));
}