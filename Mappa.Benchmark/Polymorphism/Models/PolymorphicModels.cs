// <copyright file="PolymorphicModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Polymorphism.Models;

/// <summary>
/// Polymorphic source base type.
/// </summary>
public abstract class AnimalDto
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Dog source type.
/// </summary>
public sealed class DogDto : AnimalDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the dog is trained.
    /// </summary>
    public bool Trained { get; set; }
}

/// <summary>
/// Cat source type.
/// </summary>
public sealed class CatDto : AnimalDto
{
    /// <summary>
    /// Gets or sets the number of lives.
    /// </summary>
    public int Lives { get; set; }
}

/// <summary>
/// Bird source type.
/// </summary>
public sealed class BirdDto : AnimalDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the bird can fly.
    /// </summary>
    public bool CanFly { get; set; }
}

/// <summary>
/// Polymorphic target base type.
/// </summary>
public abstract class Animal
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Dog target type.
/// </summary>
public sealed class Dog : Animal
{
    /// <summary>
    /// Gets or sets a value indicating whether the dog is trained.
    /// </summary>
    public bool Trained { get; set; }
}

/// <summary>
/// Cat target type.
/// </summary>
public sealed class Cat : Animal
{
    /// <summary>
    /// Gets or sets the number of lives.
    /// </summary>
    public int Lives { get; set; }
}

/// <summary>
/// Bird target type.
/// </summary>
public sealed class Bird : Animal
{
    /// <summary>
    /// Gets or sets a value indicating whether the bird can fly.
    /// </summary>
    public bool CanFly { get; set; }
}