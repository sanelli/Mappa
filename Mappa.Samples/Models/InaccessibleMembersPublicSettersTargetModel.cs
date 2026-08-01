// <copyright file="InaccessibleMembersPublicSettersTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with a private constructor and public property setters.
/// </summary>
#pragma warning disable S3453 // Private constructor is intentional for the inaccessible-members sample
public sealed class InaccessibleMembersPublicSettersTargetModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InaccessibleMembersPublicSettersTargetModel"/> class.
    /// </summary>
    private InaccessibleMembersPublicSettersTargetModel()
    {
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }
}
#pragma warning restore S3453