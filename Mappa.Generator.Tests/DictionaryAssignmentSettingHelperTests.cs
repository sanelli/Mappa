// <copyright file="DictionaryAssignmentSettingHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Generator.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="DictionaryAssignmentSettingHelper"/>.
/// </summary>
public sealed class DictionaryAssignmentSettingHelperTests
{
    /// <summary>
    /// Test <see cref="DictionaryAssignmentSettingHelper.GetEffective"/> returns <see cref="DictionaryAssignmentSetting.Indexer"/>
    /// when the configured setting is <see cref="DictionaryAssignmentSetting.Undefined"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetEffectiveReturnsIndexerWhenSettingIsUndefined()
    {
        DictionaryAssignmentSettingHelper.GetEffective(DictionaryAssignmentSetting.Undefined)
            .Should().Be(DictionaryAssignmentSetting.Indexer);
    }

    /// <summary>
    /// Test <see cref="DictionaryAssignmentSettingHelper.GetEffective"/> returns the configured setting when it is defined.
    /// </summary>
    /// <param name="setting">The configured setting.</param>
    [Theory]
    [InlineData(DictionaryAssignmentSetting.Indexer)]
    [InlineData(DictionaryAssignmentSetting.Add)]
    [UnitTest]
    public void GetEffectiveReturnsConfiguredSettingWhenDefined(DictionaryAssignmentSetting setting)
    {
        DictionaryAssignmentSettingHelper.GetEffective(setting)
            .Should().Be(setting);
    }
}