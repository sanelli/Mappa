// <copyright file="SymbolExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="SymbolExtensions"/>.
/// </summary>
public sealed class SymbolExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="SymbolExtensions.GetSymbolModifiers"/> emits expected keywords for methods and types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetSymbolModifiersReturnsExpectedKeywords()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public abstract class Animal
                              {
                                  public abstract void Speak();

                                  public virtual void Move()
                                  {
                                  }
                              }

                              public sealed class Dog : Animal
                              {
                                  public override void Speak()
                                  {
                                  }

                                  public override void Move()
                                  {
                                  }
                              }

                              public static class Utility
                              {
                                  public static void Run()
                                  {
                                  }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var animal = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Animal");
        var dog = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dog");
        var utility = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Utility");
        if (animal is null || dog is null || utility is null)
        {
            throw new InvalidOperationException("Expected animal, dog, and utility types to be present in the compilation.");
        }

        var abstractSpeak = animal.GetMembers("Speak").OfType<IMethodSymbol>().Single();
        var virtualMove = animal.GetMembers("Move").OfType<IMethodSymbol>().Single();
        var overrideSpeak = dog.GetMembers("Speak").OfType<IMethodSymbol>().Single();
        var overrideMove = dog.GetMembers("Move").OfType<IMethodSymbol>().Single();
        var staticRun = utility.GetMembers("Run").OfType<IMethodSymbol>().Single();

        abstractSpeak.GetSymbolModifiers().Should().Be("public abstract");
        virtualMove.GetSymbolModifiers().Should().Be("public virtual");
        overrideSpeak.GetSymbolModifiers().Should().Be("public override");
        overrideMove.GetSymbolModifiers().Should().Be("public override");
        staticRun.GetSymbolModifiers().Should().Be("public static");
        dog.GetSymbolModifiers().Should().Be("public sealed");
        utility.GetSymbolModifiers().Should().Be("public static");
    }
}