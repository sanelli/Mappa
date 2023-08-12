// <copyright file="StringMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for string related strategies.
/// </summary>
internal sealed class StringMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    public StringMapStrategyDetector(MappaMapAlgorithmContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        // 01. string -> numeric : ParseNumberStrategy
        if (this.CanMapStringToNumber())
        {
            mapStrategy = new StringToNumberMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 02. string -> DateTime : ParseDateTimeStrategy
        else if (this.CanMapStringToDateTime())
        {
            mapStrategy = new StringToDateTimeMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 03. S -> string : InvokeToStringStrategy
        else if (this.CanMapToString())
        {
            mapStrategy = new InvokeToStringMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapStringToNumber()
    {
        var isTargetDateTime = this.context.TargetType.IsNumeric();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTime && isSourceString;
    }

    private bool CanMapStringToDateTime()
    {
        var isTargetDateTime = this.context.TargetType.IsDateTime();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTime && isSourceString;
    }

    private bool CanMapToString()
    {
        var isTargetString = this.context.TargetType.IsString();
        return isTargetString;
    }
}