// <copyright file="MappaInterceptorAttributeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Interceptor AOT runners share this file by design.
#pragma warning disable SA1649 // File name need not match the first type; runners share this file by design.

using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaInterceptorAttributeMapper"/>.
/// </summary>
internal static class MappaInterceptorAttributeMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="MappaInterceptorAttributeMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaInterceptorAttributeMapper));
        var mapper = new MappaInterceptorAttributeMapper();

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapUseProperty),
            nameof(InterceptorRenameSource),
            nameof(InterceptorRenameTarget),
            new InterceptorRenameSource { SourceValue = 11 },
            mapper.MapUseProperty(new InterceptorRenameSource { SourceValue = 11 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapAssignFromConstant),
            nameof(InterceptorPersonSource),
            nameof(InterceptorConstantTarget),
            new InterceptorPersonSource(),
            mapper.MapAssignFromConstant(new InterceptorPersonSource()));

        MappaContext context = new Dictionary<string, object> { ["answer"] = 99 };
        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapAssignFromContext),
            "InterceptorPersonSource, MappaContext",
            nameof(InterceptorContextTarget),
            new InterceptorPersonSource(),
            mapper.MapAssignFromContext(new InterceptorPersonSource(), context));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapNullSubstitute),
            nameof(NullSubstituteSourceModel),
            nameof(NullSubstituteTargetModel),
            new NullSubstituteSourceModel(),
            mapper.MapNullSubstitute(new NullSubstituteSourceModel()));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapInvokeMethod),
            nameof(InterceptorPersonSource),
            nameof(InterceptorPersonTarget),
            new InterceptorPersonSource { Name = "Ada", Age = 36 },
            mapper.MapInvokeMethod(new InterceptorPersonSource { Name = "Ada", Age = 36 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapHooks),
            nameof(InterceptorHookCounter),
            nameof(InterceptorHookCounter),
            new InterceptorHookCounter { Value = 1 },
            mapper.MapHooks(new InterceptorHookCounter { Value = 1 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapMustMap),
            nameof(InterceptorPersonSource),
            nameof(InterceptorPersonTarget),
            new InterceptorPersonSource { Name = "Ada", Age = 36 },
            mapper.MapMustMap(new InterceptorPersonSource { Name = "Ada", Age = 36 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapIgnoreTarget),
            nameof(InterceptorKeepDropSource),
            nameof(InterceptorKeepDropTarget),
            new InterceptorKeepDropSource { Keep = 5, Drop = 9 },
            mapper.MapIgnoreTarget(new InterceptorKeepDropSource { Keep = 5, Drop = 9 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapInaccessible),
            nameof(InaccessibleMembersSourceModel),
            nameof(InaccessibleMembersPublicCtorTargetModel),
            new InaccessibleMembersSourceModel("Ada", 36),
            mapper.MapInaccessible(new InaccessibleMembersSourceModel("Ada", 36)));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapTransformer),
            "string",
            "string",
            "  hello  ",
            mapper.MapTransformer("  hello  "));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapObjectFactory),
            nameof(ObjectFactorySourceModel),
            nameof(ObjectFactoryTargetModel),
            new ObjectFactorySourceModel { Name = "n", Value = 3 },
            mapper.MapObjectFactory(new ObjectFactorySourceModel { Name = "n", Value = 3 }));

        report.RecordInvocation(
            nameof(MappaInterceptorAttributeMapper.MapNestedPath),
            nameof(NestedPropertyPathPersonSourceModel),
            nameof(NestedPropertyPathPersonTargetModel),
            new NestedPropertyPathPersonSourceModel
            {
                Address = new NestedPropertyPathAddressModel { City = "Rome", ZipCode = "00100" },
            },
            mapper.MapNestedPath(new NestedPropertyPathPersonSourceModel
            {
                Address = new NestedPropertyPathAddressModel { City = "Rome", ZipCode = "00100" },
            }));
    }
}

/// <summary>
/// AOT runner for interceptor dependency and field sample mappers.
/// </summary>
internal static class MappaInterceptorDependencyMapperRunner
{
    /// <summary>
    /// Runs map methods on interceptor dependency and field sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaInterceptorDualMapper));
        var dualMapper = new MappaInterceptorDualMapper();
        var parent = new InterceptorParentSource
        {
            Child = new InterceptorChildSource { Code = 7 },
        };
        report.RecordInvocation(
            nameof(MappaInterceptorDualMapper.MapParent),
            nameof(InterceptorParentSource),
            nameof(InterceptorParentTarget),
            parent,
            dualMapper.MapParent(parent));

        report.BeginMapper(nameof(MappaInterceptorMethodDependencyMapper));
        var methodDependencyMapper = new MappaInterceptorMethodDependencyMapper();
        report.RecordInvocation(
            nameof(MappaInterceptorMethodDependencyMapper.MapParent),
            nameof(InterceptorParentSource),
            nameof(InterceptorParentTarget),
            parent,
            methodDependencyMapper.MapParent(parent));

        report.BeginMapper(nameof(MappaInterceptorStaticDependencyMapper));
        var staticDependencyMapper = new MappaInterceptorStaticDependencyMapper();
        report.RecordInvocation(
            nameof(MappaInterceptorStaticDependencyMapper.MapWithStaticDependency),
            nameof(InterceptorRenameSource),
            nameof(InterceptorRenameTarget),
            new InterceptorRenameSource { SourceValue = 4 },
            staticDependencyMapper.MapWithStaticDependency(new InterceptorRenameSource { SourceValue = 4 }));

        report.BeginMapper(nameof(MappaInterceptorFieldMapper));
        var fieldMapper = new MappaInterceptorFieldMapper();
        var fieldSource = new InterceptorFieldSource { Value = 15 };
        report.RecordInvocation(
            nameof(MappaInterceptorFieldMapper.MapFields),
            nameof(InterceptorFieldSource),
            nameof(InterceptorFieldTarget),
            fieldSource,
            fieldMapper.MapFields(fieldSource));
    }
}