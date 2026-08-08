// <copyright file="MappaDependencyInjectionAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute used to mark a partial class for which the Mappa source generator should
/// emit a method that registers all <see cref="MappaAttribute"/> mapper types from the
/// same assembly into an <c>IServiceCollection</c>.
/// <para>
/// By default only mappers in the registrar's assembly are discovered. When
/// <see cref="InjectFromAssemblies"/> is non-empty, mappers from each marker type's
/// assembly are included as well (additive). Marker types are not specially registered
/// unless they also have <see cref="MappaAttribute"/> and pass the usual filters.
/// </para>
/// <para>
/// The class must be <c>partial</c>; otherwise the generator reports a warning and does
/// not emit registration code. Applying both <see cref="MappaAttribute"/> and
/// <see cref="MappaDependencyInjectionAttribute"/> on the same class is an error.
/// </para>
/// <para>
/// The generated method name is resolved as follows: a non-empty <see cref="MethodName"/>
/// property value wins; otherwise <see cref="ConstructorMethodName"/> is used; otherwise
/// the name is <c>Register{ClassName}</c>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MappaDependencyInjectionAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaDependencyInjectionAttribute"/> class.
    /// The generated method name defaults to <c>Register{ClassName}</c> unless
    /// <see cref="MethodName"/> is set.
    /// </summary>
    public MappaDependencyInjectionAttribute()
    {
        this.IgnoreType = [];
        this.InjectFromAssemblies = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaDependencyInjectionAttribute"/> class
    /// with a preferred generated method name.
    /// </summary>
    /// <param name="constructorMethodName">
    /// The name of the registration method to generate when <see cref="MethodName"/> is not set.
    /// </param>
    public MappaDependencyInjectionAttribute(string constructorMethodName)
    {
        this.ConstructorMethodName = constructorMethodName;
        this.IgnoreType = [];
        this.InjectFromAssemblies = [];
    }

    /// <summary>
    /// Gets the method name supplied via the constructor, if any.
    /// Used when <see cref="MethodName"/> is <c>null</c> or empty.
    /// </summary>
    public string? ConstructorMethodName { get; }

    /// <summary>
    /// Gets or sets a value indicating whether a <c>static</c> registrar class should generate
    /// an extension method on <c>IServiceCollection</c>. Defaults to <c>true</c>.
    /// Has no effect when the registrar class is not <c>static</c>.
    /// </summary>
    public bool ExtensionMethod { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the generated registration method.
    /// When <c>null</c> or empty, <see cref="ConstructorMethodName"/> (if any) or
    /// <c>Register{ClassName}</c> is used instead.
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the accessibility of the generated registration method.
    /// Defaults to <see cref="MappaDependencyInjectionMethodAccessibility.Public"/>.
    /// </summary>
    public MappaDependencyInjectionMethodAccessibility Accessibility { get; set; }
        = MappaDependencyInjectionMethodAccessibility.Public;

    /// <summary>
    /// Gets or sets the service lifetime used when registering mapper types.
    /// Defaults to <see cref="MappaDependencyInjectionServiceLifetime.Singleton"/>.
    /// </summary>
    public MappaDependencyInjectionServiceLifetime ServiceLifetime { get; set; }
        = MappaDependencyInjectionServiceLifetime.Singleton;

    /// <summary>
    /// Gets or sets how mapper classes and their interfaces are registered.
    /// Defaults to <see cref="MappaDependencyInjectionInjectInterfaces.ClassOnly"/>.
    /// </summary>
    public MappaDependencyInjectionInjectInterfaces InjectInterfaces { get; set; }
        = MappaDependencyInjectionInjectInterfaces.ClassOnly;

    /// <summary>
    /// Gets or sets the types to exclude from dependency injection registration.
    /// Defaults to an empty array. When a type is an interface and
    /// <see cref="InjectInterfaces"/> is <see cref="MappaDependencyInjectionInjectInterfaces.InterfaceOnly"/>
    /// or <see cref="MappaDependencyInjectionInjectInterfaces.InterfaceAndClass"/>, that interface
    /// is not used for registration.
    /// </summary>
    public Type[] IgnoreType { get; set; }

    /// <summary>
    /// Gets or sets marker types whose assemblies are scanned for additional
    /// <see cref="MappaAttribute"/> mapper types to register.
    /// Defaults to an empty array (same-assembly discovery only).
    /// When non-empty, each type's assembly is included in addition to the registrar's
    /// assembly. Existing tunables such as <see cref="IgnoreType"/>,
    /// <see cref="InjectInterfaces"/>, and <see cref="ServiceLifetime"/> apply across
    /// all discovered assemblies. Marker types themselves are not specially registered
    /// unless they also have <see cref="MappaAttribute"/> and are otherwise eligible.
    /// </summary>
    public Type[] InjectFromAssemblies { get; set; }
}