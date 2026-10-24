// <copyright file="MappaUserSettingsReplaceNullWithEmptyContainerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUserSettings.ReplaceNullWithEmptyContainer"/>.
/// </summary>
public sealed class MappaUserSettingsReplaceNullWithEmptyContainerTests
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
        undefinedSettings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Undefined);

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.replacenullwithemptycontainer = enable
                                    """;
        var enabledSettings = new MappaUserSettings(CreateGlobalOptions(editorConfig));
        enabledSettings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Enable);
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
                                    mappa.replacenullwithemptycontainer = enable
                                    """;
        var settings = new MappaUserSettings(CreateGlobalOptions(editorConfig));

        using (settings.Apply(new MappaSettingsAttribute { ReplaceNullWithEmptyContainer = BooleanSetting.Disable }))
        {
            settings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Disable);

            using (settings.Apply(new MappaSettingsAttribute()))
            {
                settings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Disable);
            }

            using (settings.Apply(new MappaSettingsAttribute { ReplaceNullWithEmptyContainer = BooleanSetting.Enable }))
            {
                settings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Enable);
            }

            settings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Disable);
        }

        settings.ReplaceNullWithEmptyContainer.Should().Be(BooleanSetting.Enable);
    }

    private static MappaGlobalOptions CreateGlobalOptions(string editorConfig)
        => new(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken));
}