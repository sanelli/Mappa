// <copyright file="IndentStringBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text;

namespace Mappa.Generator.Helpers;

/// <summary>
/// String builder that keep into account indentation.
/// </summary>
internal sealed class IndentStringBuilder
{
    /// <summary>
    /// Gets the number of spaces per indentation.
    /// </summary>
    private const int SpacesPerIndentation = 3;

    /// <summary>
    /// The buffer used to store the final string.
    /// </summary>
    private readonly StringBuilder buffer = new();

    /// <summary>
    /// The indentation.
    /// </summary>
    private int indent;

    /// <summary>
    /// Gets the overall number of spaces of the indentation.
    /// </summary>
    private string Spaces => new(' ', this.indent * SpacesPerIndentation);

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.buffer.ToString().TrimEnd();
    }

    /// <summary>
    /// Append the line to the the internal buffer.
    /// </summary>
    /// <param name="line">The line to be appended.</param>
    /// <returns>This object.</returns>
    /// <remarks>If the line contains multiple lines is split and properly indented.</remarks>
    internal IndentStringBuilder AppendLine(string line)
        => this.AppendLines(line
            .Split(new[] { "\n" }, StringSplitOptions.None)
            .Select(subLine => subLine.TrimEnd()));

    /// <summary>
    /// Append an empty line to the buffer.
    /// </summary>
    /// <returns>This object.</returns>
    internal IndentStringBuilder AppendEmptyLine()
    {
        this.buffer.AppendLine();
        return this;
    }

    /// <summary>
    /// Start indentation.
    /// </summary>
    /// <returns>A disposable that reduce the indentation upon disposing.</returns>
    internal IDisposable Indent()
        => new Indentation(this);

    /// <summary>
    /// Begin a new code block.
    /// </summary>
    /// <param name="addSemicolonAfterClose"><c>true</c> if a semicolon should be added when the block is being closed..</param>
    /// <returns>A disposable object that closes the block upon dispose.</returns>
    internal IDisposable CodeBlock(bool addSemicolonAfterClose = false)
        => new CodeBlockDefinition(this, addSemicolonAfterClose);

    /// <summary>
    /// Generate a surrounding <c>#nullable</c> block.
    /// </summary>
    /// <param name="isNullableEnabled"><c>true</c> if nullable.</param>
    /// <returns>A disposable object that closes the block upon dispose.</returns>
    internal IDisposable NullableBlock(bool isNullableEnabled)
        => new NullableBlockDefinition(this, isNullableEnabled);

    /// <summary>
    /// Generate a surrounding <c>#pragma warning</c> block.
    /// </summary>
    /// <param name="warnings">Optional list of warnings.</param>
    /// <returns>A disposable object that closes the block upon dispose.</returns>
    internal IDisposable PragmaWarningDisableBlock(string warnings = "")
        => new PragmaWarningBlockDefinition(this, warnings);

    /// <summary>
    /// Append the lines to the the internal buffer.
    /// </summary>
    /// <param name="lines">The lines to be appended.</param>
    /// <returns>This object.</returns>
    private IndentStringBuilder AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            this.buffer.AppendLine($"{this.Spaces}{line.TrimEnd()}");
        }

        return this;
    }

    /// <summary>
    /// Increase the indentation.
    /// </summary>
    private void IncreaseIndentation() => ++this.indent;

    /// <summary>
    /// Decrease the indentation.
    /// </summary>
    private void DecreaseIndentation() => --this.indent;

    /// <summary>
    /// Describe a code block that is constructed using the
    /// <see cref="IDisposable"/> pattern.
    /// </summary>
    private sealed class CodeBlockDefinition
        : IDisposable
    {
        /// <summary>
        /// The string builder.
        /// </summary>
        private readonly IndentStringBuilder stringBuilder;

        /// <summary>
        /// <c>true</c> if a semicolon should be added when closing
        /// the block.
        /// </summary>
        private readonly bool addSemicolonAfterClose;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockDefinition"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="addSemicolonAfterClose"><c>true</c> if a semicolon should be added when closing the block.</param>
        internal CodeBlockDefinition(IndentStringBuilder stringBuilder, bool addSemicolonAfterClose)
        {
            this.stringBuilder = stringBuilder;
            this.addSemicolonAfterClose = addSemicolonAfterClose;
            this.stringBuilder.AppendLine("{");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.AppendLine(this.addSemicolonAfterClose ? "};" : "}");
        }
    }

    /// <summary>
    /// Describe the current indentation block and allow for automatic
    /// indentation to terminate using the <see cref="IDisposable"/>
    /// pattern.
    /// </summary>
    private sealed class Indentation
        : IDisposable
    {
        /// <summary>
        /// The indentation object.
        /// </summary>
        private readonly IndentStringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Indentation"/> class.
        /// </summary>
        /// <param name="stringBuilder">The indentation object.</param>
        internal Indentation(IndentStringBuilder stringBuilder)
        {
            this.stringBuilder = stringBuilder;
            this.stringBuilder.IncreaseIndentation();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.DecreaseIndentation();
        }
    }

    /// <summary>
    /// Describe a nullability pragma block that will be closed
    /// using the <see cref="IDisposable"/> pattern.
    /// </summary>
    private sealed class NullableBlockDefinition
        : IDisposable
    {
        /// <summary>
        /// The indentation object.
        /// </summary>
        private readonly IndentStringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableBlockDefinition"/> class.
        /// </summary>
        /// <param name="stringBuilder">The stirng builder.</param>
        /// <param name="nullable"><c>true</c> if nullable should be enabled.</param>
        internal NullableBlockDefinition(IndentStringBuilder stringBuilder, bool nullable)
        {
            this.stringBuilder = stringBuilder;
            this.stringBuilder.AppendLine($"#nullable {(nullable ? "enable" : "disable")}");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.AppendLine("#nullable restore");
        }
    }

    /// <summary>
    /// Describe a warning disable pragma block that will be closed (i.e. restored)
    /// using the <see cref="IDisposable"/> pattern.
    /// </summary>
    private sealed class PragmaWarningBlockDefinition
        : IDisposable
    {
        /// <summary>
        /// The indentation object.
        /// </summary>
        private readonly IndentStringBuilder stringBuilder;

        /// <summary>
        /// The warnings disabled.
        /// </summary>
        private readonly string warnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PragmaWarningBlockDefinition"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="warnings">The warnings to disable.</param>
        internal PragmaWarningBlockDefinition(IndentStringBuilder stringBuilder, string warnings = "")
        {
            this.stringBuilder = stringBuilder;
            this.warnings = string.IsNullOrWhiteSpace(warnings) ? string.Empty : $" {warnings}";
            this.stringBuilder.AppendLine($"#pragma warning disable{this.warnings}");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.AppendLine($"#pragma warning restore{this.warnings}");
        }
    }
}