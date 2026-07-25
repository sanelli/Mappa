// <copyright file="EnumMappingConfigurationMappers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="int"/> with a <see cref="MappaMapEnumMemberAttribute{TEnum}"/> integral override.
/// </summary>
[Mappa]
public sealed partial class EnumMemberIntMapper
{
    /// <summary>
    /// Maps a status to its integral code, remapping <see cref="ConfigStatus.Inactive"/> to <c>99</c>.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped integral code.</returns>
    [MappaMapEnumMember<ConfigStatus>(ConfigStatus.Inactive, 99)]
    public partial int Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="string"/> with a <see cref="MappaMapEnumMemberAttribute{TEnum}"/> string override.
/// </summary>
[Mappa]
public sealed partial class EnumMemberStringMapper
{
    /// <summary>
    /// Maps a status to its string representation, remapping <see cref="ConfigStatus.Inactive"/> to <c>disabled</c>.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped string.</returns>
    [MappaMapEnumMember<ConfigStatus>(ConfigStatus.Inactive, "disabled")]
    public partial string Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigSourceStatus"/> to <see cref="ConfigTargetStatus"/> with a two-enum member override.
/// </summary>
[Mappa]
public sealed partial class EnumMemberTwoEnumMapper
{
    /// <summary>
    /// Maps a source deployment status to the target enum, pairing <see cref="ConfigSourceStatus.Offline"/>
    /// with <see cref="ConfigTargetStatus.Standby"/> explicitly.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped target status.</returns>
#pragma warning disable MP00039 // Partial enum mapping is intentional; ConfigSourceStatus.Legacy has no target member
    [MappaMapEnumMember<ConfigSourceStatus, ConfigTargetStatus>(ConfigSourceStatus.Offline, ConfigTargetStatus.Standby)]
    public partial ConfigTargetStatus Map(ConfigSourceStatus input);
#pragma warning restore MP00039
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="int"/> while excluding <see cref="ConfigStatus.Deprecated"/>.
/// </summary>
[Mappa]
public sealed partial class EnumIgnoreMapper
{
    /// <summary>
    /// Maps a status to its integral code, ignoring <see cref="ConfigStatus.Deprecated"/>.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped integral code.</returns>
    [MappaMapEnumIgnore<ConfigStatus>(ConfigStatus.Deprecated)]
    public partial int Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="int"/> with <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.
/// </summary>
[Mappa]
public sealed partial class EnumDefaultUseDefaultValueIntegralMapper
{
    /// <summary>
    /// Maps a status to its integral code, returning <c>42</c> for unmapped values.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped integral code.</returns>
    [MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.UseDefaultValue, 42)]
    public partial int Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="string"/> with <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.
/// </summary>
[Mappa]
public sealed partial class EnumDefaultUseDefaultValueStringMapper
{
    /// <summary>
    /// Maps a status to its string representation, returning <c>unknown</c> for unmapped values.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped string.</returns>
    [MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.UseDefaultValue, "unknown")]
    public partial string Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigSourceStatus"/> to <see cref="ConfigTargetStatus"/> with an enum default fallback.
/// </summary>
[Mappa]
public sealed partial class EnumDefaultUseDefaultValueEnumMapper
{
    /// <summary>
    /// Maps a source deployment status to the target enum, returning <see cref="ConfigTargetStatus.Offline"/> for unmapped values.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped target status.</returns>
#pragma warning disable MP00039 // Partial enum mapping is intentional; ConfigSourceStatus.Legacy has no target member
    [MappaMapEnumDefault<ConfigTargetStatus>(MappaMapEnumDefaultBehavior.UseDefaultValue, ConfigTargetStatus.Offline)]
    public partial ConfigTargetStatus Map(ConfigSourceStatus input);
#pragma warning restore MP00039
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="int"/> combining ignore and default fallback behaviour.
/// </summary>
[Mappa]
public sealed partial class EnumIgnoreAndDefaultMapper
{
    /// <summary>
    /// Maps a status to its integral code, ignoring <see cref="ConfigStatus.Inactive"/> and returning <c>42</c> for it.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped integral code.</returns>
    [MappaMapEnumIgnore<ConfigStatus>(ConfigStatus.Inactive)]
    [MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.UseDefaultValue, 42)]
    public partial int Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="ConfigStatus"/> to <see cref="int"/> with explicit throw-on-default behaviour.
/// </summary>
[Mappa]
public sealed partial class EnumDefaultThrowMapper
{
    /// <summary>
    /// Maps a status to its integral code, throwing when the value cannot be mapped.
    /// </summary>
    /// <param name="input">The source status.</param>
    /// <returns>The mapped integral code.</returns>
    [MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.Throw)]
    public partial int Map(ConfigStatus input);
}

/// <summary>
/// Maps <see cref="EnumConfigSourceModel"/> to <see cref="EnumConfigTargetModel"/> with nested enum member overrides.
/// </summary>
[Mappa]
public sealed partial class EnumConfigClassPropertyMapper
{
    /// <summary>
    /// Maps a configuration model, remapping nested <see cref="ConfigStatus.Inactive"/> to status code <c>99</c>.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped target model.</returns>
    [MappaMapEnumMember<ConfigStatus>(ConfigStatus.Inactive, 99)]
    public partial EnumConfigTargetModel Map(EnumConfigSourceModel input);
}

/// <summary>
/// Maps <see cref="EnumConfigMultiDefaultSourceModel"/> to <see cref="EnumConfigMultiDefaultTargetModel"/>
/// with different <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> per nested enum.
/// </summary>
[Mappa]
public sealed partial class EnumConfigMultiDefaultClassMapper
{
    /// <summary>
    /// Maps a configuration model applying throw-on-default for status and integral default for priority.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped target model.</returns>
    [MappaMapEnumDefault<ConfigStatus>(MappaMapEnumDefaultBehavior.Throw)]
    [MappaMapEnumDefault<ConfigPriority>(MappaMapEnumDefaultBehavior.UseDefaultValue, 0)]
    public partial EnumConfigMultiDefaultTargetModel Map(EnumConfigMultiDefaultSourceModel input);
}