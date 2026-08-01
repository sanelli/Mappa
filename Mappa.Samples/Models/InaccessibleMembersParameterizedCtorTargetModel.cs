// <copyright file="InaccessibleMembersParameterizedCtorTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model constructed via a private parameterized constructor.
/// </summary>
#pragma warning disable S3453 // Private constructor is intentional for the inaccessible-members sample
#pragma warning disable S1144 // Private constructor is invoked via UnsafeAccessor by the mapper
public sealed class InaccessibleMembersParameterizedCtorTargetModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InaccessibleMembersParameterizedCtorTargetModel"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="age">The age.</param>
    private InaccessibleMembersParameterizedCtorTargetModel(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the age.
    /// </summary>
    public int Age { get; }
}
#pragma warning restore S1144
#pragma warning restore S3453