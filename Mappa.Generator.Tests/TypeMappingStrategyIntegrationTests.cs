// <copyright file="TypeMappingStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="TypeMappingStrategy"/>.
/// </summary>
// TODO [#49] Test with interface.
// TODO [#49] Test with nullable.
// TODO [#49] Test with nested classes.
// TODO [#49] Test with explicit throw behaviour without class.
// TODO [#49] Test with explicit throw behaviour with exception class.
// TODO [#49] Test with explicit map to behaviour without type.
// TODO [#49] Test with explicit map to behaviour failing because target is interface.
// TODO [#49] Test with explicit map to behaviour failing because target is virtual.
// TODO [#49] Test with explicit map to behaviour with specific type.
// TODO [#49] Test with explicit map to behaviour with null.
// TODO [#49] Test with explicit map to behaviour with default.
// TODO [#49] Test with invoke method to behaviour with static method in mapper with single parameter.
// TODO [#49] Test with invoke method to behaviour with non-static method in mapper.
// TODO [#49] Test with invoke method to behaviour with static method in a different class mapper.
// TODO [#49] Test with invoke method to behaviour with static method in mapper with context parameter.
// TODO [#49] Test with invoke method to behaviour with static method defined in mapper base class.
public sealed class TypeMappingStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test mapping works between classes using multiple
    /// <see cref="MappaTypeMappingAttribute"/> and no
    /// <see cref="MappaTypeMappingDefaultAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithMultipleClassesSubTypeMappingAttributes()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class SourceBaseClass 
                                  {
                                     public byte BaseClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFirstDerivedClass : SourceBaseClass
                                  {
                                     public float FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceSecondDerivedClass : SourceBaseClass
                                  {
                                     public DateTime SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceThirdDerivedClass : SourceSecondDerivedClass
                                  {
                                     public string ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFourthDerivedClass : SourceSecondDerivedClass
                                  {
                                     public Guid[] FourthDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class SourceFifthDerivedClass : SourceFourthDerivedClass
                                  {
                                     public string[] FifthDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetBaseClass 
                                  {
                                     public int BaseClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFirstDerivedClass : TargetBaseClass
                                  {
                                     public string FirstDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetSecondDerivedClass : TargetBaseClass
                                  {
                                     public DateOnly SecondDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetThirdDerivedClass : TargetSecondDerivedClass
                                  {
                                     public long ThirdDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFourthDerivedClass : TargetSecondDerivedClass
                                  {
                                     public string[] FourthDerivedClassProperty {get; set;}
                                  }
                                  
                                  public class TargetFifthDerivedClass : TargetFourthDerivedClass
                                  {
                                     public Guid[] FifthDerivedClassProperty {get; set;}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaTypeMapping(typeof(TargetSecondDerivedClass), typeof(SourceSecondDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetThirdDerivedClass), typeof(SourceThirdDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFourthDerivedClass), typeof(SourceFourthDerivedClass))]
                                      [MappaTypeMapping(typeof(TargetFifthDerivedClass), typeof(SourceFifthDerivedClass))]
                                      public partial TargetBaseClass Map(SourceBaseClass input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetBaseClass",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceBaseClass",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    // TODO [#49] Add assertions.
                });
    }
}