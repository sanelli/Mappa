// <copyright file="TargetClassModelWithSingleMapperConstructorFromSourceClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target model for class <see cref="SourceClassModel"/>
/// with a single mapping constructor.
/// </summary>
public sealed class TargetClassModelWithSingleMapperConstructorFromSourceClassModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassModelWithSingleMapperConstructorFromSourceClassModel"/> class.
    /// </summary>
    /// <param name="sourceClassModel">The input model.</param>
    public TargetClassModelWithSingleMapperConstructorFromSourceClassModel(SourceClassModel sourceClassModel)
    {
        ArgumentNullException.ThrowIfNull(sourceClassModel);

        this.ParamA = sourceClassModel.ParamA;
        this.ParamB = sourceClassModel.ParamB;
    }

    /// <summary>
    /// Gets an integer value.
    /// </summary>
    public int ParamA { get; }

    /// <summary>
    /// Gets an enumeration value.
    /// </summary>
    public CountingValues ParamB { get; }
}