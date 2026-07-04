// <copyright file="IdentityMapDeepCopyStruct.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

#pragma warning disable CA1815 // Override equals and operator equals on value types

/// <summary>
/// Struct root model with a reference-type field used by the identity map deep copy sample mappers.
/// </summary>
public struct IdentityMapDeepCopyStruct
{
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Field should be private
#pragma warning disable S1104 // Fields should not be public
    /// <summary>
    /// The nested child reference.
    /// </summary>
    public IdentityMapDeepCopyChild Child;
#pragma warning restore S1104
#pragma warning restore SA1401
#pragma warning restore CA1051
}

#pragma warning restore CA1815