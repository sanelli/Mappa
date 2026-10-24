// <copyright file="AttributeConstantValueHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="AttributeConstantValueHelper"/>.
/// </summary>
public sealed class AttributeConstantValueHelperTests
{
    /// <summary>
    /// Test <see cref="AttributeConstantValueHelper.ToCode"/> rejects unexpected object values.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ToCodeThrowsForUnexpectedObjectValue()
    {
        var act = () => AttributeConstantValueHelper.ToCode(new object());

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected attribute constant value.");
    }
}