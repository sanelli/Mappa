// <copyright file="Indent.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

namespace Mappa.Generator.Models.Helpers;

/// <summary>
/// Describe the indentation.
/// </summary>
internal sealed class Indent
{
    /// <summary>
    /// Gets the number of spaces per indentation.
    /// </summary>
    private const int Spaces = 3;

    /// <summary>
    /// Gets the current indentation size.
    /// </summary>
    internal int Size { get; private set; }

    /// <summary>
    /// Begin a new indentation.
    /// </summary>
    /// <returns>An object to dispose to close the identation.</returns>
    internal IDisposable Begin()
        => new Indentation(this);

    /// <summary>
    /// Gets the overall number of spaces to be added.
    /// </summary>
    /// <returns>The over number of spaces representing this indentation.</returns>
    internal string GetSpaces() => new string(' ', this.Size * Spaces);

    /// <summary>
    /// Increase the indenttion.
    /// </summary>
    private void Incrase() => ++this.Size;

    /// <summary>
    /// Decrease the indentation.
    /// </summary>
    private void Decrease() => --this.Size;

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
        private readonly Indent indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Indentation"/> class.
        /// </summary>
        /// <param name="indent">The indentation object.</param>
        internal Indentation(Indent indent)
        {
            this.indent = indent;
            this.indent.Incrase();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.indent.Decrease();
        }
    }
}