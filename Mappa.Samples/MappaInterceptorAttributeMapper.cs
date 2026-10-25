// <copyright file="MappaInterceptorAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Sample mapper demonstrating supported <c>With</c> attributes and settings on the interceptor route.
/// </summary>
#pragma warning disable CA1822 // Sample helpers remain instance methods to match typical mapper style.
[MappaInterceptor]
[MappaObjectFactory(typeof(ObjectFactoryTargetModel), nameof(CreateTarget))]
public sealed partial class MappaInterceptorAttributeMapper
{
    /// <summary>
    /// Gets the recorded hook invocation order for the latest mapping.
    /// </summary>
    public Collection<string> HookCalls { get; } = [];

    /// <summary>
    /// Remap differently named properties via <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorRenameTarget MapUseProperty(InterceptorRenameSource value)
    {
        return this.MappaMap<InterceptorRenameTarget>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaUsePropertyAttribute(
                        nameof(InterceptorRenameTarget.TargetValue),
                        nameof(InterceptorRenameSource.SourceValue)),
                ]);
    }

    /// <summary>
    /// Assign a constant to a target member via <see cref="MappaAssignFromConstantAttribute"/>.
    /// </summary>
    /// <param name="value">Unused source.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorConstantTarget MapAssignFromConstant(InterceptorPersonSource value)
    {
        return this.MappaMap<InterceptorConstantTarget>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaAssignFromConstantAttribute(nameof(InterceptorConstantTarget.Value), 42),
                ]);
    }

    /// <summary>
    /// Assign from <see cref="MappaContext"/> via <see cref="MappaAssignFromContextAttribute"/>.
    /// </summary>
    /// <param name="value">Unused source.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorContextTarget MapAssignFromContext(InterceptorPersonSource value, MappaContext context)
    {
        return this.MappaMap<InterceptorContextTarget>(value, context)
            .With(
                mappaMethodAttributes:
                [
                    new MappaAssignFromContextAttribute(nameof(InterceptorContextTarget.Value), "answer"),
                ]);
    }

    /// <summary>
    /// Substitute null source members via <see cref="MappaNullSubstituteAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public NullSubstituteTargetModel MapNullSubstitute(NullSubstituteSourceModel value)
    {
        return this.MappaMap<NullSubstituteTargetModel>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaNullSubstituteAttribute(nameof(NullSubstituteTargetModel.Name)) { AssignValue = "Anonymous" },
                    new MappaNullSubstituteAttribute(nameof(NullSubstituteTargetModel.Alias)) { InvokeMethod = nameof(WhenAliasNull) },
                ]);
    }

    /// <summary>
    /// Invoke a mapper helper via <see cref="MappaInvokeMethodAttribute"/>.
    /// </summary>
    /// <param name="value">The source person.</param>
    /// <returns>The mapped person with a formatted name.</returns>
    public InterceptorPersonTarget MapInvokeMethod(InterceptorPersonSource value)
    {
        return this.MappaMap<InterceptorPersonTarget>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaInvokeMethodAttribute(nameof(InterceptorPersonTarget.Name), nameof(this.FormatName)),
                ]);
    }

    /// <summary>
    /// Run before/after map hooks via <see cref="MappaBeforeMapAttribute"/> and <see cref="MappaAfterMapAttribute"/>.
    /// </summary>
    /// <param name="value">The counter.</param>
    /// <returns>The mapped counter.</returns>
    public InterceptorHookCounter MapHooks(InterceptorHookCounter value)
    {
        this.HookCalls.Clear();
        return this.MappaMap<InterceptorHookCounter>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaBeforeMapAttribute(nameof(this.Before)),
                    new MappaAfterMapAttribute(nameof(this.After)),
                ]);
    }

    /// <summary>
    /// Require a target member via <see cref="MappaMustMapTargetPropertyAttribute"/>.
    /// </summary>
    /// <param name="value">The source person.</param>
    /// <returns>The mapped person.</returns>
    public InterceptorPersonTarget MapMustMap(InterceptorPersonSource value)
    {
        return this.MappaMap<InterceptorPersonTarget>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaMustMapTargetPropertyAttribute(nameof(InterceptorPersonTarget.Name)),
                ]);
    }

    /// <summary>
    /// Ignore a target member via <see cref="MappaIgnoreTargetPropertyAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public InterceptorKeepDropTarget MapIgnoreTarget(InterceptorKeepDropSource value)
    {
        return this.MappaMap<InterceptorKeepDropTarget>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaIgnoreTargetPropertyAttribute(nameof(InterceptorKeepDropTarget.Drop)),
                ]);
    }

    /// <summary>
    /// Read inaccessible source members via <see cref="MappaAllowInaccessibleSourceMembersAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public InaccessibleMembersPublicCtorTargetModel MapInaccessible(InaccessibleMembersSourceModel value)
    {
        return this.MappaMap<InaccessibleMembersPublicCtorTargetModel>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaAllowInaccessibleSourceMembersAttribute(),
                    new MappaAllowInaccessibleTargetMembersAttribute(),
                ]);
    }

    /// <summary>
    /// Apply class-level value transformers on the interceptor route.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>The transformed string.</returns>
    public string MapTransformer(string value) => this.MappaMap<string>(value);

    /// <summary>
    /// Construct the target via class-level <see cref="MappaObjectFactoryAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public ObjectFactoryTargetModel MapObjectFactory(ObjectFactorySourceModel value)
    {
        return this.MappaMap<ObjectFactoryTargetModel>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaIgnoreTargetPropertyAttribute(nameof(ObjectFactoryTargetModel.FactoryTag)),
                ]);
    }

    /// <summary>
    /// Map nested property paths via <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <param name="value">The source.</param>
    /// <returns>The mapped target.</returns>
    public NestedPropertyPathPersonTargetModel MapNestedPath(NestedPropertyPathPersonSourceModel value)
    {
        return this.MappaMap<NestedPropertyPathPersonTargetModel>(value)
            .With(
                mappaMethodAttributes:
                [
                    new MappaUsePropertyAttribute("Address.City", "Address.City"),
                    new MappaUsePropertyAttribute("Address.ZipCode", "Address.ZipCode"),
                ]);
    }

    /// <summary>
    /// Factory used by class-level <see cref="MappaObjectFactoryAttribute"/>.
    /// </summary>
    /// <returns>A new target instance.</returns>
    public ObjectFactoryTargetModel CreateTarget()
    {
        return new ObjectFactoryTargetModel
        {
            FactoryTag = "interceptor-factory",
        };
    }

    [MappaIgnore]
    private static string WhenAliasNull(string? value) => value ?? "NoAlias";

    [MappaIgnore]
    [MappaTransformer]
    private string Trim(string value) => value.Trim();

    [MappaIgnore]
    private string FormatName(InterceptorPersonSource source) => source.Name.ToUpperInvariant();

    private void Before(ref InterceptorHookCounter input)
    {
        this.HookCalls.Add("before");
        input.Value += 1;
    }

    private void After(ref InterceptorHookCounter target)
    {
        this.HookCalls.Add("after");
        target.Value += 10;
    }
}
#pragma warning restore CA1822