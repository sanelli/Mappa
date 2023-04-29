// <copyright file="StackSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Helpers;

/// <summary>
/// Describe settings that can be pushed or pulled.
/// </summary>
/// <typeparam name="TSettings">The type of the settings.</typeparam>
internal sealed class StackSettings<TSettings>
{
    private readonly Stack<TSettings> stack;

    /// <summary>
    /// Initializes a new instance of the <see cref="StackSettings{TSettings}"/> class.
    /// </summary>
    /// <param name="default">The default value of the settings.</param>
    public StackSettings(TSettings @default)
    {
        this.stack = new Stack<TSettings>();
        this.stack.Push(@default);
    }

    /// <summary>
    /// Gets the current value of the setting.
    /// </summary>
    internal TSettings CurrentValue => this.stack.Peek();

    /// <summary>
    /// Convert the settings to the current settings value.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>The current settings value.</returns>
    public static implicit operator TSettings(StackSettings<TSettings> settings) => settings.CurrentValue;

    /// <summary>
    /// Push a new value on top of the stack overriding the current value.
    /// </summary>
    /// <param name="value">The new value.</param>
    internal void Push(TSettings value) => this.stack.Push(value);

    /// <summary>
    /// Pop the current value from the stack
    /// making sure that the new value is the last value used.
    /// </summary>
    internal void Pop() => this.stack.Pop();

    /// <summary>
    /// Push the <paramref name="value"/>.
    /// Value will be popped using the <see cref="IDisposable"/>
    /// pattern.
    /// </summary>
    /// <param name="value">The value to be pushed on the stack.</param>
    /// <returns>A disposable object that will pop the value from stack upon disposal.</returns>
    internal IDisposable Apply(TSettings value)
        => new StackSettingsDisposable<TSettings>(this, value);

    private sealed class StackSettingsDisposable<T>
        : IDisposable
    {
        private readonly StackSettings<T> stackSettings;

        public StackSettingsDisposable(StackSettings<T> stackSettings, T value)
        {
            this.stackSettings = stackSettings;
            this.stackSettings.Push(value);
        }

        public void Dispose()
        {
            this.stackSettings.Pop();
        }
    }
}