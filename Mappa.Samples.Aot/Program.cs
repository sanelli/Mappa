// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot;

/// <summary>
/// Program class.
/// </summary>
// TODO [#41] Add all remaining classes from Mappa.Samples.
public static class Program
{
    /// <summary>
    /// Entrypoint.
    /// </summary>
    public static void Main()
    {
        // -- ArrayOrListToArrayMapper --
        var arrayOrListToArrayMapper = new ArrayOrListToArrayMapper();
        Printout($"-- {nameof(ArrayOrListToArrayMapper)} --");
        Printout(arrayOrListToArrayMapper.Map(new[] { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToArrayMapper.Map(new CountingValues?[] { CountingValues.One, null, CountingValues.Two }));
        Printout(arrayOrListToArrayMapper.Map(new List<CountingValues> { CountingValues.One, CountingValues.Two }));
        Printout(arrayOrListToArrayMapper.Map((IList<CountingValues>)new List<CountingValues> { CountingValues.One, CountingValues.Two }));
    }

    private static void Printout(string s)
        => Console.WriteLine(s);

    private static void Printout<T>(IEnumerable<T> enumerable)
        => Console.WriteLine(JoinToString(enumerable));

    private static string JoinToString<T>(IEnumerable<T> enumerable)
        => string.Join(", ", enumerable);
}