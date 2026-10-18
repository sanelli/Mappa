// <copyright file="MappaUserSettingsProjectionOverridesTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUserSettings.ApplyProjectionOverrides"/>.
/// </summary>
public sealed class MappaUserSettingsProjectionOverridesTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test projection overrides replace settings when set and fall back when unset.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ApplyProjectionOverridesAppliesDefinedValuesAndFallsBackWhenUnset()
    {
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: TestContext.Current.CancellationToken));
        var settings = new MappaUserSettings(globalOptions);

        using (settings.Apply(new MappaSettingsAttribute
               {
                   GuidFormat = "D",
                   CultureInfoSetting = CultureInfoSetting.CurrentCulture,
                   CultureName = "en-US",
                   CaseInsensitiveEnumMap = BooleanSetting.Disable,
                   EnumStringMapSetting = EnumStringMapSetting.MemberName,
                   EnumToEnumMapSetting = EnumToEnumMapSetting.MemberName,
               }))
        {
            using (settings.ApplyProjectionOverrides(null))
            {
                settings.GuidFormat.Should().Be("D");
            }

            using (settings.ApplyProjectionOverrides(new MappaSettingsAttribute
                   {
                       ProjectionGuidFormat = "N",
                       ProjectionCultureInfoSetting = CultureInfoSetting.InvariantCulture,
                       ProjectionCultureName = "en-GB",
                       ProjectionCaseInsensitiveEnumMap = BooleanSetting.Enable,
                       ProjectionEnumStringMapSetting = EnumStringMapSetting.Description,
                       ProjectionEnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue,
                   }))
            {
                settings.GuidFormat.Should().Be("N");
                settings.CultureInfoSetting.Should().Be(CultureInfoSetting.InvariantCulture);
                settings.CultureName.Should().Be("en-GB");
                settings.CaseInsensitiveEnumMap.Should().Be(BooleanSetting.Enable);
                settings.EnumStringMapSetting.Should().Be(EnumStringMapSetting.Description);
                settings.EnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.NumericValue);
            }

            using (settings.ApplyProjectionOverrides(new MappaSettingsAttribute()))
            {
                settings.GuidFormat.Should().Be("D");
                settings.CultureInfoSetting.Should().Be(CultureInfoSetting.CurrentCulture);
                settings.CultureName.Should().Be("en-US");
                settings.CaseInsensitiveEnumMap.Should().Be(BooleanSetting.Disable);
                settings.EnumStringMapSetting.Should().Be(EnumStringMapSetting.MemberName);
                settings.EnumToEnumMapSetting.Should().Be(EnumToEnumMapSetting.MemberName);
            }
        }
    }
}