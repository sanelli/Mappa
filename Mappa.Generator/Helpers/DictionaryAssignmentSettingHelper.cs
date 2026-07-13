// <copyright file="DictionaryAssignmentSettingHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helper methods for <see cref="DictionaryAssignmentSetting"/>.
/// </summary>
internal static class DictionaryAssignmentSettingHelper
{
    /// <summary>
    /// Gets the effective dictionary assignment setting.
    /// </summary>
    /// <param name="setting">The configured setting.</param>
    /// <returns>The effective setting.</returns>
    internal static DictionaryAssignmentSetting GetEffective(DictionaryAssignmentSetting setting)
        => setting is DictionaryAssignmentSetting.Undefined
            ? DictionaryAssignmentSetting.Indexer
            : setting;
}