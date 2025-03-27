// <copyright file="TargetClassWithPrivateSetterPropertyCollections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target class for <see cref="SourceClassWithCollections"/>.
/// </summary>
public sealed class TargetClassWithPrivateSetterPropertyCollections
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassWithPrivateSetterPropertyCollections"/> class.
    /// </summary>
    public TargetClassWithPrivateSetterPropertyCollections()
    {
        this.PropertyA = new List<string>();
        this.PropertyB = new List<string>();
        this.PropertyC = new List<string>();
        this.PropertyD = new List<string>();
        this.PropertyE = new List<string>();
        this.PropertyF = new List<string>();
        this.PropertyG = new Dictionary<string, int>();
        this.PropertyH = new Dictionary<string, int>();
        this.PropertyI = new();
        this.PropertyJ = new();
    }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public ICollection<string> PropertyA { get; private set; }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public ICollection<string> PropertyB { get; private set; }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public IList<string> PropertyC { get; private set; }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public IList<string> PropertyD { get; private set; }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public ICollection<string> PropertyE { get; private set; }

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public ICollection<string> PropertyF { get; private set; }

    /// <summary>
    /// Gets a dictionary property.
    /// </summary>
    public Dictionary<string, int> PropertyG { get; private set; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey,TValue}"/> property.
    /// </summary>
    public IDictionary<string, int> PropertyH { get; private set; }

    /// <summary>
    /// Gets a <see cref="CustomCollectionImplementingExplicitlyICollection{T}"/> property.
    /// </summary>
    public CustomCollectionImplementingExplicitlyICollection<string> PropertyI { get; }

    /// <summary>
    /// Gets a <see cref="CustomCollectionImplementingExplicitlyICollectionOfStrings"/> property.
    /// </summary>
    public CustomCollectionImplementingExplicitlyICollectionOfStrings PropertyJ { get; }
}