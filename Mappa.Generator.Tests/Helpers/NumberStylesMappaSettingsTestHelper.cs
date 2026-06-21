// <copyright file="NumberStylesMappaSettingsTestHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Shared test data for numeric <see cref="Mappa.Attributes.MappaSettingsAttribute"/> style settings.
/// </summary>
internal static class NumberStylesMappaSettingsTestHelper
{
    /// <summary>
    /// Returns test data for numeric type mapping tests.
    /// </summary>
    /// <returns>Type alias, full type name, style property name, and editorconfig key.</returns>
    public static IEnumerable<object[]> NumericTypeTestData()
    {
        yield return ["sbyte", typeof(sbyte).ToString(), "SByteStyle", "sbytestyle"];
        yield return ["byte", typeof(byte).ToString(), "ByteStyle", "bytestyle"];
        yield return ["short", typeof(short).ToString(), "ShortStyle", "shortstyle"];
        yield return ["ushort", typeof(ushort).ToString(), "UShortStyle", "ushortstyle"];
        yield return ["int", typeof(int).ToString(), "IntStyle", "intstyle"];
        yield return ["uint", typeof(uint).ToString(), "UIntStyle", "uintstyle"];
        yield return ["long", typeof(long).ToString(), "LongStyle", "longstyle"];
        yield return ["ulong", typeof(ulong).ToString(), "ULongStyle", "ulongstyle"];
        yield return ["float", typeof(float).ToString(), "FloatStyle", "floatstyle"];
        yield return ["double", typeof(double).ToString(), "DoubleStyle", "doublestyle"];
        yield return ["decimal", typeof(decimal).ToString(), "DecimalStyle", "decimalstyle"];
    }
}