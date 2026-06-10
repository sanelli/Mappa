// <copyright file="TestAnalyzerConfigOptionsProvider.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Mappa.Generator.Tests.Helpers;

/// <summary>
/// Provides analyzer configuration options for generator tests.
/// </summary>
internal sealed class TestAnalyzerConfigOptionsProvider
    : AnalyzerConfigOptionsProvider
{
    private readonly TestAnalyzerConfigOptions options;

    private TestAnalyzerConfigOptionsProvider(ImmutableDictionary<string, string> values)
    {
        this.options = new TestAnalyzerConfigOptions(values);
    }

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GlobalOptions
        => this.options;

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        => this.options;

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        => this.options;

    /// <summary>
    /// Creates a provider from simplified <c>.editorconfig</c> content.
    /// </summary>
    /// <param name="editorConfig">The <c>.editorconfig</c> content.</param>
    /// <returns>The analyzer configuration options provider.</returns>
    internal static TestAnalyzerConfigOptionsProvider FromEditorConfig(string editorConfig)
    {
        var values = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in editorConfig.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (line.StartsWith('[') || line.StartsWith("is_global", StringComparison.Ordinal) || line.StartsWith("root", StringComparison.Ordinal))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            values[key] = value;
        }

        return new TestAnalyzerConfigOptionsProvider(values.ToImmutable());
    }

    private sealed class TestAnalyzerConfigOptions
        : AnalyzerConfigOptions
    {
        private readonly ImmutableDictionary<string, string> values;

        internal TestAnalyzerConfigOptions(ImmutableDictionary<string, string> values)
        {
            this.values = values;
        }

        /// <inheritdoc/>
        public override bool TryGetValue(string key, out string value)
        {
            if (this.values.TryGetValue(key, out var storedValue))
            {
                value = storedValue;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}