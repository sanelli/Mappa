// <copyright file="MappaGlobalOptionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaGlobalOptions"/>.
/// </summary>
public sealed class MappaGlobalOptionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaGlobalOptions"/> reads rarely combined settings from <c>.editorconfig</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReadsRarelyCombinedSettingsFromEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.sbyteformat = sb
                                    mappa.ushortformat = us
                                    mappa.uintformat = ui
                                    mappa.ulongformat = ul
                                    mappa.globalnumberstyle = AllowThousands
                                    mappa.sbytestyle = AllowLeadingSign
                                    mappa.cultureinfosettings = UserDefined
                                    mappa.culturename = de-DE
                                    mappa.pragmawarning = disable
                                    """;

        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            compilation.SyntaxTrees[0]);

        options.SByteFormat.Should().Be("sb");
        options.UShortFormat.Should().Be("us");
        options.UIntFormat.Should().Be("ui");
        options.ULongFormat.Should().Be("ul");
        options.GlobalNumberStyle.Should().Be(NumberStyles.AllowThousands);
        options.SByteStyle.Should().Be(NumberStyles.AllowLeadingSign);
        options.CultureInfoSetting.Should().Be(CultureInfoSetting.UserDefined);
        options.CultureName.Should().Be("de-DE");
        options.PragmaWarning.Should().Be(PragmaWarningSetting.Disable);
    }

    /// <summary>
    /// Test <see cref="MappaGlobalOptions"/> leaves unset options at their defaults.
    /// </summary>
    [Fact]
    [UnitTest]
    public void LeavesUnsetOptionsAtDefaults()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        options.SByteFormat.Should().BeNull();
        options.CultureInfoSetting.Should().Be(CultureInfoSetting.None);
        options.CultureName.Should().BeNull();
        options.MappaDebug.Should().BeFalse();
        options.MappaDebugComments.Should().BeFalse();
        options.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Indexer);
    }
}