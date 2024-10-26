// <copyright file="MappaAssignFromContextAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instruct the Mappa source generator to set the property value
/// from the input context.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaAssignFromContextAttribute
    : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromContextAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The target property name involved in the mapping.</param>
    /// <param name="itemName">The name of the item to use.</param>
    public MappaAssignFromContextAttribute(string targetPropertyName, string itemName)
    {
        this.TargetPropertyName = targetPropertyName;
        this.ItemName = itemName;
    }

    /// <inheritdoc/>
    public string TargetPropertyName { get; }

    /// <summary>
    /// Gets the name of the items to be assigned to <see cref="TargetPropertyName"/>.
    /// </summary>
    public string ItemName { get; }
}