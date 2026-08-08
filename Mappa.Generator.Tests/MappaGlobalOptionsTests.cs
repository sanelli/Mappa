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
                                    mappa.dictionaryassignment = Add
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
        options.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Add);
    }

    /// <summary>
    /// Test <see cref="MappaGlobalOptions"/> parses <c>mappa.dictionaryassignment</c> values.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParsesDictionaryAssignmentFromEditorConfig()
    {
        const string undefinedEditorConfig = """
                                             root = true

                                             [*.cs]
                                             mappa.dictionaryassignment = Undefined
                                             """;

        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var undefinedOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(undefinedEditorConfig),
            compilation.SyntaxTrees[0]);
        undefinedOptions.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Undefined);

        const string invalidEditorConfig = """
                                           root = true

                                           [*.cs]
                                           mappa.dictionaryassignment = NotAValidValue
                                           """;

        var invalidOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(invalidEditorConfig),
            compilation.SyntaxTrees[0]);
        invalidOptions.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Indexer);

        const string indexerEditorConfig = """
                                           root = true

                                           [*.cs]
                                           mappa.dictionaryassignment = Indexer
                                           """;

        var indexerOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(indexerEditorConfig),
            compilation.SyntaxTrees[0]);
        indexerOptions.DictionaryAssignment.Should().Be(DictionaryAssignmentSetting.Indexer);
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
        options.ReferenceReusing.Should().Be(BooleanSetting.Undefined);
        options.MaxRuntimeDepth.Should().Be((short)0);
        options.MaxCompileTimeDepth.Should().Be((short)50);
    }

    /// <summary>
    /// Test <see cref="MappaGlobalOptions"/> reads reference-handling settings from <c>.editorconfig</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReadsReferenceHandlingSettingsFromEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.referencereusing = enable
                                    mappa.maxruntimedepth = 7
                                    mappa.maxcompiletimedepth = 3
                                    """;

        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            compilation.SyntaxTrees[0]);

        options.ReferenceReusing.Should().Be(BooleanSetting.Enable);
        options.MaxRuntimeDepth.Should().Be((short)7);
        options.MaxCompileTimeDepth.Should().Be((short)3);
    }

    /// <summary>
    /// Test negative depth values in <c>.editorconfig</c> fall back to defaults.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TreatsNegativeDepthValuesInEditorConfigAsUnset()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxruntimedepth = -1
                                    mappa.maxcompiletimedepth = -5
                                    mappa.referencereusing = Undefined
                                    """;

        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig(editorConfig),
            compilation.SyntaxTrees[0]);

        options.ReferenceReusing.Should().Be(BooleanSetting.Undefined);
        options.MaxRuntimeDepth.Should().Be((short)0);
        options.MaxCompileTimeDepth.Should().Be((short)50);
    }
}