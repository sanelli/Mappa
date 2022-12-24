// <copyright file="IndentStringBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;
using System.Text;

namespace Mappa.Generator.Models.Helpers;

/// <summary>
/// String builder that keep into account indentation.
/// </summary>
internal sealed class IndentStringBuilder
{
    /// <summary>
    /// Gets the number of spaces per indentation.
    /// </summary>
    private const int SpacesPerIndetation = 3;

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
    internal string Spaces => new string(' ', this.indent * SpacesPerIndetation);

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
        => this.AppendLines(line.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

    /// <summary>
    /// Append the lines to the the internal buffer.
    /// </summary>
    /// <param name="lines">The lines to be appended.</param>
    /// <returns>This object.</returns>
    internal IndentStringBuilder AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            this.buffer.AppendLine($"{this.Spaces}{line.TrimEnd()}");
        }

        return this;
    }

    /// <summary>
    /// Start indentation.
    /// </summary>
    /// <returns>A disposable that reduce the indentation upon disposing.</returns>
    internal IDisposable Indent()
        => new Indentation(this);

    /// <summary>
    /// Begina a new code block.
    /// </summary>
    /// <returns>A disposable object that closes the block upon dispose.</returns>
    internal IDisposable BeginCodeBlock()
        => new CodeBlock(this);

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
    private sealed class CodeBlock
        : IDisposable
    {
        /// <summary>
        /// The string builder.
        /// </summary>
        private readonly IndentStringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlock"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        internal CodeBlock(IndentStringBuilder stringBuilder)
        {
            this.stringBuilder = stringBuilder;
            this.stringBuilder.AppendLine("{");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.AppendLine("}");
        }
    }

    /// <summary>
    /// Descrbe the current indentation and allow for automatic
    /// indentation to happen using the <see cref="IDisposable"/>
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
        /// <param name="indent">The indentation object.</param>
        internal Indentation(IndentStringBuilder indent)
        {
            this.stringBuilder = indent;
            this.stringBuilder.IncreaseIndentation();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stringBuilder.DecreaseIndentation();
        }
    }
}