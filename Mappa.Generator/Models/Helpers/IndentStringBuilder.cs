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
    private StringBuilder Buffer { get; } = new();

    private Indent Indenter { get; } = new();

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Buffer.ToString().TrimEnd();
    }

    /// <summary>
    /// Append the line to the the internal buffer.
    /// </summary>
    /// <param name="line">The line to be appended.</param>
    /// <returns>This object.</returns>
    /// <remarks>If the line contains multiple lines is split and properly indented.</remarks>
    internal IndentStringBuilder Append(string line)
        => this.Append(line.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

    /// <summary>
    /// Append the lines to the the internal buffer.
    /// </summary>
    /// <param name="lines">The lines to be appended.</param>
    /// <returns>This object.</returns>
    internal IndentStringBuilder Append(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            this.Buffer.AppendLine($"{this.Indenter.GetSpaces()}{line.TrimEnd()}");
        }

        return this;
    }

    /// <summary>
    /// Start indentation.
    /// </summary>
    /// <returns>A disposable object to be disposed to stop disposing.</returns>
    internal IDisposable Indent()
        => this.Indenter.Begin();
}