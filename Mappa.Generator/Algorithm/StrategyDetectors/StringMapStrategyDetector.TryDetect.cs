// <copyright file="StringMapStrategyDetector.TryDetect.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// String mapping detection branches for <see cref="StringMapStrategyDetector"/>.
/// </summary>
internal sealed partial class StringMapStrategyDetector
{
    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        if (this.TryDetectPrimitiveStringMappings(out mapStrategy)
            || this.TryDetectParseAndToStringMappings(out mapStrategy))
        {
            return true;
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool TryDetectPrimitiveStringMappings(out MapStrategy mapStrategy)
        => this.TryDetectStringToNumber(out mapStrategy)
           || this.TryDetectStringToDateTime(out mapStrategy)
           || this.TryDetectStringToDateTimeOffset(out mapStrategy)
           || this.TryDetectStringToTimeSpan(out mapStrategy)
           || this.TryDetectStringToTimeOnly(out mapStrategy)
           || this.TryDetectStringToDateOnly(out mapStrategy);

    private bool TryDetectParseAndToStringMappings(out MapStrategy mapStrategy)
        => this.TryDetectStringToGuid(out mapStrategy)
           || this.TryDetectStringToUri(out mapStrategy)
           || this.TryDetectUsingStaticParseMethod(out mapStrategy)
           || this.TryDetectToString(out mapStrategy);
}