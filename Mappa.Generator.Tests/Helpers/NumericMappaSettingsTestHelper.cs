// <copyright file="NumericMappaSettingsTestHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Shared test data for numeric <see cref="Mappa.Attributes.MappaSettingsAttribute"/> format settings.
/// </summary>
internal static class NumericMappaSettingsTestHelper
{
    /// <summary>
    /// Returns test data for numeric type mapping tests.
    /// </summary>
    /// <returns>Alias, full type name, format property name, and editorconfig key.</returns>
    public static IEnumerable<object[]> NumericTypeTestData()
    {
        yield return ["sbyte", typeof(sbyte).ToString(), "SByteFormat", "sbyteformat"];
        yield return ["byte", typeof(byte).ToString(), "ByteFormat", "byteformat"];
        yield return ["short", typeof(short).ToString(), "ShortFormat", "shortformat"];
        yield return ["ushort", typeof(ushort).ToString(), "UShortFormat", "ushortformat"];
        yield return ["int", typeof(int).ToString(), "IntFormat", "intformat"];
        yield return ["uint", typeof(uint).ToString(), "UIntFormat", "uintformat"];
        yield return ["long", typeof(long).ToString(), "LongFormat", "longformat"];
        yield return ["ulong", typeof(ulong).ToString(), "ULongFormat", "ulongformat"];
        yield return ["float", typeof(float).ToString(), "FloatFormat", "floatformat"];
        yield return ["double", typeof(double).ToString(), "DoubleFormat", "doubleformat"];
        yield return ["decimal", typeof(decimal).ToString(), "DecimalFormat", "decimalformat"];
    }
}