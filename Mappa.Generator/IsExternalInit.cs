// <copyright file="IsExternalInit.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Enables the <c>init</c> accessor and positional record structs on older TFMs.
/// </summary>
#pragma warning disable S2094 // Empty class required for init accessors on older TFMs
internal static class IsExternalInit
{
}
#pragma warning restore S2094
#endif