// <copyright file="StackSettingTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="StackSetting{TSettings}"/>.
/// </summary>
public sealed class StackSettingTests
{
    /// <summary>
    /// Test <see cref="StackSetting{TSettings}.Apply"/> restores the previous value when disposed.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ApplyRestoresPreviousValueOnDispose()
    {
        var setting = new StackSetting<string>("default");

        setting.CurrentValue.Should().Be("default");

        using (setting.Apply("override"))
        {
            setting.CurrentValue.Should().Be("override");
        }

        setting.CurrentValue.Should().Be("default");
    }

    /// <summary>
    /// Test <see cref="StackSetting{TSettings}.ApplyDefault"/> restores the default value when disposed.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ApplyDefaultRestoresDefaultValueOnDispose()
    {
        var setting = new StackSetting<int>(1);
        setting.Push(2);

        using (setting.ApplyDefault())
        {
            setting.CurrentValue.Should().Be(1);
        }

        setting.CurrentValue.Should().Be(2);
    }

    /// <summary>
    /// Test <see cref="StackSetting{TSettings}.Pop"/> removes the top value from the stack.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PopRemovesTopValue()
    {
        var setting = new StackSetting<string>("first");
        setting.Push("second");

        setting.Pop();

        setting.CurrentValue.Should().Be("first");
        setting.Count.Should().Be(1);
    }

    /// <summary>
    /// Test <see cref="StackSetting{TSettings}.Equals(TSettings)"/> returns <c>false</c> when the expected value is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EqualsReturnsFalseWhenExpectedValueIsNull()
    {
        var setting = new StackSetting<string?>("value");

        setting.Equals(null).Should().BeFalse();
    }

    /// <summary>
    /// Test the implicit conversion operator returns <see cref="StackSetting{TSettings}.CurrentValue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ImplicitConversionReturnsCurrentValue()
    {
        var setting = new StackSetting<string>("current");

        string value = setting;

        value.Should().Be("current");
    }
}