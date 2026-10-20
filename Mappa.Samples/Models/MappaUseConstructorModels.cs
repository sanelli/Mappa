// <copyright file="MappaUseConstructorModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

using System.Globalization;

namespace Mappa.Samples.Models;

/// <summary>
/// Source model for <see cref="MappaUseConstructorMapper"/> samples.
/// </summary>
public sealed class UseConstructorSource
{
    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public int PropertyA { get; set; }

    /// <summary>
    /// Gets or sets property B.
    /// </summary>
    public string PropertyB { get; set; } = string.Empty;
}

/// <summary>
/// Target with both an empty constructor and a mapping constructor.
/// </summary>
public sealed class UseConstructorEmptyPreferredTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorEmptyPreferredTarget"/> class.
    /// </summary>
    public UseConstructorEmptyPreferredTarget()
    {
        this.ConstructorUsed = "empty";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorEmptyPreferredTarget"/> class
    /// from the source (mapping constructor).
    /// </summary>
    /// <param name="source">The source.</param>
    public UseConstructorEmptyPreferredTarget(UseConstructorSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        this.ConstructorUsed = "mapping";
        this.PropertyA = source.PropertyA.ToString(CultureInfo.InvariantCulture);
        this.PropertyB = source.PropertyB;
    }

    /// <summary>
    /// Gets which constructor was invoked.
    /// </summary>
    public string ConstructorUsed { get; }

    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public string PropertyA { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets property B.
    /// </summary>
    public string PropertyB { get; set; } = string.Empty;
}

/// <summary>
/// Target with both an empty constructor and a parameterized constructor.
/// </summary>
public sealed class UseConstructorParameterizedPreferredTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorParameterizedPreferredTarget"/> class.
    /// </summary>
    public UseConstructorParameterizedPreferredTarget()
    {
        this.ConstructorUsed = "empty";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorParameterizedPreferredTarget"/> class
    /// with a string parameter.
    /// </summary>
    /// <param name="propertyB">The property B value.</param>
    public UseConstructorParameterizedPreferredTarget(string propertyB)
    {
        this.ConstructorUsed = "parameterized";
        this.PropertyB = propertyB;
    }

    /// <summary>
    /// Gets which constructor was invoked.
    /// </summary>
    public string ConstructorUsed { get; }

    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public string PropertyA { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets property B.
    /// </summary>
    public string PropertyB { get; set; } = string.Empty;
}

/// <summary>
/// Target with multiple parameterized constructors for exact-signature selection.
/// </summary>
public sealed class UseConstructorExactSignatureTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorExactSignatureTarget"/> class.
    /// </summary>
    public UseConstructorExactSignatureTarget()
    {
        this.ConstructorUsed = "empty";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorExactSignatureTarget"/> class
    /// with a single int parameter.
    /// </summary>
    /// <param name="propertyA">The property A value.</param>
    public UseConstructorExactSignatureTarget(int propertyA)
    {
        this.ConstructorUsed = "int";
        this.PropertyA = propertyA;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorExactSignatureTarget"/> class
    /// with int and string parameters.
    /// </summary>
    /// <param name="propertyA">The property A value.</param>
    /// <param name="propertyB">The property B value.</param>
    public UseConstructorExactSignatureTarget(int propertyA, string propertyB)
    {
        this.ConstructorUsed = "int-string";
        this.PropertyA = propertyA;
        this.PropertyB = propertyB;
    }

    /// <summary>
    /// Gets which constructor was invoked.
    /// </summary>
    public string ConstructorUsed { get; }

    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public int PropertyA { get; set; }

    /// <summary>
    /// Gets or sets property B.
    /// </summary>
    public string PropertyB { get; set; } = string.Empty;
}

/// <summary>
/// Nested source for typed <c>MappaUseConstructor</c> samples.
/// </summary>
public sealed class UseConstructorNestedInnerSource
{
    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public string PropertyA { get; set; } = string.Empty;
}

/// <summary>
/// Outer source containing a nested source.
/// </summary>
public sealed class UseConstructorNestedOuterSource
{
    /// <summary>
    /// Gets or sets the nested source.
    /// </summary>
    public UseConstructorNestedInnerSource Nested { get; set; } = new();
}

/// <summary>
/// Nested target with empty and parameterized constructors.
/// </summary>
public sealed class UseConstructorNestedInnerTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorNestedInnerTarget"/> class.
    /// </summary>
    public UseConstructorNestedInnerTarget()
    {
        this.ConstructorUsed = "empty";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseConstructorNestedInnerTarget"/> class
    /// with a string parameter.
    /// </summary>
    /// <param name="propertyA">The property A value.</param>
    public UseConstructorNestedInnerTarget(string propertyA)
    {
        this.ConstructorUsed = "parameterized";
        this.PropertyA = propertyA;
    }

    /// <summary>
    /// Gets which constructor was invoked.
    /// </summary>
    public string ConstructorUsed { get; }

    /// <summary>
    /// Gets or sets property A.
    /// </summary>
    public string PropertyA { get; set; } = string.Empty;
}

/// <summary>
/// Outer target containing a nested target.
/// </summary>
public sealed class UseConstructorNestedOuterTarget
{
    /// <summary>
    /// Gets or sets the nested target.
    /// </summary>
    public UseConstructorNestedInnerTarget Nested { get; set; } = new();
}