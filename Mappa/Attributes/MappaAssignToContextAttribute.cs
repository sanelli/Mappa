// <copyright file="MappaAssignToContextAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instruct the Mappa source generator to store the value of a target property or field
/// in the input context after the target object has been fully constructed.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaAssignToContextAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignToContextAttribute"/> class.
    /// </summary>
    /// <param name="contextKey">The key of the context entry to assign.</param>
    /// <param name="targetPropertyName">The name of the target property or field whose value is stored in the context.</param>
    public MappaAssignToContextAttribute(string contextKey, string targetPropertyName)
    {
        this.ContextKey = contextKey;
        this.TargetPropertyName = targetPropertyName;
    }

    /// <summary>
    /// Gets the key of the context entry to assign.
    /// </summary>
    public string ContextKey { get; }

    /// <summary>
    /// Gets the name of the target property or field whose value is stored in the context.
    /// </summary>
    public string TargetPropertyName { get; }
}