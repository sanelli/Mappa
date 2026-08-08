// <copyright file="GeneratedSourceDumpHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text;

using Xunit.v3;

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Dumps successfully generated source from generator integration tests to disk.
/// </summary>
internal static class GeneratedSourceDumpHelper
{
    /// <summary>
    /// The dump folder name under the repository root.
    /// </summary>
    internal const string DumpFolderName = ".mappa-generator-tests-dump";

    private const string VersionTargetsFileName = "MappaVersion.targets";
    private const string InvocationCounterKey = "Mappa.Generator.Tests.GeneratedSourceDump.InvocationCounter";
    private const int MaxFileNameLength = 200;

    /// <summary>
    /// Attempts to dump generated sources when the generator produced at least one source.
    /// </summary>
    /// <param name="driver">The generator driver after a run.</param>
    /// <param name="dumpDirectoryOverride">Optional dump directory (for unit tests).</param>
    internal static void TryDumpGeneratedSources(GeneratorDriver driver, string? dumpDirectoryOverride = null)
    {
        try
        {
            var generatedSources = CollectGeneratedSources(driver);
            if (generatedSources.Count == 0)
            {
                return;
            }

            var dumpDirectory = dumpDirectoryOverride ?? ResolveDumpDirectory();
            if (string.IsNullOrWhiteSpace(dumpDirectory))
            {
                return;
            }

            Directory.CreateDirectory(dumpDirectory);

            var (className, methodName, theoryArguments) = GetTestIdentity();
            var fileName = BuildDumpFileName(className, methodName, theoryArguments, NextInvocationIndex());
            var content = BuildDumpContent(generatedSources);
            File.WriteAllText(Path.Combine(dumpDirectory, fileName), content);
        }
#pragma warning disable CA1031
        catch (Exception exception)
#pragma warning restore CA1031
        {
            TestContext.Current.SendDiagnosticMessage(
                "Failed to dump generated Mappa sources: {0}",
                exception.Message);
        }
    }

    /// <summary>
    /// Builds a filesystem-safe dump file name.
    /// </summary>
    /// <param name="className">The test class name.</param>
    /// <param name="methodName">The test method name.</param>
    /// <param name="theoryArguments">Optional theory arguments.</param>
    /// <param name="invocationIndex">The 1-based dump invocation index within the test.</param>
    /// <returns>The dump file name.</returns>
    internal static string BuildDumpFileName(
        string? className,
        string? methodName,
        object?[]? theoryArguments,
        int invocationIndex)
    {
        var classPart = SanitizeFileNameFragment(
            string.IsNullOrWhiteSpace(className) ? "UnknownClass" : className);
        var methodPart = SanitizeFileNameFragment(
            string.IsNullOrWhiteSpace(methodName) ? "UnknownMethod" : methodName);
        var name = $"{classPart}_{methodPart}";

        if (theoryArguments is { Length: > 0 })
        {
            var parametersPart = string.Join(
                "_",
                theoryArguments.Select(FormatTheoryArgument));
            name = $"{name}_{SanitizeFileNameFragment(parametersPart)}";
        }

        if (invocationIndex > 1)
        {
            name = $"{name}_{invocationIndex.ToString(CultureInfo.InvariantCulture)}";
        }

        const string suffix = ".g.cs";
        if (name.Length + suffix.Length > MaxFileNameLength)
        {
            name = name[..(MaxFileNameLength - suffix.Length)];
        }

        return name + suffix;
    }

    /// <summary>
    /// Sanitizes a value for use as a file-name fragment.
    /// </summary>
    /// <param name="value">The value to sanitize.</param>
    /// <returns>A filesystem-safe fragment.</returns>
    internal static string SanitizeFileNameFragment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "empty";
        }

        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (IsUnsafeFileNameCharacter(character))
            {
                builder.Append('_');
            }
            else
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formats a theory argument for inclusion in a dump file name.
    /// </summary>
    /// <param name="argument">The theory argument.</param>
    /// <returns>A string representation of the argument.</returns>
    internal static string FormatTheoryArgument(object? argument)
    {
        return argument switch
        {
            null => "null",
            string text => text,
            bool boolean => boolean ? "True" : "False",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? "null",
            _ => argument.ToString() ?? "null",
        };
    }

    /// <summary>
    /// Resolves the dump directory under the repository root.
    /// </summary>
    /// <returns>The dump directory path, or <see langword="null"/> when the repository root cannot be found.</returns>
    internal static string? ResolveDumpDirectory()
    {
        var repositoryRoot = ResolveRepositoryRoot();
        return repositoryRoot is null
            ? null
            : Path.Combine(repositoryRoot, DumpFolderName);
    }

    private static bool IsUnsafeFileNameCharacter(char character)
    {
        if (character is ' ' or '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*')
        {
            return true;
        }

        return Path.GetInvalidFileNameChars().Contains(character);
    }

    private static string? ResolveRepositoryRoot()
    {
        foreach (var startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(startPath);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, VersionTargetsFileName)))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        return null;
    }

    private static List<(string HintName, string Source)> CollectGeneratedSources(GeneratorDriver driver)
    {
        var sources = new List<(string HintName, string Source)>();
        foreach (var result in driver.GetRunResult().Results)
        {
            foreach (var generatedSource in result.GeneratedSources)
            {
                sources.Add((generatedSource.HintName, generatedSource.SourceText.ToString()));
            }
        }

        return sources;
    }

    private static string BuildDumpContent(List<(string HintName, string Source)> generatedSources)
    {
        if (generatedSources.Count == 1)
        {
            return generatedSources[0].Source;
        }

        var builder = new StringBuilder();
        for (var index = 0; index < generatedSources.Count; index++)
        {
            if (index > 0)
            {
                builder.AppendLine();
                builder.AppendLine();
            }

            var (hintName, source) = generatedSources[index];
            builder.Append("// HintName: ");
            builder.AppendLine(hintName);
            builder.Append(source);
        }

        return builder.ToString();
    }

    private static (string ClassName, string MethodName, object?[]? TheoryArguments) GetTestIdentity()
    {
        var testCase = TestContext.Current.TestCase;
        var className = testCase?.TestClassSimpleName ?? "UnknownClass";
        var methodName = testCase?.TestMethodName ?? "UnknownMethod";
        object?[]? theoryArguments = null;
        if (TestContext.Current.Test is IXunitTest xunitTest)
        {
            theoryArguments = xunitTest.TestMethodArguments;
        }

        return (className, methodName, theoryArguments);
    }

    private static int NextInvocationIndex()
    {
        try
        {
            var storage = TestContext.Current.KeyValueStorage;
            var next = 1;
            if (storage.TryGetValue(InvocationCounterKey, out var existing) && existing is int current)
            {
                next = current + 1;
            }

            storage[InvocationCounterKey] = next;
            return next;
        }
        catch (InvalidOperationException)
        {
            return 1;
        }
    }
}