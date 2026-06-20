// <copyright file="DateTimeStylesMappaSettingsTestHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Shared test data for date/time <see cref="Mappa.Attributes.MappaSettingsAttribute"/> style settings.
/// </summary>
internal static class DateTimeStylesMappaSettingsTestHelper
{
    /// <summary>
    /// Returns test data for date/time type mapping tests.
    /// </summary>
    /// <returns>Target type, style property name, editorconfig key, and default format.</returns>
    public static IEnumerable<object[]> DateTimeTypeTestData()
    {
        yield return [typeof(DateTime), "DateTimeStyle", "datetimestyle", "d"];
        yield return [typeof(DateTimeOffset), "DateTimeOffsetStyle", "datetimeoffsetstyle", "d"];
        yield return [typeof(DateOnly), "DateOnlyStyle", "dateonlystyle", "d"];
        yield return [typeof(TimeOnly), "TimeOnlyStyle", "timeonlystyle", "t"];
    }
}