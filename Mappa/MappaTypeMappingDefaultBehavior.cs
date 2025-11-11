// <copyright file="MappaTypeMappingDefaultBehavior.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describe the default mapping from source to target when type mapping is applied to a method.
/// </summary>
public enum MappaTypeMappingDefaultBehavior
{
    /// <summary>
    /// Undefined setting.
    /// </summary>
    Undefined,

    /// <summary>
    /// If the source type cannot be mapped using any other the subtypes
    /// then throw an exception. The default exception thrown is <see cref="ArgumentOutOfRangeException"/>
    /// but the exception thrown can be defined using the <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    Throw,

    /// <summary>
    /// If the source type cannot be mapped using any other the subtypes
    /// then return the <c>default</c> value for that type (which might be <c>null</c>.
    /// </summary>
    Default,

    /// <summary>
    /// If the source type cannot be mapped using any other the subtypes
    /// then return <c>null</c>).
    /// </summary>
    Null,

    /// <summary>
    /// If the source type cannot be mapped using any other the subtypes
    /// then map the source to a target type. The default target type used
    /// is the target type of the mapping method, but the target type
    /// can be defined using the <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    MapSourceType,

    /// <summary>
    /// If the source type cannot be mapped using any other the subtypes
    /// then invoke the method that should be returning exactly the target type
    /// and can receive as input the input type (and possibly the context parameter).
    /// The name of the method and the class on which the method resides can be defined
    /// via <see cref="MappaTypeMappingDefaultAttribute"/>. If the method is on a
    /// different class it must be a static method.
    /// </summary>
    InvokeMethod,
}