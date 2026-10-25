// <copyright file="MappaInterceptorAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Runtime tests for <see cref="MappaInterceptorAttributeMapper"/> and related interceptor samples.
/// </summary>
public sealed class MappaInterceptorAttributeMapperUnitTests
{
    private readonly MappaInterceptorAttributeMapper mapper = new();

    /// <summary>
    /// Test property remapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUseProperty()
    {
        var actual = this.mapper.MapUseProperty(new InterceptorRenameSource { SourceValue = 11 });
        actual.TargetValue.Should().Be(11);
    }

    /// <summary>
    /// Test assign-from-constant on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapAssignFromConstant()
    {
        this.mapper.MapAssignFromConstant(new InterceptorPersonSource()).Value.Should().Be(42);
    }

    /// <summary>
    /// Test assign-from-context on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapAssignFromContext()
    {
        var context = new MappaContext { ["answer"] = 99 };
        this.mapper.MapAssignFromContext(new InterceptorPersonSource(), context).Value.Should().Be(99);
    }

    /// <summary>
    /// Test null substitute on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNullSubstitute()
    {
        var actual = this.mapper.MapNullSubstitute(new NullSubstituteSourceModel());
        actual.Name.Should().Be("Anonymous");
        actual.Alias.Should().Be("NoAlias");
    }

    /// <summary>
    /// Test before/after map hooks on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapHooks()
    {
        var actual = this.mapper.MapHooks(new InterceptorHookCounter { Value = 1 });
        actual.Value.Should().Be(12);
        this.mapper.HookCalls.Should().Equal("before", "after");
    }

    /// <summary>
    /// Test must-map on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMustMap()
    {
        var actual = this.mapper.MapMustMap(new InterceptorPersonSource { Name = "Ada", Age = 36 });
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Test ignore-target on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIgnoreTarget()
    {
        var actual = this.mapper.MapIgnoreTarget(new InterceptorKeepDropSource { Keep = 5, Drop = 9 });
        actual.Keep.Should().Be(5);
        actual.Drop.Should().Be(0);
    }

    /// <summary>
    /// Test inaccessible member mapping on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInaccessible()
    {
        var actual = this.mapper.MapInaccessible(new InaccessibleMembersSourceModel("Ada", 36));
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Test value transformers on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTransformer()
    {
        this.mapper.MapTransformer("  hello  ").Should().Be("hello");
    }

    /// <summary>
    /// Test object factory on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapObjectFactory()
    {
        var actual = this.mapper.MapObjectFactory(new ObjectFactorySourceModel { Name = "n", Value = 3 });
        actual.Name.Should().Be("n");
        actual.Value.Should().Be(3);
        actual.FactoryTag.Should().Be("interceptor-factory");
    }

    /// <summary>
    /// Test nested property paths on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNestedPath()
    {
        var actual = this.mapper.MapNestedPath(new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel { City = "Rome", ZipCode = "00100" },
        });
        actual.Address.City.Should().Be("Rome");
        actual.Address.ZipCode.Should().Be("00100");
    }

    /// <summary>
    /// Test dual <c>[Mappa]</c> + <c>[MappaInterceptor]</c> dependency reuse.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDualDependency()
    {
        var dualMapper = new MappaInterceptorDualMapper();
        var actual = dualMapper.MapParent(new InterceptorParentSource
        {
            Child = new InterceptorChildSource { Code = 7 },
        });
        actual.Child.Code.Should().Be("7");
    }

    /// <summary>
    /// Test interceptor method dependency attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMethodDependency()
    {
        var methodDependencyMapper = new MappaInterceptorMethodDependencyMapper();
        var actual = methodDependencyMapper.MapParent(new InterceptorParentSource
        {
            Child = new InterceptorChildSource { Code = 8 },
        });
        actual.Child.Code.Should().Be("8");
    }

    /// <summary>
    /// Test static dependency via <c>With</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStaticDependency()
    {
        var staticDependencyMapper = new MappaInterceptorStaticDependencyMapper();
        staticDependencyMapper.MapWithStaticDependency(new InterceptorRenameSource { SourceValue = 4 })
            .TargetValue.Should().Be(4);
    }

    /// <summary>
    /// Test field mapping via <c>With</c> settings.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFields()
    {
        var fieldMapper = new MappaInterceptorFieldMapper();
        var source = new InterceptorFieldSource { Value = 15 };
        fieldMapper.MapFields(source).Value.Should().Be(15);
    }
}