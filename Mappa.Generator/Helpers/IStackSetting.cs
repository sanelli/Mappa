// <copyright file="IStackSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Helpers;

/// <summary>
/// Common operations for the setting
/// which values can be stacked.
/// </summary>
internal interface IStackSetting
{
    /// <summary>
    /// Pop the current value from the stack
    /// making sure that the new value is the last value used.
    /// </summary>
    void Pop();

    /// <summary>
    /// Push the default value on top of the value stack.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object that will pop the value from stack upon disposal.</returns>
    IDisposable ApplyDefault();
}