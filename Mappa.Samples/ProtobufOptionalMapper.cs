// <copyright file="ProtobufOptionalMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper showcasing the <see cref="MappaSettingsAttribute.ProtobufOptional"/> setting for protobuf.
/// </summary>
[Mappa]
[MappaSettings(ProtobufOptional = BooleanSetting.Enable)]
public sealed partial class ProtobufOptionalMapper
{
    /// <summary>
    /// Map from protobuf optional to a generic target class.
    /// </summary>
    /// <param name="input">The protobuf message model with optional values.</param>
    /// <returns>The target model.</returns>
    [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
    public partial TargetClassModel Map(SourceProtobufOptionalModel input);

    /// <summary>
    /// Map from generic source model to protobuf.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The target protobuf model with optional values.</returns>
    public partial TargetProtobufOptionalModel MapToOptionalProtobuf(SourceClassModel input);

    /// <summary>
    /// Map from optional protobuf to optional protobuf.
    /// </summary>
    /// <param name="input">The protobuf message model with optional values.</param>
    /// <returns>The target protobuf model with optional values.</returns>
    public partial TargetProtobufOptionalModel MapToOptionalProtobuf(SourceProtobufOptionalModel input);
}