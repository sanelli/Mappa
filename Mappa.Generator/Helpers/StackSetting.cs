// <copyright file="StackSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Helpers;

/// <summary>
/// Describe settings that can be pushed or pulled.
/// </summary>
/// <typeparam name="TSettings">The type of the settings.</typeparam>
internal sealed class StackSetting<TSettings>
    : IStackSetting
{
    private readonly Stack<TSettings> stack;
    private readonly TSettings @default;

    /// <summary>
    /// Initializes a new instance of the <see cref="StackSetting{TSettings}"/> class.
    /// </summary>
    /// <param name="default">The default value of the settings.</param>
    public StackSetting(TSettings @default)
    {
        this.@default = @default;
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
    /// <param name="setting">The settings.</param>
    /// <returns>The current settings value.</returns>
    public static implicit operator TSettings(StackSetting<TSettings> setting) => setting.CurrentValue;

    /// <inheritdoc />
    public void Pop() => this.stack.Pop();

    /// <inheritdoc />
    public IDisposable ApplyDefault()
        => new StackSettingsDisposable<TSettings>(this, this.@default);

    /// <summary>
    /// Push a new value on top of the stack overriding the current value.
    /// </summary>
    /// <param name="value">The new value.</param>
    internal void Push(TSettings value) => this.stack.Push(value);

    /// <summary>
    /// Push the <paramref name="value"/>.
    /// Value will be popped using the <see cref="IDisposable"/>
    /// pattern.
    /// </summary>
    /// <param name="value">The value to be pushed on the stack.</param>
    /// <returns>An <see cref="IDisposable"/> object that will pop the value from stack upon disposal.</returns>
    internal IDisposable Apply(TSettings value)
        => new StackSettingsDisposable<TSettings>(this, value);

    /// <summary>
    /// Check if <see cref="CurrentValue"/> is equal to
    /// <paramref name="expectedValue"/>.
    /// </summary>
    /// <param name="expectedValue">The expected value.</param>
    /// <returns><c>true</c> if <see cref="CurrentValue"/> is equal to
    /// <paramref name="expectedValue"/>, <c>false</c> otherwise.</returns>
    internal bool Equals(TSettings expectedValue)
    {
        if (expectedValue is null)
        {
            return false;
        }

        return expectedValue.Equals(this.CurrentValue);
    }

    private sealed class StackSettingsDisposable<T>
        : IDisposable
    {
        private readonly StackSetting<T> stackSetting;

        public StackSettingsDisposable(StackSetting<T> stackSetting, T value)
        {
            this.stackSetting = stackSetting;
            this.stackSetting.Push(value);
        }

        public void Dispose()
        {
            this.stackSetting.Pop();
        }
    }
}