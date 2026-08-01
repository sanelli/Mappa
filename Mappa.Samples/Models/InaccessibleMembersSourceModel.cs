// <copyright file="InaccessibleMembersSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model demonstrating inaccessible (private) source properties.
/// </summary>
public sealed class InaccessibleMembersSourceModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InaccessibleMembersSourceModel"/> class.
    /// </summary>
    /// <param name="name">The private name value.</param>
    /// <param name="age">The public age value.</param>
    public InaccessibleMembersSourceModel(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    /// <summary>
    /// Gets the age.
    /// </summary>
    public int Age { get; }

    /// <summary>
    /// Gets the name (private getter; accessed via UnsafeAccessor when opted in).
    /// </summary>
    private string Name { get; }
}