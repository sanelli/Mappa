// <copyright file="MapMethodTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MapMethod"/>.
/// </summary>
public sealed class MapMethodTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MapMethod.SetStrategy"/> throws when a strategy has already been set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SetStrategyThrowsWhenStrategyIsAlreadySet()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map");
        var identityStrategy = new IdentityMapStrategy(mapMethod.TargetType, mapMethod.SourceType);
        mapMethod.SetStrategy(new MethodParameterMapStrategy(identityStrategy));

        var act = () => mapMethod.SetStrategy(new MethodParameterMapStrategy(identityStrategy));

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Strategy for method\"Map\" has already been identified.");
    }

    /// <summary>
    /// Test <see cref="MapMethod.SetPragmaWarning"/> throws when the value has already been set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SetPragmaWarningThrowsWhenValueIsAlreadySet()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map");
        mapMethod.SetPragmaWarning(PragmaWarningSetting.NoBlock);

        var act = () => mapMethod.SetPragmaWarning(PragmaWarningSetting.Disable);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("You are trying to set a pragma warning multiple times.");
    }

    /// <summary>
    /// Test <see cref="MapMethod.GetMappaContextParameterName"/> throws when the method does not provide a context parameter.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMappaContextParameterNameThrowsWhenContextParameterIsMissing()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map");

        var act = () => mapMethod.GetMappaContextParameterName();

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Method does not have a mappa context parameter.");
    }

    /// <summary>
    /// Test the dependency-method constructor sets <see cref="MapMethod.AccessFieldName"/> and marks the method as mapped.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DependencyConstructorSetsAccessFieldNameAndMarksMethodAsMapped()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public sealed class Dependency
                              {
                                  public Target Map(Source input, MappaContext context) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var dependencyType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
        if (dependencyType is null)
        {
            throw new InvalidOperationException("Expected dependency type to be present in the compilation.");
        }

        var methodSymbol = dependencyType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(
            methodSymbol,
            "this.DependencyProperty",
            nullableEnabled: false,
            canBeUsedByStaticMethod: false,
            attributes: []);

        mapMethod.AccessFieldName.Should().Be("this.DependencyProperty");
        mapMethod.Mapped.Should().BeTrue();
        mapMethod.MaybeGetMappaContextParameterName().Should().Be("context");
        mapMethod.RequireMappaContextWhenInvoked().Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsRelaxedMapFor"/> matches when nullability can be relaxed on both axes.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedMapForReturnsTrueForSupportedNullabilityMismatches()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target? Map(Source? input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map", nullableEnabled: true);
        var requiredTarget = mapMethod.TargetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var requiredSource = mapMethod.SourceType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        mapMethod.IsRelaxedMapFor(requiredTarget, requiredSource, includeNullability: true).Should().BeTrue();
        mapMethod.IsMapFor(requiredTarget, requiredSource, includeNullability: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsRelaxedMapFor"/> rejects unsupported nullability mismatches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedMapForReturnsFalseForUnsupportedNullabilityMismatches()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public Target Map(Source input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map", nullableEnabled: true);
        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!
            .WithNullableAnnotation(NullableAnnotation.Annotated);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        mapMethod.IsRelaxedMapFor(targetType, sourceType, includeNullability: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsRelaxedMapFor"/> is disabled when nullability is disabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedMapForReturnsFalseWhenNullabilityIsDisabled()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var mapMethod = CreateMapMethodFromSyntax(source, "Map", nullableEnabled: false);
        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;

        mapMethod.IsRelaxedMapFor(targetType, sourceType, includeNullability: false).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsCompatibleMapFor"/> matches when the required source is derived
    /// from the method parameter type and the method return type is derived from the required target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsCompatibleMapForReturnsTrueForBaseParameterAndDerivedReturn()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseSource { }

                              public class DerivedSource : BaseSource { }

                              public class BaseTarget { }

                              public class DerivedTarget : BaseTarget { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public DerivedTarget Map(BaseSource input) => new DerivedTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateMapMethodFromCompilation(compilation, "Map");
        var requiredSource = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource")!;
        var requiredTarget = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget")!;

        mapMethod.IsCompatibleMapFor(requiredTarget, requiredSource, compilation).Should().BeTrue();
        mapMethod.IsMapFor(requiredTarget, requiredSource, includeNullability: false).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsCompatibleMapFor"/> matches when the method parameter is an interface
    /// implemented by the required source.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsCompatibleMapForReturnsTrueForInterfaceParameter()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public interface ISource { }

                              public class DerivedSource : ISource { }

                              public class BaseTarget { }

                              public class DerivedTarget : BaseTarget { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public DerivedTarget Map(ISource input) => new DerivedTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateMapMethodFromCompilation(compilation, "Map");
        var requiredSource = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource")!;
        var requiredTarget = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget")!;

        mapMethod.IsCompatibleMapFor(requiredTarget, requiredSource, compilation).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="MapMethod.IsCompatibleMapFor"/> rejects incompatible types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsCompatibleMapForReturnsFalseForIncompatibleTypes()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseSource { }

                              public class UnrelatedSource { }

                              public class BaseTarget { }

                              public class UnrelatedTarget { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public BaseTarget Map(BaseSource input) => new BaseTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateMapMethodFromCompilation(compilation, "Map");
        var requiredSource = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.UnrelatedSource")!;
        var requiredTarget = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.UnrelatedTarget")!;

        mapMethod.IsCompatibleMapFor(requiredTarget, requiredSource, compilation).Should().BeFalse();
    }

    private static MapMethod CreateMapMethodFromSyntax(string source, string methodName, bool nullableEnabled = false)
    {
        var compilation = BuildCompilation(source);
        return CreateMapMethodFromCompilation(compilation, methodName, nullableEnabled);
    }

    private static MapMethod CreateMapMethodFromCompilation(CSharpCompilation compilation, string methodName, bool nullableEnabled = false)
    {
        var syntaxTree = compilation.SyntaxTrees[0];
        var methodDeclarationSyntax = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == methodName);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        return new MapMethod(
            methodDeclarationSyntax,
            semanticModel,
            nullableEnabled,
            CancellationToken.None);
    }
}