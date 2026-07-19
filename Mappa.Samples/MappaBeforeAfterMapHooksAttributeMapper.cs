// <copyright file="MappaBeforeAfterMapHooksAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaBeforeMapAttribute"/> and <see cref="MappaAfterMapAttribute"/>.
/// </summary>
/// <remarks>
/// Class-level before hooks run before method-level before hooks.
/// Method-level after hooks run before class-level after hooks.
/// Class-level hooks resolve independently for each mapping method's source/target type.
/// </remarks>
[Mappa]
[MappaBeforeMap(nameof(ClassBefore))]
[MappaAfterMap(nameof(ClassAfter))]
public sealed partial class MappaBeforeAfterMapHooksAttributeMapper
{
    /// <summary>
    /// Gets the recorded hook invocation order for the latest mapping.
    /// </summary>
    public Collection<string> HookCalls { get; } = [];

    /// <summary>
    /// Map a person while mutating the source before mapping and the target after mapping.
    /// </summary>
    /// <param name="input">The source person.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The mapped person.</returns>
    [MappaBeforeMap(nameof(MethodBefore))]
    [MappaAfterMap(nameof(MethodAfter))]
    public partial BeforeAfterMapHookPersonModel MapPerson(BeforeAfterMapHookPersonModel input, MappaContext context);

    /// <summary>
    /// Map a counter using only the class-level hooks that resolve for this type.
    /// </summary>
    /// <param name="input">The source counter.</param>
    /// <returns>The mapped counter.</returns>
    public partial BeforeAfterMapHookCounterModel MapCounter(BeforeAfterMapHookCounterModel input);

    private void ClassBefore(ref BeforeAfterMapHookPersonModel input, MappaContext context)
    {
        this.HookCalls.Add("class-before");
        input.Score += 1;
        if (context.TryGetValue<string>("suffix", out var text))
        {
            input.Name = $"{input.Name}-{text}";
        }
    }

    private void ClassBefore(ref BeforeAfterMapHookCounterModel input)
    {
        this.HookCalls.Add("class-before");
        input.Value += 1;
    }

    private void MethodBefore(ref BeforeAfterMapHookPersonModel input)
    {
        this.HookCalls.Add("method-before");
        input.Score += 10;
    }

    private void MethodAfter(ref BeforeAfterMapHookPersonModel target)
    {
        this.HookCalls.Add("method-after");
        target.Name = $"{target.Name}-method";
    }

    private void ClassAfter(ref BeforeAfterMapHookPersonModel target)
    {
        this.HookCalls.Add("class-after");
        target.Name = $"{target.Name}-class";
    }

    private void ClassAfter(ref BeforeAfterMapHookCounterModel target)
    {
        this.HookCalls.Add("class-after");
        target.Value += 100;
    }
}