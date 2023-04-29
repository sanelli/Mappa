// <copyright file="ConstructorMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for the constructor strategies.
/// </summary>
internal sealed class ConstructorMapStrategyDetector
    : IMapStrategyDetector
{
    /// <inheritdoc/>
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        // 01. Constructor TargetType(SourceType input) exists -> InvokeMappingConstructorStrategy ( IMapStrategy(T.InputParameterType, S) )
        // TODO: Implement me
        // 02. Can map individual properties. -> InvokeConstructorStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        // TODO: Implement me
        throw new NotImplementedException();
    }
}